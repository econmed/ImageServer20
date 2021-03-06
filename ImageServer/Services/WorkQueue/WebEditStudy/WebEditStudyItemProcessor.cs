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

using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    public class WebEditStudyItemProcessor : BaseItemProcessor
    {
        #region Private Fields

        private ServerFilesystemInfo _filesystem;
        private Study _study;
        private Patient _patient;

        #endregion

        #region Private Methods

        private void LoadAdditionalEntities()
        {
            Debug.Assert(ServerPartition != null);
            Debug.Assert(StorageLocation != null);

			using (ExecutionContext context = new ExecutionContext())
			{
				if (_filesystem != null)
					_filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);
				_study = StorageLocation.LoadStudy(context.ReadContext);
				_patient = Patient.Load(context.ReadContext, _study.PatientKey);
			}
        }

        #endregion

        #region Overriden Protected Methods
		protected override void ProcessItem(Model.WorkQueue item)
		{
			LoadAdditionalEntities();

			using (StudyEditor editor = new StudyEditor(ServerPartition, StorageLocation, _patient, Study))
			{
				if (!editor.Edit(item.Data.DocumentElement))
				{
					FailQueueItem(WorkQueueItem, editor.FailureReason);
					Platform.Log(LogLevel.Error, "Study Edit failed. WorkQueueKey:{0}. Reason={1}", WorkQueueItem.Key,
					             editor.FailureReason);
				}
				else
				{
					// update this to reflect any changes to the storage location, eg Study Folder
					StorageLocation = editor.NewStorageLocation;
					Platform.CheckForNullReference(StorageLocation, "StorageLocation");
					Complete();
				}
			}
		}

    	#endregion

        #region Protected Methods
        protected void Complete()
        {
			PostProcessing(WorkQueueItem, 
				WorkQueueProcessorStatus.Complete, 
				WorkQueueProcessorDatabaseUpdate.ResetQueueState);
            Platform.Log(LogLevel.Info, "Study Edit completed. WorkQueueKey={0}", WorkQueueItem.Key);
        }

        protected override bool CanStart()
        {
            Model.WorkQueue item = WorkQueueItem;

            IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(item,
                new []
                {
                    WorkQueueTypeEnum.StudyProcess,
                    WorkQueueTypeEnum.ReconcileStudy,
					WorkQueueTypeEnum.ProcessDuplicate,
            		WorkQueueTypeEnum.CleanupDuplicate,
        	        WorkQueueTypeEnum.CleanupStudy
                }, null);

            if (! (relatedItems == null || relatedItems.Count == 0))
            {
                PostponeItem("Study is being processed or reconciled.");
            	return false;
            }

			_filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);
        	Platform.CheckForNullReference(_filesystem, "filesystem");
			if (_filesystem.Full)
			{
				PostponeItem("The Filesystem is full.");
				return false;
			}

			if (!_filesystem.Writeable)
			{
				PostponeItem("The Filesystem is not writeable.");
				return false;
			}
			
        	return true;
        }

        #endregion
    }
}
