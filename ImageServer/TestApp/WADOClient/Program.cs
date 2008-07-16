using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.ServiceModel.Streaming;

namespace WADOClient
{
    class Program
    {
        static private string serverHost;
        static private int serverPort;
        private static ContentTypes type;
        private static string studyFolder;
        static bool repeat;

        static void Main(string[] args)
        {
            CommandLine cmdline = new CommandLine(args);
            IDictionary<string, string> parameters = cmdline.Named;

            serverHost = parameters["host"];
            serverPort = int.Parse(parameters["port"]);
            studyFolder = parameters["folder"];
            
            if (parameters.ContainsKey("type"))
            {
                if (parameters["type"] == "dicom") type = ContentTypes.Dicom;
                else if (parameters["type"] == "pixel") type = ContentTypes.RawPixel;
                else
                    throw new Exception("Invalid 'type' value");
            }

            repeat = cmdline.Switches.ContainsKey("repeat");

            Console.WriteLine("Retrieve image in {0}", studyFolder);

            DirectoryInfo dirInfo = new DirectoryInfo(studyFolder);

            DirectoryInfo[] partitions = dirInfo.GetDirectories();

            try
            {
                do
                {
                    Random r = new Random();
                    DirectoryInfo partition = partitions[r.Next(partitions.Length)];

                    DirectoryInfo[] studydates = partition.GetDirectories();
                    // pick one
                    DirectoryInfo studyate = studydates[r.Next(studydates.Length)];

                    DirectoryInfo[] studies = studyate.GetDirectories();

                    if (studies.Length > 0)
                    {
                        // pick one
                        DirectoryInfo study = studies[r.Next(studies.Length)];

                        string path = study.FullName;
                        RetrieveImages(path);
                    }

                    if (repeat)
                        Thread.Sleep(r.Next(10000));

                } while (repeat);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        
        }

        
        private static void RetrieveImages(string studyPath)
        {
            StreamingClient client = new StreamingClient();
            int totalFrameCount = 0;
            
            DirectoryInfo directoryInfo = new DirectoryInfo(studyPath);
            string studyUid = directoryInfo.Name;

            RateStatistics frameRate = new RateStatistics("Speed", "frame");
            RateStatistics speed = new RateStatistics("Speed", RateType.BYTES);
            AverageRateStatistics averageSpeed = new AverageRateStatistics(RateType.BYTES);
            ByteCountStatistics totalSize = new ByteCountStatistics("Size");
            
            frameRate.Start();
            speed.Start();

            Console.WriteLine("\n------------------------------------------------------------------------------------------------------------------------");

            string[] seriesDirs = Directory.GetDirectories(studyPath);
            foreach(string seriesPath in seriesDirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(seriesPath);
                string seriesUid = dirInfo.Name;
                string[] objectUidPath = Directory.GetFiles(seriesPath, "*.dcm");
                
                foreach (string uidPath in objectUidPath)
                {
                    FileInfo fileInfo = new FileInfo(uidPath);
                    string uid = fileInfo.Name.Replace(".dcm", "");
                    Console.Write("{0,-64}... ", uid);
                                    
                    try
                    {
                        string baseUri = String.Format("http://{0}:{1}/wado", serverHost, serverPort);
                        Stream imageStream;
                        StreamingResultMetaData imageMetaData;
                        FrameStreamingResultMetaData frameMetaData;
                                   
                        switch(type)
                        {
                            case ContentTypes.Dicom:
                                imageStream = client.RetrieveImage(baseUri, studyUid, seriesUid, uid, out imageMetaData);
                                totalFrameCount++;
                                averageSpeed.AddSample(imageMetaData.Speed);
                                totalSize.Value += (ulong)imageMetaData.ContentLength;

                                Console.WriteLine("1 dicom sop [{0,10}] in {1,12}\t[mime={2}]", ByteCountFormatter.Format((ulong)imageStream.Length), TimeSpanFormatter.Format(imageMetaData.Speed.ElapsedTime), imageMetaData.ResponseMimeType);
                                
                                break;

                            case ContentTypes.RawPixel:
                                TimeSpanStatistics elapsedTime = new TimeSpanStatistics();
                                elapsedTime.Start();
                                ulong instanceSize = 0;
                                int frameCount = 0;
                                do
                                {
                                    client.RetrievePixelData(baseUri, studyUid, seriesUid, uid, frameCount, out frameMetaData);
                                    totalFrameCount++;
                                    frameCount++;
                                    averageSpeed.AddSample(frameMetaData.Speed);
                                    totalSize.Value += (ulong)frameMetaData.ContentLength;
                                    instanceSize += (ulong)frameMetaData.ContentLength;

                                } while (!frameMetaData.IsLast);

                                elapsedTime.End();
                                Console.WriteLine("{0,3} frame(s) [{1,10}] in {2,12}\t[mime={3}]", frameCount, ByteCountFormatter.Format(instanceSize), elapsedTime.FormattedValue, frameMetaData.ResponseMimeType);
                                break;

                            default:

                                imageStream = client.RetrieveImage(baseUri, studyUid, seriesUid, uid, out imageMetaData);
                                totalFrameCount++;
                                averageSpeed.AddSample(imageMetaData.Speed);
                                totalSize.Value += (ulong)imageMetaData.ContentLength;

                                Console.WriteLine("1 object [{0,10}] in {1,12}\t[mime={2}]", ByteCountFormatter.Format((ulong)imageStream.Length), TimeSpanFormatter.Format(imageMetaData.Speed.ElapsedTime), imageMetaData.ResponseMimeType);

                                break;
                        }

                    }
                    catch(Exception ex)
                    {
                        if (ex is WebException)
                        {
                            HttpWebResponse rsp = ( (ex as WebException).Response as HttpWebResponse);
                        
                            string msg = String.Format("Error: {0} : {1}", rsp.StatusCode,HttpUtility.HtmlDecode(rsp.StatusDescription)
                                );
                            Console.WriteLine(msg);
                        }
                        else
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            frameRate.SetData(totalFrameCount);
            frameRate.End();
            speed.SetData(totalSize.Value);
            speed.End();


            Console.WriteLine("\nTotal {0,3} image(s)/frame(s) [{1,10}] in {2,12}   ==>  [ Speed: {3,12} or {4,12}]",
                    totalFrameCount, totalSize.FormattedValue,
                    TimeSpanFormatter.Format(frameRate.ElapsedTime),
                    frameRate.FormattedValue,
                    speed.FormattedValue
                    );

                                        
        }
    }
}
