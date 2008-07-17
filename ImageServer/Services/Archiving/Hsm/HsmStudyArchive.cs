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
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// Support class for archiving a specific study with an <see cref="HsmArchive"/>.
	/// </summary>
	public class HsmStudyArchive
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="hsmArchive">The HsmArchive to work with.</param>
		public HsmStudyArchive(HsmArchive hsmArchive)
		{
			_hsmArchive = hsmArchive;
		}
		private StudyXml _studyXml;
		private StudyStorageLocation _storageLocation;
		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
		private readonly HsmArchive _hsmArchive;
		private XmlDocument _archiveXml;
		private ServerRulesEngine _rulesEngine;

		/// <summary>
		/// Retrieves the storage location fromthe database for the specified study.
		/// </summary>
		/// <param name="queueItem">The queueItem.</param>
		/// <returns>true if a location was found, false otherwise.</returns>
		public void GetStudyStorageLocation(ArchiveQueue queueItem)
		{
			using (IReadContext read = _store.OpenReadContext())
			{
				IQueryStudyStorageLocation procedure = read.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
				parms.StudyStorageKey = queueItem.StudyStorageKey;
				IList<StudyStorageLocation> locationList = procedure.Execute(parms);

				foreach (StudyStorageLocation studyLocation in locationList)
				{
					_storageLocation = studyLocation;
					return;
				}
				return;
			}
		}

		/// <summary>
		/// Load the StudyXml file.
		/// </summary>
		/// <param name="studyXmlFile"></param>
		public void LoadStudyXml(string studyXmlFile)
		{
			using (Stream fileStream = new FileStream(studyXmlFile, FileMode.Open))
			{
				XmlDocument theDoc = new XmlDocument();

				StudyXmlIo.Read(theDoc, fileStream);

				_studyXml = new StudyXml(_storageLocation.StudyInstanceUid);
				_studyXml.SetMemento(theDoc);

				fileStream.Close();
			}
		}


		/// <summary>
		/// Archive the specified <see cref="ArchiveQueue"/> item.
		/// </summary>
		/// <param name="queueItem">The ArchiveQueue item to archive.</param>
		public void Run(ArchiveQueue queueItem)
		{
			try
			{
				_rulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyArchived, _hsmArchive.ServerPartition.GetKey());
				_rulesEngine.Load();

				GetStudyStorageLocation(queueItem);

				string studyFolder = _storageLocation.GetStudyPath();

				string studyXmlFile = Path.Combine(studyFolder,String.Format("{0}.xml",  _storageLocation.StudyInstanceUid));

				// Load the study Xml file, this is used to generate the list of dicom files to archive.
				LoadStudyXml(studyXmlFile);

				DicomMessage message = LoadMessageFromStudyXml();

				// Use the command processor to do the archival.
				ServerCommandProcessor commandProcessor = new ServerCommandProcessor("HSM Archive");

				_archiveXml = new XmlDocument();

				// Create the study date folder
				string zipFilename = Path.Combine(_hsmArchive.HsmPath, _storageLocation.StudyFolder);
				commandProcessor.AddCommand(new CreateDirectoryCommand(zipFilename));

				// Create a folder for the study
				zipFilename = Path.Combine(zipFilename, _storageLocation.StudyInstanceUid);
				commandProcessor.AddCommand(new CreateDirectoryCommand(zipFilename));

				// Save the archive data in the study folder, based on a filename with a date / time stamp
				string filename = String.Format("{0}.zip",Platform.Time.ToString("yyyy-MM-dd-HHmm"));
				string folderPlusZip = Path.Combine(_storageLocation.StudyFolder,
				                                    String.Format("{0}", _storageLocation.StudyInstanceUid));
				folderPlusZip = Path.Combine(folderPlusZip, filename);
				zipFilename = Path.Combine(zipFilename, filename);


				// Create the Xml data to store in the ArchiveStudyStorage table telling
				// where the archived study is located.
				XmlElement hsmArchiveElement = _archiveXml.CreateElement("HsmArchive");
				_archiveXml.AppendChild(hsmArchiveElement);
				XmlElement pathElement = _archiveXml.CreateElement("Path");
				hsmArchiveElement.AppendChild(pathElement);
				pathElement.InnerText = folderPlusZip;

				// Create the Zip file
				commandProcessor.AddCommand(new CreateStudyZipCommand(zipFilename,_studyXml,studyFolder));

				// Update the database.
				commandProcessor.AddCommand(new InsertArchiveStudyStorageCommand(queueItem.StudyStorageKey,queueItem.PartitionArchiveKey,queueItem.GetKey(),_storageLocation.ServerTransferSyntaxKey, _archiveXml));

				// Apply the rules engine.
				ServerActionContext context = new ServerActionContext(message, _storageLocation.FilesystemKey, _hsmArchive.PartitionArchive.ServerPartitionKey, queueItem.StudyStorageKey);

				context.CommandProcessor = commandProcessor;

				_rulesEngine.Execute(context);

				if (!commandProcessor.Execute())
				{
					Platform.Log(LogLevel.Error, "Unexpected failure archiving study");

					_hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Failed, Platform.Time);
				}
				else
					Platform.Log(LogLevel.Info, "Successfully archived study {0} on {1}", _storageLocation.StudyInstanceUid,
					             _hsmArchive.PartitionArchive.Description);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception archiving study: {0} on {1}",
				             _storageLocation.StudyInstanceUid, _hsmArchive.PartitionArchive.Description);
				_hsmArchive.UpdateArchiveQueue(queueItem, ArchiveQueueStatusEnum.Failed, Platform.Time);
			}
		}

		private DicomMessage LoadMessageFromStudyXml()
		{
			foreach (SeriesXml seriesXml in _studyXml)
				foreach (InstanceXml instanceXml in seriesXml)
				{
					// Skip non-image objects
					if (instanceXml.SopClass.Equals(SopClass.KeyObjectSelectionDocumentStorage)
					    || instanceXml.SopClass.Equals(SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass)
						|| instanceXml.SopClass.Equals(SopClass.BlendingSoftcopyPresentationStateStorageSopClass)
						|| instanceXml.SopClass.Equals(SopClass.ColorSoftcopyPresentationStateStorageSopClass))
						continue;

					return new DicomMessage(new DicomAttributeCollection(), instanceXml.Collection);
				}

			return null;
		}
	}
}
