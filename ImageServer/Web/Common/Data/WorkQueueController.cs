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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

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
        	using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
        	{
				IQueryStudyStorageLocation select = ctx.GetBroker<IQueryStudyStorageLocation>();

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
        			             "WorkQueueController:LoadStorageLocation: multiple study storage found for work queue item {0}",
        			             item.Key.Key);
        		}

        		return storages[0];
        	}
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
                || item.WorkQueueStatusEnum == WorkQueueStatusEnum.Idle
                /* somebody claimed it but hasn't updated it for quite a while */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress &&
                    !String.IsNullOrEmpty(item.ProcessorID) &&
                    item.ScheduledTime < Platform.Time.Subtract(TimeSpan.FromHours(12)));
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
                /* somebody claimed it but hasn't updated it for quite a while */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress &&
                    !String.IsNullOrEmpty(item.ProcessorID) &&
                    item.ScheduledTime < Platform.Time.Subtract(TimeSpan.FromHours(12)));

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
                /* somebody claimed it but hasn't updated it for quite a while */
                ||
                (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress &&
                 !String.IsNullOrEmpty(item.ProcessorID) &&
                 item.ScheduledTime < Platform.Time.Subtract(TimeSpan.FromHours(12)))
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
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.WebDeleteStudy);
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
                
                IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

                using (IReadContext ctx = _store.OpenReadContext())
                {
                    IWebQueryWorkQueue broker = ctx.GetBroker<IWebQueryWorkQueue>();
                    list = broker.Find(parameters);
                }

                return list;
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Error, "FindWorkQueue failed", e);
            	return new List<WorkQueue>();
            }
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
        /// <returns>
        /// A value indicating whether all <see cref="WorkQueue"/> in <paramref name="items"/> have been reset successfully.
        /// If one or more item cannot be reset, <b>false</b> will be returned and all changes are not committed.
        /// </returns>
        public bool ResetWorkQueueItems(IList<WorkQueue> items, DateTime newScheduledTime, DateTime expirationTime)
        {
            if (items == null || items.Count==0)
                return false;

            
            WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
            columns.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
            columns.ProcessorID = "";
            columns.FailureCount = 0;
            columns.ScheduledTime = newScheduledTime;
            columns.ExpirationTime = expirationTime;

            bool result = true;
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
                
                foreach(WorkQueue item in items)
                {
                    if (!workQueueBroker.Update(item.Key, columns))
                    {
                        result = false;
                        break;
                        
                    }
                }

                if (result)
                    ctx.Commit();
            }

            return result;
        }


        public bool ReprocessWorkQueueItem(WorkQueue item)
        {
            WorkQueue newItem = null;
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {

                // delete current workqueue
                IWorkQueueUidEntityBroker uidBroker = ctx.GetBroker<IWorkQueueUidEntityBroker>();
                WorkQueueUidSelectCriteria criteria = new WorkQueueUidSelectCriteria();
                criteria.WorkQueueKey.EqualTo(item.GetKey());
                
                if (uidBroker.Delete(criteria)>=0)
                {
                    IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
                    if (workQueueBroker.Delete(item.GetKey()))
                    {
                        WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
                        columns.InsertTime = Platform.Time;
                        columns.ScheduledTime = Platform.Time;
                        columns.ServerPartitionKey = item.ServerPartitionKey;
                        columns.StudyStorageKey = item.StudyStorageKey;
                        columns.WorkQueuePriorityEnum = WorkQueuePriorityEnum.Medium;
                        columns.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
                        columns.WorkQueueTypeEnum = WorkQueueTypeEnum.ReprocessStudy;
                        columns.ExpirationTime = Platform.Time.Add(TimeSpan.FromMinutes(15));
                        newItem = workQueueBroker.Insert(columns);

                        if (newItem!=null)
                            ctx.Commit();
                    }

                }
            }

            return newItem != null;
        }
        #endregion
    }
}
