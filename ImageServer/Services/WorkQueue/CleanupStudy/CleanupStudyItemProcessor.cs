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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupStudy
{
    /// <summary>
    /// For processing 'CleanupStudy' WorkQueue items.
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
    public class CleanupStudyItemProcessor : BaseItemProcessor
    {

        private void CheckEmptyStudy(Model.WorkQueue item)
        {
            using (IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IStudyEntityBroker study = context.GetBroker<IStudyEntityBroker>();
                StudySelectCriteria criteria = new StudySelectCriteria();

                criteria.StudyInstanceUid.EqualTo(StorageLocation.StudyInstanceUid);
                criteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);

                int count = study.Count(criteria);
                if (count == 0)
                {
                    IDeleteStudyStorage delete = context.GetBroker<IDeleteStudyStorage>();

                    DeleteStudyStorageParameters parms = new DeleteStudyStorageParameters
                                                         	{
                                                         		ServerPartitionKey = item.ServerPartitionKey,
                                                         		StudyStorageKey = item.StudyStorageKey
                                                         	};

                	delete.Execute(parms);
                }
                context.Commit();
            }
        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
			if (!LoadWritableStorageLocation(item))
			{
				Platform.Log(LogLevel.Warn, "Unable to find readable location when processing CleanupStudy WorkQueue item, rescheduling");
                PostponeItem(item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2), "Unable to find readable location.");

				return;
			}

            LoadUids(item);
            
            if (WorkQueueUidList.Count == 0)
            {
                // No UIDs associated with the WorkQueue item.  Set the status back to idle
				if (item.ExpirationTime <= Platform.Time)
				{
					Platform.Log(LogLevel.Info, "Applying rules engine to study being cleaned up to ensure disk management is applied.");

					// Run Study / Series Rules Engine.
					StudyRulesEngine engine = new StudyRulesEngine(StorageLocation,ServerPartition);
					engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed);
					StorageLocation.LogFilesystemQueue();

					PostProcessing(item,
								   WorkQueueProcessorStatus.Complete,
								   WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				}
				else
				{
					PostProcessing(item,
					               WorkQueueProcessorStatus.IdleNoDelete,
					               WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				}

				// This will just delete the study, if there's no images that have been sucessfully processed.
            	CheckEmptyStudy(item);
                return;
            }

        	Platform.Log(LogLevel.Info,
        	             "Starting Cleanup of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
        	             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
        	             Study.AccessionNumber, ServerPartition.Description);

            string basePath = StorageLocation.GetStudyPath();

            using (IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                foreach (WorkQueueUid sop in WorkQueueUidList)
                {
                    string path = Path.Combine(basePath, sop.SeriesInstanceUid);

                    path = Path.Combine(path, sop.SopInstanceUid);

                    if (sop.Extension != null)
                        path += "." + sop.Extension;
                    else
                        path += ServerPlatform.DicomFileExtension;

                    try
                    {
                        if (File.Exists(path))
                        {
							FileUtils.Delete(path);
                        }
                        IWorkQueueUidEntityBroker delete = context.GetBroker<IWorkQueueUidEntityBroker>();

                        delete.Delete(sop.GetKey());
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception deleting file: {0}", path);
                    }
                }

                context.Commit();
            }

        	Platform.Log(LogLevel.Info,
        	             "Completed Cleanup of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
        	             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
        	             Study.AccessionNumber, ServerPartition.Description);

			PostProcessing(item, 
				WorkQueueProcessorStatus.Pending, 
				WorkQueueProcessorDatabaseUpdate.ResetQueueState);

        }

        protected override bool CanStart()
        {
            return true; // can start anytime
        }
    }
}
