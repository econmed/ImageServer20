﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Process
{
    /// <summary>
    /// Represents the context during processing of DICOM object.
    /// </summary>
    public class SopProcessingContext
    {
        #region Private Members

        private readonly ServerCommandProcessor _commandProcessor;
        private readonly StudyStorageLocation _studyLocation;
        private readonly string _group;

    	#endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SopProcessingContext"/>
        /// </summary>
        /// <param name="commandProcessor">The <see cref="ServerCommandProcessor"/> used in the context</param>
        /// <param name="studyLocation">The <see cref="StudyStorageLocation"/> of the study being processed</param>
        /// <param name="uidGroup">A String value respresenting the group of SOP instances which are being processed.</param>
        public SopProcessingContext(ServerCommandProcessor commandProcessor, StudyStorageLocation studyLocation, string uidGroup)
        {
            _commandProcessor = commandProcessor;
            _studyLocation = studyLocation;
            _group = uidGroup;
        }
        
        #endregion

        #region Public Properties

        public ServerCommandProcessor CommandProcessor
        {
            get { return _commandProcessor; }
        }

        public StudyStorageLocation StudyLocation
        {
            get { return _studyLocation; }
        }

        public string Group
        {
            get { return _group; }
        }

        #endregion
    }

    /// <summary>
    /// Provides helper method to process duplicates.
    /// </summary>
    static public class DuplicateSopProcessorHelper
    {
        #region Private Members

        // TODO: Make these values configurable
      
        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts the duplicate DICOM file into the <see cref="WorkQueue"/> for processing (if applicable).
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="file">Thje duplicate DICOM file being processed.</param>
        /// <returns>A <see cref="DicomProcessingResult"/> that contains the result of the processing.</returns>
        /// <remarks>
        /// This method inserts <see cref="ServerCommand"/> into <paramref name="context.CommandProcessor"/>.
        /// The outcome of the operation depends on the <see cref="DuplicateSopPolicyEnum"/> of the <see cref="ServerPartition"/>.
        /// If it is set to <see cref="DuplicateSopPolicyEnum.CompareDuplicates"/>, the duplicate file will be
        /// inserted into the <see cref="WorkQueue"/> for processing.
        /// </remarks>
        static public DicomProcessingResult Process(SopProcessingContext context, DicomFile file)
        {
            Platform.CheckForNullReference(file, "file");
            Platform.CheckForNullReference(context, "context");
            Platform.CheckMemberIsSet(context.Group, "parameters.Group");
            Platform.CheckMemberIsSet(context.CommandProcessor, "parameters.CommandProcessor");
            Platform.CheckMemberIsSet(context.StudyLocation, "parameters.StudyLocation");

			String studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
        	String sopInstanceUid = file.MediaStorageSopInstanceUid;
			String accessionNumber = file.DataSet[DicomTags.AccessionNumber].GetString(0, string.Empty);

			DicomProcessingResult result = new DicomProcessingResult
			{
				DicomStatus = DicomStatuses.Success,
				Successful = true,
				StudyInstanceUid = studyInstanceUid,
				SeriesInstanceUid = seriesInstanceUid,
				SopInstanceUid = sopInstanceUid,
				AccessionNumber = accessionNumber
			};

        	string failureMessage;

            if (context.StudyLocation.ServerPartition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.SendSuccess))
            {
                Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, sending success response {0}", sopInstanceUid);
                return result;
            }
        	if (context.StudyLocation.ServerPartition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.RejectDuplicates))
        	{
        		failureMessage = String.Format("Duplicate SOP Instance received, rejecting {0}", sopInstanceUid);
        		Platform.Log(LogLevel.Info, failureMessage);
        		result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
        		return result;
        	}

        	if (context.StudyLocation.ServerPartition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.CompareDuplicates))
        	{
        		SaveDuplicate(context, file);
        		context.CommandProcessor.AddCommand(
					new UpdateWorkQueueCommand(file, context.StudyLocation, true, ServerPlatform.DuplicateFileExtension, context.Group));
        	}
        	else
        	{
        		failureMessage = String.Format("Duplicate SOP Instance received. Unsupported duplicate policy {0}.", context.StudyLocation.ServerPartition.DuplicateSopPolicyEnum);
        		result.SetError(DicomStatuses.DuplicateSOPInstance, failureMessage);
        		return result;
        	}

        	return result;
        }

		/// <summary>
		/// Create Duplicate SIQ Entry
		/// </summary>
		/// <param name="file"></param>
		/// <param name="location"></param>
		/// <param name="sourcePath"></param>
		/// <param name="queue"></param>
		/// <param name="uid"></param>
		public static void CreateDuplicateSIQEntry(DicomFile file, StudyStorageLocation location, string sourcePath, WorkQueue queue, WorkQueueUid uid)
		{
			Platform.Log(LogLevel.Info, "Creating Work Queue Entry for duplicate...");
			String uidGroup = queue.GroupID ?? queue.GetKey().Key.ToString();
			using (ServerCommandProcessor commandProcessor = new ServerCommandProcessor("Insert Work Queue entry for duplicate"))
			{
				commandProcessor.AddCommand(new FileDeleteCommand(sourcePath, true));

				SopProcessingContext sopProcessingContext = new SopProcessingContext(commandProcessor, location, uidGroup);
				DicomProcessingResult result = Process(sopProcessingContext, file);
				if (!result.Successful)
				{
					FailUid(uid, true);
					return;
				}

				commandProcessor.AddCommand(new DeleteWorkQueueUidCommand(uid));

				if (!commandProcessor.Execute())
				{
					Platform.Log(LogLevel.Error, "Unexpected error when creating duplicate study integrity queue entry: {0}", commandProcessor.FailureReason);
					FailUid(uid, true);
				}
			}
		}

        #endregion

        #region Private Methods

		private static void FailUid(WorkQueueUid sop, bool retry)
		{
			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IWorkQueueUidEntityBroker uidUpdateBroker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
				WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();
				if (!retry)
					columns.Failed = true;
				else
				{
					if (sop.FailureCount >= ImageServerCommonConfiguration.WorkQueueMaxFailureCount)
					{
						columns.Failed = true;
					}
					else
					{
						columns.FailureCount = sop.FailureCount++;
					}
				}

				uidUpdateBroker.Update(sop.GetKey(), columns);
				updateContext.Commit();
			}
		}

    	static private void SaveDuplicate(SopProcessingContext context, DicomFile file)
        {
            String sopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();

            String path = Path.Combine(context.StudyLocation.FilesystemPath, context.StudyLocation.PartitionFolder);
            context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

			path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
            context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, context.Group /* the AE title + timestamp */);
            context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, context.StudyLocation.StudyInstanceUid);
            context.CommandProcessor.AddCommand(new CreateDirectoryCommand(path));

            path = Path.Combine(path, sopUid);
			path += "." + ServerPlatform.DuplicateFileExtension;

            context.CommandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true));

            Platform.Log(ServerPlatform.InstanceLogLevel, "Duplicate ==> {0}", path);
        }

        #endregion
    }
}