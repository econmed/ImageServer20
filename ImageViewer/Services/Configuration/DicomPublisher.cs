using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Configuration
{
	//TODO: move to ImageViewer.Services.
	//Provide option in LDS to send upon import completion?
	public class DicomPublisher
	{
		private DicomPublisher()
		{
		}

		private static void DeleteEmptyFolders(string directory)
		{
			foreach (string subDirectory in Directory.GetDirectories(directory))
			{
				try
				{
					DateTime now = Platform.Time.ToUniversalTime();
					DirectoryInfo info = new DirectoryInfo(directory);

					if (now.Subtract(info.CreationTimeUtc) < TimeSpan.FromHours(12))
					{
						FileInfo[] files = info.GetFiles();
						if (files == null || files.Length == 0)
							Directory.Delete(subDirectory);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Failed to delete old temp directory ({0})", subDirectory);
				}
			}
		}

		public static List<Server> GetServers()
		{
			List<Server> servers = new List<Server>();
			ImageViewer.Services.ServerTree.ServerTree serverTree = new ImageViewer.Services.ServerTree.ServerTree();

			StringCollection paths = DefaultServerSettings.Default.DefaultServerPaths ?? new StringCollection();
			foreach (string path in paths)
			{
				Server server = serverTree.FindServer(path);
				if (server != null)
					servers.Add(server);
			}

			return servers;
		}

		//TODO: later, add progress callbacks/events.
		public static void PublishLocal(IList<DicomFile> files)
		{
			string tempFileDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ClearCanvas");
			DeleteEmptyFolders(tempFileDirectory);

			tempFileDirectory = System.IO.Path.Combine(tempFileDirectory, System.IO.Path.GetRandomFileName());
			Directory.CreateDirectory(tempFileDirectory);

			foreach (DicomFile file in files)
			{
				string savePath = System.IO.Path.Combine(tempFileDirectory, file.DataSet[DicomTags.SopInstanceUid]);
				savePath = System.IO.Path.ChangeExtension(savePath, ".dcm");
				file.Save(savePath);
			}

			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
			try
			{
				client.Open();
				FileImportRequest request = new FileImportRequest();
				request.FilePaths = new string[] { tempFileDirectory };
				request.Recursive = true;
				request.FileImportBehaviour = FileImportBehaviour.Move;
				client.Import(request);
				client.Close();
			}
			catch
			{
				client.Abort();
				throw;
			}
		}

		public static void PublishRemote(IEnumerable<DicomFile> messages)
		{
		}
	}
}
