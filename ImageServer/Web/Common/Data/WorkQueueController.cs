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
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Provides methods to interact with the database to query/update work queue.
    /// </summary>
    public class WorkQueueController : BaseController
    {
        #region Private Members

        
        #endregion

		#region Static Public Methods

		/// <summary>
        /// Gets the <see cref="StudyStorageLocation"/> for the study associated with the specified <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
		static public StudyStorageLocation GetLoadStorageLocation(WorkQueue item)
        {

            IQueryStudyStorageLocation select = HttpContextData.Current.ReadContext.GetBroker<IQueryStudyStorageLocation>();

        		StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
        		parms.StudyStorageKey = item.StudyStorageKey;

        		IList<StudyStorageLocation> storages = select.Find(parms);

        		if (storages == null || storages.Count == 0)
        		{
        			Platform.Log(LogLevel.Error, "Unable to find storage location for WorkQueue item: {0}",
        			             item.Key.ToString());
        			throw new ApplicationException("Unable to find storage location for WorkQueue item.");
        		}

        		if (storages.Count > 1)
        		{
        			Platform.Log(LogLevel.Warn,
        			             "WorkQueueController:LoadWritableStorageLocation: multiple study storage found for work queue item {0}",
        			             item.Key.Key);
        		}

        		return storages[0];
        	
        }

        /// <summary>
        /// Gets the <see cref="StudyStorageLocation"/> for the study associated with the specified <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public String GetLoadDuplicateStorageLocation(WorkQueue item)
        {
            XmlDocument document = item.Data;
            
           
            return document.ToString();

        }


        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually rescheduled.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanReschedule(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                // it's pending
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.Pending
                // it's idle
                || item.WorkQueueStatusEnum == WorkQueueStatusEnum.Idle;
                
        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually reset.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanReset(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                /* failed item */
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed
                /* nobody claimed it */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress && String.IsNullOrEmpty(item.ProcessorID))
                /* allow reset "stuck" items (except items that are InProgress)*/
                || (!item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.InProgress) && !ServerPlatform.IsActiveWorkQueue(item));

        }


        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually deleted from the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanDelete(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                /* failed item */
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed
                /* completed item */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.Completed)
                /* nobody claimed it */
                ||
                (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress &&
                 String.IsNullOrEmpty(item.ProcessorID))
                // allow deletes of some pending entries
                ||
                (item.WorkQueueStatusEnum != WorkQueueStatusEnum.InProgress &&
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.WebMoveStudy)
                ||
                (item.WorkQueueStatusEnum != WorkQueueStatusEnum.InProgress &&
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.WebEditStudy)
                ||
                (item.WorkQueueStatusEnum != WorkQueueStatusEnum.InProgress &&
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.AutoRoute)
                ||
                (item.WorkQueueStatusEnum != WorkQueueStatusEnum.InProgress &&
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.WebDeleteStudy)

				/* allow deletes of "stuck" items (except items that are InProgress)*/
                || (!item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.InProgress) && !ServerPlatform.IsActiveWorkQueue(item));
		}


        static public bool CanReprocess(WorkQueue item)
        {
            return
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed && item.WorkQueueTypeEnum == WorkQueueTypeEnum.StudyProcess;
        }
		#endregion Static Public Methods

		#region Public Methods

		/// <summary>
        /// Gets a list of <see cref="WorkQueue"/> items with specified criteria
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<WorkQueue> FindWorkQueue(WebWorkQueueQueryParameters parameters)
        {
            try
            {
                IList<WorkQueue> list;

                IWebQueryWorkQueue broker = HttpContextData.Current.ReadContext.GetBroker<IWebQueryWorkQueue>();
                list = broker.Find(parameters);
                return list;
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unable to retrieve work queue.");
            	return new List<WorkQueue>();
            }
        }

        public IList<WorkQueueInfo> GetWorkQueueOverview()
        {            
            IList<WorkQueueInfo> workQueueInfo = new List<WorkQueueInfo>();
            
            WorkQueueProcessorIDsParameters parameters = new WorkQueueProcessorIDsParameters();
            IWorkQueueProcessorIDs broker = HttpContextData.Current.ReadContext.GetBroker<IWorkQueueProcessorIDs>();
            IList<WorkQueue> processorIDList = broker.Find(parameters);

            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
            WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
            int numItems = 0;
            foreach(WorkQueue row in processorIDList)
            {
                criteria.ProcessorID.EqualTo(row.ProcessorID);
                numItems = adaptor.GetCount(criteria);
                workQueueInfo.Add(new WorkQueueInfo(row.ProcessorID, numItems));
            }
            return workQueueInfo;
        }

        /// <summary>
        /// Deletes a list of <see cref="WorkQueue"/> items from the system.
        /// </summary>
        /// <param name="items">The list of <see cref="WorkQueue"/> items to be deleted</param>
        /// <returns>A value indicating whether all items have been successfully deleted.</returns>
        /// 
        /// <remarks>
        /// If one or more <see cref="WorkQueue"/> in <paramref name="items"/> cannot be deleted, the method will return <b>false</b>
        /// and the deletion will be undone (i.e., All of the <see cref="WorkQueue"/> items will remain in the database)
        /// </remarks>
        public bool DeleteWorkQueueItems(IList<WorkQueue> items)
        {
            if (items == null || items.Count == 0)
                return false;

            bool result = true;

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext uctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IDeleteWorkQueue delete = uctx.GetBroker<IDeleteWorkQueue>();
                    
                foreach (WorkQueue item in items)
                {
					WorkQueueDeleteParameters parms = new WorkQueueDeleteParameters();
                    parms.ServerPartitionKey = item.ServerPartitionKey;
                    parms.StudyStorageKey = item.StudyStorageKey;
                    parms.WorkQueueKey = item.Key;
                    parms.WorkQueueTypeEnum = item.WorkQueueTypeEnum;
					// NOTE: QueueStudyState is reset by the stored procedure
                    if (!delete.Execute(parms))
                    {
                        Platform.Log(LogLevel.Error, "Unexpected error trying to delete WorkQueue entry");
                        result = false;
                    }
                }

                if (result)
                    uctx.Commit();
            }

            return result;
        }

        /// <summary>
        /// Reschedule a list of <see cref="WorkQueue"/> items
        /// </summary>
        /// <param name="items">List of <see cref="WorkQueue"/> items to be rescheduled</param>
        /// <param name="newScheduledTime">New schedule start date/time</param>
        /// <param name="expirationTime">New expiration date/time</param>
        /// <param name="priority">New priority</param>
        /// <returns>A value indicating whether all <see cref="WorkQueue"/> items in <paramref name="items"/> are updated successfully.</returns>
        /// <remarks>
        /// If one or more <see cref="WorkQueue"/> in <paramref name="items"/> cannot be rescheduled, all changes will be 
        /// reverted and <b>false</b> will be returned.
        /// </remarks>
        public bool RescheduleWorkQueueItems(IList<WorkQueue> items, DateTime newScheduledTime, DateTime expirationTime, WorkQueuePriorityEnum priority)
        {
            if (items == null || items.Count == 0)
                return false;

            WorkQueueUpdateColumns updatedColumns = new WorkQueueUpdateColumns();
            updatedColumns.WorkQueuePriorityEnum = priority;
            updatedColumns.ScheduledTime = newScheduledTime;
            updatedColumns.ExpirationTime = expirationTime;
            updatedColumns.FailureCount = 0;
            updatedColumns.FailureDescription = String.Empty;
            updatedColumns.LastUpdatedTime = Platform.Time;

            bool result = false;
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
                foreach (WorkQueue item in items)
                {
                    result = workQueueBroker.Update(item.Key, updatedColumns);
                    if (!result)
                    {
                        break;
                    }
                }
                if (result)
                    ctx.Commit();
            }

            return result;
        }


        /// <summary>
        /// Resets a list of <see cref="WorkQueue"/> items.
        /// </summary>
        /// <param name="items">List of <see cref="WorkQueue"/>  to be reset</param>
        /// <param name="newScheduledTime">The new scheduled start date/time for the entries</param>
        /// <param name="expirationTime">The new expiration start date/time for the entries</param>
        public void ResetWorkQueueItems(IList<WorkQueue> items, DateTime newScheduledTime, DateTime expirationTime)
        {
            if (items == null || items.Count==0)
                return;

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                IWebResetWorkQueue broker = ctx.GetBroker<IWebResetWorkQueue>();
                foreach(WorkQueue item in items)
                {
                    WebResetWorkQueueParameters parameters = new WebResetWorkQueueParameters
                                                                 {
                                                                     WorkQueueKey = item.Key,
                                                                     NewScheduledTime= newScheduledTime,
                                                                     NewExpirationTime = expirationTime
                                                                 };

                    if (!broker.Execute(parameters))
                    {
                        Platform.Log(LogLevel.Error,
                                     "Unexpected error when calling WebResetWorkQueue stored procedure. Could not reset {0} work queue entry {1}",
                                     item.WorkQueueTypeEnum.Description, item.Key);
                    }
                }
            }
        }

		public bool ReprocessWorkQueueItem(WorkQueue item)
		{
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
			using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				// delete current workqueue
				IWorkQueueUidEntityBroker uidBroker = ctx.GetBroker<IWorkQueueUidEntityBroker>();
				WorkQueueUidSelectCriteria criteria = new WorkQueueUidSelectCriteria();
				criteria.WorkQueueKey.EqualTo(item.GetKey());

				if (uidBroker.Delete(criteria) >= 0)
				{
					IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
					if (workQueueBroker.Delete(item.GetKey()))
					{
					    IList<StudyStorageLocation> locations = item.LoadStudyLocations(ctx);
                        if (locations!=null && locations.Count>0)
                        {
                            StudyReprocessor reprocessor = new StudyReprocessor();
                            String reason = String.Format("Reprocess failed {0}", item.WorkQueueTypeEnum);
                            WorkQueue reprocessEntry = reprocessor.ReprocessStudy(ctx, reason, locations[0], Platform.Time);
							if (reprocessEntry!=null)
								ctx.Commit();
                        	return reprocessEntry!=null;
                        }	
					}
				}
			}
			return false;
		}

    	#endregion
    }
}
