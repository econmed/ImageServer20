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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.PurgeStudy
{

	public class PurgeStudyItemProcessor : BaseItemProcessor, IWorkQueueItemProcessor
	{
		#region Private Members

		private ServerPartition _partition;

		#endregion

		#region Private Methods
		private void RemoveFilesystem()
		{
			foreach (StudyStorageLocation location in StorageLocationList)
			{
				string path = location.GetStudyPath();
				try
				{
					if (Directory.Exists(path))
					{
						Directory.Delete(path, true);

						DirectoryInfo info = Directory.GetParent(path);
						DirectoryInfo[] subdirs = info.GetDirectories();
						if (subdirs.Length == 0)
							Directory.Delete(info.FullName);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when trying to delete directory: {0}", path);
				}
			}
		}

		private void RemoveDatabase(Model.WorkQueue item)
		{
			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				// Setup the delete parameters
				DeleteFilesystemStudyStorageParameters parms = new DeleteFilesystemStudyStorageParameters();

				parms.ServerPartitionKey = item.ServerPartitionKey;
				parms.StudyStorageKey = item.StudyStorageKey;
				parms.StudyStatusEnum = StudyStatusEnum.Nearline;

				// Get the Insert Instance broker and do the insert
				IDeleteFilesystemStudyStorage delete = updateContext.GetBroker<IDeleteFilesystemStudyStorage>();

				if (false == delete.Execute(parms))
				{
					Platform.Log(LogLevel.Error, "Unexpected error when trying to delete study: {0} on partition {1}",
								 StorageLocation.StudyInstanceUid, _partition.Description);
				}
				else
					updateContext.Commit();
			}
		}
		#endregion

		#region Overridden Protected Method

		protected override void ProcessItem(Model.WorkQueue item)
		{
            //Load the storage location.
            LoadStorageLocation(item);

            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(item.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[] {WorkQueueTypeEnum.StudyProcess});
            workQueueCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending});

            List<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(item , workQueueCriteria);
            if (relatedItems != null && relatedItems.Count > 0)
            {
                // can't do it now. Reschedule it for future
                relatedItems.Sort(delegate(Model.WorkQueue item1, Model.WorkQueue item2)
                                      {
                                          return item1.ScheduledTime.CompareTo(item2.ScheduledTime);
                                      });

                DateTime newScheduledTime = relatedItems[0].ScheduledTime.AddMinutes(1);
                if (newScheduledTime < Platform.Time.AddMinutes(1))
                    newScheduledTime = Platform.Time.AddMinutes(1);

                PostponeItem(item, newScheduledTime, newScheduledTime.AddDays(1));
                Platform.Log(LogLevel.Info, "{0} postponed to {1}. Study UID={2}", item.WorkQueueTypeEnum, newScheduledTime, StorageLocation.StudyInstanceUid);
            }
            else
            {
                _partition = ServerPartition.Load(ReadContext, item.ServerPartitionKey);

                Platform.Log(LogLevel.Info, "Purging study '{0}' from partition '{1}'", StorageLocation.StudyInstanceUid,
                             _partition.Description);

                RemoveFilesystem();
            }

		    RemoveDatabase(item);

			// No need to remove / update the Queue entry, it was deleted as part of the delete process.
		}

		#endregion
	}
}
