#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    /// <summary>
    /// Class for reprocessing the rules engine for studies on a filesystem.
    /// </summary>
    public class FilesystemStudyProcessItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor
    {
        #region Private Members
        private readonly FilesystemReprocessStatistics _stats = new FilesystemReprocessStatistics();
        private readonly Dictionary<ServerPartition, ServerRulesEngine> _engines = new Dictionary<ServerPartition, ServerRulesEngine>();
        #endregion

        #region Private Methods

        /// <summary>
        /// Load the first instance from the first series of the StudyXml file for a study.
        /// </summary>
        /// <param name="location">The storage location of the study.</param>
        /// <returns></returns>
        private static DicomFile LoadInstance(StudyStorageLocation location)
        {
            string studyXml = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");

            if (!File.Exists(studyXml))
            {
                return null;
            }

            FileStream stream = new FileStream(studyXml, FileMode.Open);
            XmlDocument theDoc = new XmlDocument();
            StudyXmlIo.Read(theDoc, stream);
            stream.Close();
            stream.Dispose();
            StudyXml xml = new StudyXml();
            xml.SetMemento(theDoc);           
            
            IEnumerator<SeriesXml> seriesEnumerator = xml.GetEnumerator();
            if (seriesEnumerator.MoveNext())
            {
                SeriesXml seriesXml = seriesEnumerator.Current;
                IEnumerator<InstanceXml> instanceEnumerator = seriesXml.GetEnumerator();
                if (instanceEnumerator.MoveNext())
                {
                    InstanceXml instance = instanceEnumerator.Current;
					DicomFile file = new DicomFile("file.dcm",new DicomAttributeCollection(), instance.Collection);
                    file.TransferSyntax = instance.TransferSyntax;
                    return file;
                }
            }

            return null;
        }

        /// <summary>
        /// Reprocess a specific study.
        /// </summary>
        /// <param name="location">The storage location of the study to process.</param>
        /// <param name="engine">The rules engine to use when processing the study.</param>
        private void ProcessStudy(StudyStorageLocation location, ServerRulesEngine engine)
        {
            DicomFile msg = LoadInstance(location);
            if (msg == null)
            {
                Platform.Log(LogLevel.Error, "Unable to load file for study {0}", location.StudyInstanceUid);
                return;
            }

            ServerActionContext context = new ServerActionContext(msg, location.FilesystemKey, location.ServerPartitionKey, location.GetKey());
            context.CommandProcessor = new ServerCommandProcessor("Study Rule Processor");

            // Add a command to delete the current filesystemQueue entries, so that they can 
            // be reinserted by the rules engine.
            context.CommandProcessor.AddCommand(new DeleteFilesystemQueueCommand(location.GetKey()));

			// Execute the rules engine, insert commands to update the database into the command processor.
            engine.Execute(context);

			// Re-do insert into the archive queue.  Note the "update" flag must be set,
			// so that we don't insert a new record if the study has been already archived
			// Note also this should be done after the rules engine, so we don't insert into
			// the archive queue if a study delete has been input.
			context.CommandProcessor.AddCommand(
				new InsertArchiveQueueCommand(location.ServerPartitionKey, location.GetKey(), true));


            // Do the actual database updates.
            if (false == context.CommandProcessor.Execute())
            {
                Platform.Log(LogLevel.Error, "Unexpected failure processing Study level rules for study {0}", location.StudyInstanceUid);
            }
        }

        /// <summary>
        /// Reprocess a file systems
        /// </summary>
        /// <param name="filesystem"></param>
        private void ReprocessFilesystem(Filesystem filesystem)
        {
            DirectoryInfo filesystemDir = new DirectoryInfo(filesystem.FilesystemPath);

            foreach (DirectoryInfo partitionDir in filesystemDir.GetDirectories())
            {
                ServerPartition partition;
                if (GetServerPartition(partitionDir.Name, out partition) == false)
                {
                    Platform.Log(LogLevel.Error, "Unknown partition folder '{0}' in filesystem: {1}", partitionDir.Name,
                                 filesystem.Description);
                    continue;
                }

                // Since we found a partition, we should find a rules engine too.
                ServerRulesEngine engine = _engines[partition];

                foreach (DirectoryInfo dateDir in partitionDir.GetDirectories())
                {
                    foreach (DirectoryInfo studyDir in dateDir.GetDirectories())
                    {
                        String studyInstanceUid = studyDir.Name;
                        try
                        {
                            StudyStorageLocation location = LoadStorageLocation(partition.GetKey(), studyInstanceUid);
                            ProcessStudy(location, engine);
                            _stats.NumStudies++;
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error, e,
                                         "Unexpected exception loading storage location or processing study: {0} on partition {1}.",
                                         studyInstanceUid, partition.Description);
                        }                        
                    }
                }
            }
        }

        /// <summary>
        /// Get a ServerPartition object from the list of loaded partitions.
        /// </summary>
        /// <param name="partitionFolderName">The folder for the partition.</param>
        /// <param name="partition">The ServerPartition object.</param>
        /// <returns>true on success.</returns>
        private bool GetServerPartition(string partitionFolderName, out ServerPartition partition)
        {
            foreach (ServerPartition part in _engines.Keys)
            {
                if (part.PartitionFolder == partitionFolderName)
                {
                    partition = part;
                    return true;
                }
            }

            partition = null;
            return false;
        }

        /// <summary>
        /// Load the <see cref="ServerRulesEngine"/> for each partition.
        /// </summary>
        private void LoadRulesEngine()
        {
            IServerPartitionEntityBroker broker = ReadContext.GetBroker<IServerPartitionEntityBroker>();
            ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
            IList<ServerPartition> partitions = broker.Find(criteria);

            foreach (ServerPartition partition in partitions)
            {
                ServerRulesEngine engine =
                    new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyProcessed, partition.GetKey());
                _engines.Add(partition, engine);

                engine.Load();
            }            
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Process the <see cref="ServiceLock"/> item.
        /// </summary>
        /// <param name="item"></param>
		public void Process(Model.ServiceLock item)
        {
        	LoadRulesEngine();

        	ServerFilesystemInfo info = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

        	_stats.StudyRate.Start();
        	_stats.Filesystem = info.Filesystem.Description;

        	Platform.Log(LogLevel.Info, "Starting reprocess of filesystem: {0}", info.Filesystem.Description);

        	ReprocessFilesystem(info.Filesystem);

        	Platform.Log(LogLevel.Info, "Completed reprocess of filesystem: {0}", info.Filesystem.Description);

        	_stats.StudyRate.SetData(_stats.NumStudies);
        	_stats.StudyRate.End();

        	StatisticsLogger.Log(LogLevel.Info, _stats);

        	item.ScheduledTime = item.ScheduledTime.AddDays(1);

        	UnlockServiceLock(item, false, Platform.Time.AddDays(1));
        }

    	public new void Dispose()
        {
            base.Dispose();
        }
        #endregion
    }
}
