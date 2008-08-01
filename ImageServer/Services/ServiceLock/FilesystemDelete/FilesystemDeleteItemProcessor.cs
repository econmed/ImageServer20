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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemDelete
{
    /// <summary>
    /// Provide a convenient means to track duration of an event 
    /// </summary>
    class TimeTracker<T>
    {
        #region Delegates
        public delegate object ObjectKeyDelegate(T obj);
        #endregion

        private readonly Dictionary<object, DateTime> _lutObjectTrackingTime = new Dictionary<object, DateTime>();
        private ObjectKeyDelegate _del;

        public TimeTracker(ObjectKeyDelegate del)
        {
            _del = del;
        }
    
        public bool IsTracking(T obj)
        {
            return _lutObjectTrackingTime.ContainsKey(_del(obj));
        }

        public void StartTracking(T obj)
        {
            _lutObjectTrackingTime.Add(_del(obj), Platform.Time);
        }

        public void ResetTracking(T obj)
        {
            _lutObjectTrackingTime[_del(obj)] = Platform.Time;
        }


        public TimeSpan GetTrackingDuration(T obj)
        {
            if (!IsTracking(obj))
                return TimeSpan.Zero;

            return Platform.Time - _lutObjectTrackingTime[_del(obj)];
        }

        public void StopTracking(T obj)
        {
            _lutObjectTrackingTime.Remove(_del(obj));
        }
    }

    /// <summary>
    /// Class for processing 'FilesystemDelete' <see cref="Model.ServiceLock"/> rows.
    /// </summary>
    public class FilesystemDeleteItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor
    {
        private static TimeTracker<ServerFilesystemInfo> _fsTracker = new TimeTracker<ServerFilesystemInfo>(
                    delegate(ServerFilesystemInfo fs) { return fs.Filesystem.GetKey().Key; });

        #region Private Members
        static private DateTime? _scheduledMigrateTime = null;
        
        private FilesystemMonitor _monitor;
        private float _bytesToRemove;
        private int _studiesDeleted = 0;
        private int _studiesMigrated = 0;
		private int _studiesPurged = 0;
        #endregion

        #region Private Methods


        /// <summary>
        /// Initializes the scheduled time based on the last entry in the queue.
        /// </summary>
        private void InitializeScheduleTime()
        {
            if (_scheduledMigrateTime == null)
            {
                IWorkQueueEntityBroker workQueueBroker = ReadContext.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueSelectCriteria workQueueSearchCriteria = new WorkQueueSelectCriteria();
                workQueueSearchCriteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.MigrateStudy);
                workQueueSearchCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Pending, WorkQueueStatusEnum.InProgress });
                workQueueSearchCriteria.ScheduledTime.SortDesc(0);

                IList<WorkQueue> migrateItems = workQueueBroker.Find(workQueueSearchCriteria, 1);

                if (migrateItems != null && migrateItems.Count > 0)
                {
                    _scheduledMigrateTime = migrateItems[0].ScheduledTime;
                    Platform.Log(LogLevel.Info, "Last migration entry was scheduled for {0}. New migration will be scheduled after that time.", _scheduledMigrateTime.Value);
                }
                else
                    _scheduledMigrateTime = Platform.Time;
            }

            if (_scheduledMigrateTime < Platform.Time)
                _scheduledMigrateTime = Platform.Time;
        }


        /// <summary>
        /// Process StudyDelete Candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
        /// </summary>
        /// <param name="candidateList">The list of candidate studies for deleting.</param>
        private void ProcessStudyDeleteCandidates(IList<FilesystemQueue> candidateList)
        {
			if (candidateList.Count > 0)
				Platform.Log(LogLevel.Debug, "Scheduling delete study for {0} eligable studies...", candidateList.Count);
			
			foreach (FilesystemQueue queueItem in candidateList)
            {
                if (_bytesToRemove < 0)
                    return;

                // First, get the StudyStorage locations for the study, and calculate the disk usage.
                IQueryStudyStorageLocation studyStorageQuery = ReadContext.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters studyStorageParms = new StudyStorageLocationQueryParameters();
                studyStorageParms.StudyStorageKey = queueItem.StudyStorageKey;
                IList<StudyStorageLocation> storageList = studyStorageQuery.Execute(studyStorageParms);
                
                // Get the disk usage
                StudyStorageLocation location = storageList[0];
                
                float studySize = CalculateFolderSize(location.GetStudyPath());

                using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
					IInsertWorkQueueFromFilesystemQueue studyDelete = update.GetBroker<IInsertWorkQueueFromFilesystemQueue>();

                    InsertWorkQueueFromFilesystemQueueParameters insertParms = new InsertWorkQueueFromFilesystemQueueParameters();
                    insertParms.StudyStorageKey = location.GetKey();
                    insertParms.ServerPartitionKey = location.ServerPartitionKey;
                    DateTime expirationTime = Platform.Time.AddSeconds(10);
                    insertParms.ScheduledTime = expirationTime;
                    insertParms.ExpirationTime = expirationTime;
                    insertParms.DeleteFilesystemQueue = true;
                	insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.DeleteStudy;
                	insertParms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.DeleteStudy;

                    IList<WorkQueue> insertList = studyDelete.Execute(insertParms);
					if (insertList.Count == 0)
                    {
                        Platform.Log(LogLevel.Error, "Unexpected problem inserting 'StudyDelete' record into WorkQueue for Study {0}", location.StudyInstanceUid);
                    }
                    else
                    {
                        update.Commit();
                        _bytesToRemove -= studySize;
                        _studiesDeleted++;
                    }
                }
            }
        }

		/// <summary>
		/// Process StudyPurge <see cref="FilesystemQueue"/> entries.
		/// </summary>
		/// <param name="candidateList">The list of candidates for purging</param>
		private void ProcessStudyPurgeCandidates(IList<FilesystemQueue> candidateList)
		{
			if (candidateList.Count > 0)
				Platform.Log(LogLevel.Debug, "Scheduling purge study for {0} eligable studies...", candidateList.Count);

			foreach (FilesystemQueue queueItem in candidateList)
			{
				if (_bytesToRemove < 0)
					break;
				
				// First, get the StudyStorage locations for the study, and calculate the disk usage.
				IQueryStudyStorageLocation studyStorageQuery = ReadContext.GetBroker<IQueryStudyStorageLocation>();
				StudyStorageLocationQueryParameters studyStorageParms = new StudyStorageLocationQueryParameters();
				studyStorageParms.StudyStorageKey = queueItem.StudyStorageKey;
				IList<StudyStorageLocation> storageList = studyStorageQuery.Execute(studyStorageParms);

				// Get the disk usage
				StudyStorageLocation location = storageList[0]; // TODO: What should we do with other locations?
				float studySize = CalculateFolderSize(location.GetStudyPath());

				// Update the DB
				using (
					IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IInsertWorkQueueFromFilesystemQueue studyDelete = update.GetBroker<IInsertWorkQueueFromFilesystemQueue>();

					InsertWorkQueueFromFilesystemQueueParameters insertParms = new InsertWorkQueueFromFilesystemQueueParameters();
					insertParms.StudyStorageKey = location.GetKey();
					insertParms.ServerPartitionKey = location.ServerPartitionKey;
					DateTime expirationTime = Platform.Time.AddSeconds(10);
					insertParms.ScheduledTime = expirationTime;
					insertParms.ExpirationTime = expirationTime;
					insertParms.DeleteFilesystemQueue = true;
					insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.PurgeStudy;
					insertParms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.PurgeStudy;

					IList<WorkQueue> insertList = studyDelete.Execute(insertParms);
					if (insertList.Count == 0)
					{
						Platform.Log(LogLevel.Error, "Unexpected problem inserting 'PurgeStudy' record into WorkQueue for Study {0}",
						             location.StudyInstanceUid);
					}
					else
					{
						update.Commit();
						_bytesToRemove -= studySize;
						_studiesPurged++;
					}
				}
			}
		}

    	/// <summary>
        /// Process study migration candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
        /// </summary>
        /// <param name="candidateList">The list of candidate studies for deleting.</param>
		private void ProcessStudyMigrateCandidates(IList<FilesystemQueue> candidateList)
        {
        	Platform.CheckForNullReference(candidateList, "candidateList");

        	if (candidateList.Count > 0)
        		Platform.Log(LogLevel.Debug, "Scheduling tier-migration for {0} eligable studies...", candidateList.Count);

        	foreach (FilesystemQueue queueItem in candidateList)
        	{
        		if (_bytesToRemove < 0)
        		{
        			break;
        		}

        		// First, get the StudyStorage locations for the study, and calculate the disk usage.
        		IQueryStudyStorageLocation studyStorageQuery = ReadContext.GetBroker<IQueryStudyStorageLocation>();
        		StudyStorageLocationQueryParameters studyStorageParms = new StudyStorageLocationQueryParameters();
        		studyStorageParms.StudyStorageKey = queueItem.StudyStorageKey;
        		IList<StudyStorageLocation> storageList = studyStorageQuery.Execute(studyStorageParms);

        		// Get the disk usage
        		StudyStorageLocation location = storageList[0]; // TODO: What should we do with other locations?

        		float studySize = CalculateFolderSize(location.GetStudyPath());

        		using (
        			IUpdateContext update =
        				PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
        		{
					IInsertWorkQueueFromFilesystemQueue broker = update.GetBroker<IInsertWorkQueueFromFilesystemQueue>();

					InsertWorkQueueFromFilesystemQueueParameters insertParms = new InsertWorkQueueFromFilesystemQueueParameters();
        			insertParms.StudyStorageKey = location.GetKey();
        			insertParms.ServerPartitionKey = location.ServerPartitionKey;
        			insertParms.ScheduledTime = _scheduledMigrateTime.Value;
        			insertParms.ExpirationTime = _scheduledMigrateTime.Value.AddMinutes(1);
        			insertParms.DeleteFilesystemQueue = true;
					insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.MigrateStudy;
					insertParms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.TierMigrate;

        			Platform.Log(LogLevel.Debug, "Scheduling tier-migration for study {0} from {1} at {2}...",
        			             location.StudyInstanceUid, location.FilesystemTierEnum, _scheduledMigrateTime);
        			IList<WorkQueue> insertList = broker.Execute(insertParms);
					if (insertList.Count == 0)
        			{
        				Platform.Log(LogLevel.Error,
        				             "Unexpected problem inserting 'MigrateStudy' record into WorkQueue for Study {0}",
        				             location.StudyInstanceUid);
        			}
        			else
        			{
        				update.Commit();
        				_bytesToRemove -= studySize;
        				_studiesMigrated++;

        				// spread out the scheduled migration entries based on the size
        				// assuming that the larger the study the longer it will take to migrate
        				// The assumed migration speed is arbitarily chosen.
        				double migrationSpeed = ServiceLockSettings.Default.TierMigrationSpeed*1024*1024; // MB / sec
        				TimeSpan estMigrateTime = TimeSpan.FromSeconds(studySize/migrationSpeed);
        				_scheduledMigrateTime = _scheduledMigrateTime.Value.Add(estMigrateTime);
        				if (_studiesMigrated%10 == 0)
        				{
        					_scheduledMigrateTime.Value.AddSeconds(15);
        					Thread.Sleep(200); // The list may be long, let other processes have a chance to talk to the database
        				}
        			}
        		}
        	}
        }

		/// <summary>
		/// Do the actual Study Deletes
		/// </summary>
		/// <param name="item">The ServiceLock item</param>
		/// <param name="fs">The filesystem being worked on.</param>
		private void DoStudyDelete(ServerFilesystemInfo fs, Model.ServiceLock item)
        {
            DateTime deleteTime = Platform.Time;
            FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.DeleteStudy;

            while (_bytesToRemove > 0)
            {
				Platform.Log(LogLevel.Debug,
					 "{1:0.0} MBs needs to be removed from '{0}'. Querying for studies that can be deleted",
					 fs.Filesystem.Description, _bytesToRemove / (1024 * 1024));
                IList<FilesystemQueue> list =
                    GetFilesystemQueueCandidates(item, deleteTime, type);

                if (list.Count > 0)
                {
                    ProcessStudyDeleteCandidates(list);
                }
                else
                {
                    // No candidates
                    break;
                }
            }
        }

		/// <summary>
		/// Do the actual StudyPurge
		/// </summary>
		/// <param name="item"></param>
		/// <param name="fs">The filesystem being worked on.</param>
		private void DoStudyPurge(ServerFilesystemInfo fs, Model.ServiceLock item)
		{
			DateTime deleteTime = Platform.Time;
			FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.PurgeStudy;

			while (_bytesToRemove > 0)
			{
				Platform.Log(LogLevel.Debug,
							 "{1:0.0} MBs needs to be removed from '{0}'. Querying for studies that can be purged",
							 fs.Filesystem.Description, _bytesToRemove / (1024 * 1024));
				IList<FilesystemQueue> list =
					GetFilesystemQueueCandidates(item, deleteTime, type);

				if (list.Count > 0)
				{
					ProcessStudyPurgeCandidates(list);
				}
				else
				{
					// No candidates
					break;
				}
			}
		}

		/// <summary>
		/// Do the actual Study migration.
		/// </summary>
		/// <param name="fs">The filesystem</param>
		/// <param name="item">The ServiceLock item being processed.</param>
        private void DoStudyMigrate( ServerFilesystemInfo fs, Model.ServiceLock item)
        {
            FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.TierMigrate;

            while (_bytesToRemove > 0)
            {
                Platform.Log(LogLevel.Debug,
                             "{1:0.0} MBs needs to be removed from '{0}'. Querying for studies that can be migrated",
                             fs.Filesystem.Description, _bytesToRemove/(1024*1024));
                IList<FilesystemQueue> list = GetFilesystemQueueCandidates(item, Platform.Time, type);
                if (list.Count > 0)
                {
                    ProcessStudyMigrateCandidates(list);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns number of Delete Study, Tier Migrate, and Study Purge work queue items 
        /// that are still Pending or In Progress for the filesystem associated with the
        /// specified <see cref="ServiceLock"/>.
        /// </summary>
        /// <param name="item">The ServiceLock item.</param>
        /// <returns>The number of WorkQueue entries pending.</returns>
        private int CheckWorkQueueCount(Model.ServiceLock item)
        {
            IWorkQueueEntityBroker select = ReadContext.GetBroker<IWorkQueueEntityBroker>();

            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();

			criteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[] { WorkQueueTypeEnum.DeleteStudy, WorkQueueTypeEnum.MigrateStudy, WorkQueueTypeEnum.PurgeStudy });

            // Do Pending status, in case there's a Failure status entry, we don't want to 
            // block on that.
			criteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Pending, WorkQueueStatusEnum.InProgress });

            FilesystemStudyStorageSelectCriteria filesystemCriteria = new FilesystemStudyStorageSelectCriteria();

			filesystemCriteria.FilesystemKey.EqualTo(item.FilesystemKey);

			criteria.FilesystemStudyStorageRelatedEntityCondition.Exists(filesystemCriteria);
            int count = select.Count(criteria);

            return count;
        }

		private void MigrateStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
		{
			ServerFilesystemInfo newFS = _monitor.GetLowerTierFilesystemForStorage(fs);
			if (newFS == null)
			{
				Platform.Log(LogLevel.Warn,
				             "No writable storage in lower tiers. Tier-migration for '{0}' is disabled at this time.",
				             fs.Filesystem.Description);
				return;
			}

			Platform.Log(LogLevel.Info, "Starting Tier Migration from {0}", fs.Filesystem.Description);

			try
			{
				DoStudyMigrate(fs, item);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when scheduling tier-migration.");
			}

			Platform.Log(LogLevel.Info, "{0} studies have been scheduled for migration from filesystem '{1}'",
			             _studiesMigrated, fs.Filesystem.Description);
		}

    	private void DeleteStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
		{
			Platform.Log(LogLevel.Info, "Starting query for Filesystem delete candidates on '{0}'.",
			             fs.Filesystem.Description);
			try
			{
				DoStudyDelete(fs, item);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing StudyDelete records.");
			}

			Platform.Log(LogLevel.Info, "{0} studies have been scheduled for removal from filesystem '{1}'", 
				_studiesDeleted, fs.Filesystem.Description);
		}

		private void PurgeStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
		{
			Platform.Log(LogLevel.Info, "Starting query for Filesystem Purge candidates on '{0}'.",
						 fs.Filesystem.Description);
			try
			{
				DoStudyPurge(fs, item);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing StudyDelete records.");
			}

			Platform.Log(LogLevel.Info, "{0} studies have been scheduled for purging from filesystem '{1}'", 
				_studiesPurged, fs.Filesystem.Description);
		}
    	#endregion

        #region Public Methods

        
		/// <summary>
		/// Main <see cref="ServiceLock"/> processing routine.
		/// </summary>
		/// <param name="item">The <see cref="ServiceLock"/> item to process.</param>
        public void Process(Model.ServiceLock item)
        {
            _monitor = new FilesystemMonitor("Filesystem Management");
            _monitor.Load();

			ServiceLockSettings settings = ServiceLockSettings.Default;

            ServerFilesystemInfo fs = _monitor.GetFilesystemInfo(item.FilesystemKey);
            
            InitializeScheduleTime();

            _bytesToRemove = _monitor.CheckFilesystemBytesToRemove(item.FilesystemKey);

			DateTime scheduledTime;

			if (fs.AboveHighWatermark)
			{
				int count = CheckWorkQueueCount(item);
				if (count > 0)
				{
					Platform.Log(LogLevel.Info,
								 "Delaying Filesystem ServiceLock check, {0} StudyDelete, StudyPurge or MigrateStudy items still in the WorkQueue for Filesystem: {1} (Current: {2}, High Watermark: {3})",
								 count, fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);

					scheduledTime = Platform.Time.AddMinutes(settings.FilesystemDeleteRecheckDelay);
				}
				else
				{
					Platform.Log(LogLevel.Info, "Filesystem above high watermark: {0} (Current: {1}, High Watermark: {2}",
								 fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);

					MigrateStudies(item, fs);

					if (_bytesToRemove > 0)
						DeleteStudies(item, fs);

					if (_bytesToRemove > 0)
						PurgeStudies(item, fs);

					scheduledTime = Platform.Time.AddMinutes(settings.FilesystemDeleteRecheckDelay);
				}

                OnAboveHighWatermark(fs);
			}
			else
			{
				Platform.Log(LogLevel.Info, "Filesystem below watermarks: {0} (Current: {1}, High Watermark: {2}",
				             fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);
				scheduledTime = Platform.Time.AddMinutes(settings.FilesystemDeleteCheckInterval);

			    OnBelowLowWatermark(fs);
			}

			UnlockServiceLock(item, true, scheduledTime);            

            _monitor.Dispose();
            _monitor = null;

        }



        private void OnAboveHighWatermark(ServerFilesystemInfo fs)
        {
            if (!_fsTracker.IsTracking(fs))
            {
                _fsTracker.StartTracking(fs);
            }

            TimeSpan duration = _fsTracker.GetTrackingDuration(fs);
            if (duration > TimeSpan.FromMinutes(ServiceLockSettings.Default.HighWatermarkAlertInterval))
            {
                ServerPlatform.Alert(AlertCategory.System, AlertLevel.Warning, SR.AlertNameFilesystemDelete, AlertTypeCodes.LowResources,
                                            SR.AlertFilesystemAboveHW,  fs.Filesystem.Description, TimeSpanFormatter.Format(duration));
            }
        }

        private void OnBelowLowWatermark(ServerFilesystemInfo fs)
        {
            if (_fsTracker.IsTracking(fs))
            {
                _fsTracker.StopTracking(fs);
            }
        }


        public new void Dispose()
        {
            if (_monitor != null)
            {
                _monitor.Dispose();
                _monitor = null;
            }

            base.Dispose();
        }
        #endregion
    }
}
