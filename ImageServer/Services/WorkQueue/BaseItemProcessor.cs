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
using System.Diagnostics;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Base class used when implementing WorkQueue item processors.
    /// </summary>
    public abstract class BaseItemProcessor : IWorkQueueItemProcessor
    {
        private string _name = "Work Queue";
        private IReadContext _readContext;

        //private WorkQueueProcessorStatistics _statistics;
        private TimeSpanStatistics _storageLocationLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _uidsLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _dBUpdateTime = new TimeSpanStatistics();
        private TimeSpanStatistics _studyXmlLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _processTime = new TimeSpanStatistics();

        protected IList<StudyStorageLocation> _storageLocationList;
        private IList<WorkQueueUid> _uidList;

        #region Protected Properties

        protected IReadContext ReadContext
        {
            get { return _readContext; }
        }

        protected StudyStorageLocation StorageLocation
        {
            get { return _storageLocationList[0]; }
        }

        protected IList<StudyStorageLocation> StorageLocationList
        {
            get { return _storageLocationList; }
        }

        protected IList<WorkQueueUid> WorkQueueUidList
        {
            get { return _uidList; }
        }

        protected TimeSpanStatistics StorageLocationLoadTime
        {
            get { return _storageLocationLoadTime; }
            set { _storageLocationLoadTime = value; }
        }

        protected TimeSpanStatistics UidsLoadTime
        {
            get { return _uidsLoadTime; }
            set { _uidsLoadTime = value; }
        }

        protected TimeSpanStatistics DBUpdateTime
        {
            get { return _dBUpdateTime; }
            set { _dBUpdateTime = value; }
        }

        protected TimeSpanStatistics StudyXmlLoadTime
        {
            get { return _studyXmlLoadTime; }
            set { _studyXmlLoadTime = value; }
        }

        protected TimeSpanStatistics ProcessTime
        {
            get { return _processTime; }
            set { _processTime = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region Contructors

        protected BaseItemProcessor()
        {
            _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Load the storage location for the WorkQueue item.
        /// </summary>
        /// <param name="item">The item to load the location for.</param>
        protected IList<StudyStorageLocation> LoadStorageLocation(Model.WorkQueue item)
        {
            IList<StudyStorageLocation> list = null;
            StorageLocationLoadTime.Add(
                delegate
                    {
                        IQueryStudyStorageLocation select = _readContext.GetBroker<IQueryStudyStorageLocation>();

                        StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
                        parms.StudyStorageKey = item.StudyStorageKey;

                        list = select.Execute(parms);

                        if (list.Count == 0)
                        {
                            Platform.Log(LogLevel.Error, "Unable to find storage location for WorkQueue item: {0}",
                                         item.GetKey().ToString());
                            throw new ApplicationException("Unable to find storage location for WorkQueue item.");
                        }
                    }
                );

            
            Debug.Assert(list != null && list.Count> 0);
            return list;
        }

        /// <summary>
        /// Load the specific SOP Instance Uids in the database for the WorkQueue item.
        /// </summary>
        /// <param name="item">The WorkQueue item.</param>
        protected void LoadUids(Model.WorkQueue item)
        {
            UidsLoadTime.Add(
                delegate
                    {
						IWorkQueueUidEntityBroker select = _readContext.GetBroker<IWorkQueueUidEntityBroker>();

						WorkQueueUidSelectCriteria parms = new WorkQueueUidSelectCriteria();

                        parms.WorkQueueKey.EqualTo(item.GetKey());
                        _uidList = select.Find(parms);

                        _uidList = TruncateList(item, _uidList);
                    }
                );
            
        }

        /// <summary>
        /// Returns the max batch size for a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item to be processed</param>
        /// <returns>The maximum batch size for the <see cref="WorkQueue"/> item</returns>
        /// <param name="listCount">The number of available list items.</param>
        protected static int GetMaxBatchSize(Model.WorkQueue item, int listCount)
        {
            int maxSize;

            WorkQueueSettings settings = WorkQueueSettings.Default;

            if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.Low)
            {
                maxSize = settings.LowPriorityMaxBatchSize;
            }
            else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.Medium)
            {
                maxSize = settings.MedPriorityMaxBatchSize;
            }
            else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.High)
            {
                maxSize = listCount;
            }
            else
            {
                maxSize = settings.MedPriorityMaxBatchSize;
            }

            return maxSize;
        }

        /// <summary>
        /// Truncate the SOP Instance Uid list
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item to be processed</param>
        /// <param name="list">The list of <see cref="WorkQueueUid"/> to be truncated, if needed</param>
        /// <return>A truncated list of <see cref="WorkQueueUid"/></return>
        protected static IList<WorkQueueUid> TruncateList(Model.WorkQueue item, IList<WorkQueueUid> list)
        {
			if (item != null && list != null)
			{
				int maxSize = GetMaxBatchSize(item, list.Count);
				if (list.Count <= maxSize)
					return list;

				List<WorkQueueUid> newList = new List<WorkQueueUid>();
				foreach (WorkQueueUid uid in list)
				{
					if (!uid.Failed)
						newList.Add(uid);

					if (newList.Count >= maxSize)
						return newList;
				}

				// just return the whole list, they're all going to be skipped anyways!
				if (newList.Count == 0)
					return list;

				return newList;
			}

			return list;
        }

        /// <summary>
        /// Updates the status of a study to a new status
        /// </summary>
		protected virtual void UpdateStudyStatus(StudyStorageLocation theStudyStorage, StudyStatusEnum theStatus, TransferSyntax theSyntax)
        {
        	DBUpdateTime.Add(
        		delegate
        			{
        				using (
        					IUpdateContext updateContext =
        						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
        				{
							// Select the Server Transfer Syntax
							ServerTransferSyntaxSelectCriteria syntaxCriteria = new ServerTransferSyntaxSelectCriteria();
							IServerTransferSyntaxEntityBroker syntaxBroker =
								updateContext.GetBroker<IServerTransferSyntaxEntityBroker>();
							syntaxCriteria.Uid.EqualTo(theSyntax.UidString);

							IList<ServerTransferSyntax> syntaxList = syntaxBroker.Find(syntaxCriteria);
							if (syntaxList.Count == 0)
							{
								Platform.Log(LogLevel.Error, "Unable to load ServerTransferSyntax for {0}.  Unable to update study status.", theSyntax.Name);
								return;
							}

        					ServerTransferSyntax serverSyntax = syntaxList[0];

							// Get the FilesystemStudyStorage update broker ready
        					IFilesystemStudyStorageEntityBroker filesystemQueueBroker =
								updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
							FilesystemStudyStorageUpdateColumns filesystemQueueUpdate = new FilesystemStudyStorageUpdateColumns();
        					filesystemQueueUpdate.ServerTransferSyntaxKey = serverSyntax.GetKey();
							FilesystemStudyStorageSelectCriteria filesystemQueueCriteria = new FilesystemStudyStorageSelectCriteria();
        					filesystemQueueCriteria.StudyStorageKey.EqualTo(theStudyStorage.GetKey());

							// Get the StudyStorage update broker ready
        					IStudyStorageEntityBroker studyStorageBroker =
        						updateContext.GetBroker<IStudyStorageEntityBroker>();
							StudyStorageUpdateColumns studyStorageUpdate = new StudyStorageUpdateColumns();
        					studyStorageUpdate.StudyStatusEnum = theStatus;
        					studyStorageUpdate.LastAccessedTime = Platform.Time;

							// Setup the Study Status Update
        					IStudyEntityBroker broker = updateContext.GetBroker<IStudyEntityBroker>();

							StudySelectCriteria criteria = new StudySelectCriteria();
							criteria.ServerPartitionKey.EqualTo(theStudyStorage.ServerPartitionKey);
							criteria.StudyInstanceUid.EqualTo(theStudyStorage.StudyInstanceUid);

							StudyUpdateColumns updateColumns = new StudyUpdateColumns();
        					updateColumns.StudyStatusEnum = theStatus;


							if (!broker.Update(criteria, updateColumns))
							{
								Platform.Log(LogLevel.Error, "Unable to update Study row: Study {0}, Server Entity {1}",
											 theStudyStorage.StudyInstanceUid, theStudyStorage.ServerPartitionKey);
							}
							else if (!filesystemQueueBroker.Update(filesystemQueueCriteria,filesystemQueueUpdate))
							{
								Platform.Log(LogLevel.Error, "Unable to update FilesystemQueue row: Study {0}, Server Entity {1}",
											 theStudyStorage.StudyInstanceUid, theStudyStorage.ServerPartitionKey);
								
							}
							else if (!studyStorageBroker.Update(theStudyStorage.GetKey(),studyStorageUpdate))
							{
								Platform.Log(LogLevel.Error, "Unable to update StudyStorage row: Study {0}, Server Entity {1}",
											 theStudyStorage.StudyInstanceUid, theStudyStorage.ServerPartitionKey);								
							}
							else
								updateContext.Commit();
        				}
        			}
        		);
        }

		/// <summary>
		/// Set a status of <see cref="WorkQueue"/> item after batch processing has been completed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine will set the status of the <paramref name="item"/> to one of the followings
		/// <list type="bullet">
		/// <item>Failed: if the current process failed and number of retries has been reached.</item>
		/// <item>Pending: if the current batch has been processed successfully</item>
		/// <item>Idle : if current batch size = 0.</item>
		/// <item>Completed: if batch size =0 (idle) and the item has expired.</item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <param name="item">The <see cref="WorkQueue"/> item to set.</param>
		/// <param name="complete">Indicates if complete.</param>
		/// <param name="processedBatch">Processed a batch request of uids</param>
		protected virtual void PostProcessing(Model.WorkQueue item, bool processedBatch, bool complete)
		{
			DBUpdateTime.Add(
				delegate
				{
					using (
						IUpdateContext updateContext =
							PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
					{
						IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
						WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
						parms.WorkQueueKey = item.GetKey();
						parms.StudyStorageKey = item.StudyStorageKey;
						parms.ProcessorID = item.ProcessorID;

						WorkQueueSettings settings = WorkQueueSettings.Default;


						DateTime scheduledTime;

						if (item.FailureDescription != null)
							parms.FailureDescription = item.FailureDescription;

						if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.Low)
						{
							scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayLowPrioritySeconds);
						}
						else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.High)
						{
							scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayHighPrioritySeconds);
						}
						else
						{
							scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayMedPrioritySeconds);
						}


						if (scheduledTime > item.ExpirationTime)
							scheduledTime = item.ExpirationTime;

						if (complete || !processedBatch && item.ExpirationTime < Platform.Time)
						{
							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Completed;
							parms.FailureCount = item.FailureCount;
							parms.ScheduledTime = scheduledTime;

							parms.ExpirationTime = item.ExpirationTime; // Keep the same
						}
						// If the batch size is 0, switch to idle state.
						else if (!processedBatch)
						{
							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Idle;
							parms.ScheduledTime = scheduledTime;
							parms.ExpirationTime = item.ExpirationTime; // keep the same
							parms.FailureCount = item.FailureCount;
						}
						else
						{
							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;

							parms.ExpirationTime = scheduledTime.AddSeconds(settings.WorkQueueExpireDelaySeconds);
							parms.ScheduledTime = scheduledTime;
							parms.FailureCount = item.FailureCount;
						}


						if (false == update.Execute(parms))
						{
							Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum, item.GetKey().ToString());
						}
						else
							updateContext.Commit();
					}
				}
				);
		}

    	/// <summary>
		/// Set a status of <see cref="WorkQueue"/> item after batch processing has been completed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine will set the status of the <paramref name="item"/> to one of the following
		/// <list type="bullet">
		/// <item>Failed: if the current process failed and number of retries has been reached or a fatal error.</item>
		/// <item>Pending: if the number of retries has not been reached</item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <param name="item">The <see cref="WorkQueue"/> item to set.</param>
		/// <param name="fatal">The failure is unrecoverable</param>
		protected virtual void PostProcessingFailure(Model.WorkQueue item, bool fatal)
		{
			DBUpdateTime.Add(
				delegate
					{
						using (
							IUpdateContext updateContext =
								PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
						{
							IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
							WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
							parms.WorkQueueKey = item.GetKey();
							parms.StudyStorageKey = item.StudyStorageKey;
							parms.ProcessorID = item.ProcessorID;

							WorkQueueSettings settings = WorkQueueSettings.Default;

							if (item.FailureDescription != null)
								parms.FailureDescription = item.FailureDescription;

							parms.FailureCount = item.FailureCount + 1;
							if (fatal)
							{
								Platform.Log(LogLevel.Error,
											 "Failing {0} WorkQueue entry ({1}), fatal error",
											 item.WorkQueueTypeEnum, item.GetKey());

								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
								parms.ScheduledTime = Platform.Time;
								parms.ExpirationTime = Platform.Time; // expire now		

							    ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, Name, AlertTypeCodes.UnableToProcess,
							                   "Failing {0} WorkQueue entry ({1}), fatal error",
							                   item.WorkQueueTypeEnum, item.GetKey());
							}
							else if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
							{
								Platform.Log(LogLevel.Error,
								             "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}",
											 item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
								parms.ScheduledTime = Platform.Time;
								parms.ExpirationTime = Platform.Time; // expire now


                                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error, Name, AlertTypeCodes.UnableToProcess,
							                   "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}",
							                   item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
							}
							else
							{
								Platform.Log(LogLevel.Error,
								             "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}",
								             item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
								parms.ScheduledTime = Platform.Time.AddMinutes(settings.WorkQueueFailureDelayMinutes);
								parms.ExpirationTime =
									Platform.Time.AddMinutes((settings.WorkQueueMaxFailureCount - item.FailureCount)*
									                         settings.WorkQueueFailureDelayMinutes);
							}


							if (false == update.Execute(parms))
							{
								Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}",
											 item.WorkQueueTypeEnum, item.GetKey().ToString());
							}
							else
								updateContext.Commit();
						}
					}
				);
		}



        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        /// <param name="failureDescription">The reason for the failure.</param>
        protected virtual void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            DBUpdateTime.Add(
                delegate
                    {
                        using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                        {
                            IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                            WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
                            parms.ProcessorID = ServiceTools.ProcessorId;

                            parms.WorkQueueKey = item.GetKey();
                            parms.StudyStorageKey = item.StudyStorageKey;
                            parms.FailureCount = item.FailureCount + 1;
                            parms.FailureDescription = failureDescription;

                            WorkQueueSettings settings = WorkQueueSettings.Default;
                            if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
                            {
                                Platform.Log(LogLevel.Error,
                                             "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}",
                                             item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
                                parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                                parms.ScheduledTime = Platform.Time;
                                parms.ExpirationTime = Platform.Time.AddDays(1);


                                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error, Name, AlertTypeCodes.UnableToProcess,
                                                "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}", 
                                                item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
                            }
                            else
                            {
                                Platform.Log(LogLevel.Error,
                                             "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}",
                                             item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
                                parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
                                parms.ScheduledTime = Platform.Time.AddMinutes(settings.WorkQueueFailureDelayMinutes);
                                parms.ExpirationTime =
                                    Platform.Time.AddMinutes((settings.WorkQueueMaxFailureCount - item.FailureCount) *
                                                             settings.WorkQueueFailureDelayMinutes);

                            }

                            if (false == update.Execute(parms))
                            {
                                Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum,
                                             item.GetKey().ToString());
                            }
                            else
                                updateContext.Commit();
                        }
                    }
                );

            
        }


    	/// <summary>
        /// Delete an entry in the <see cref="WorkQueueUid"/> table.
        /// </summary>
        /// <param name="sop">The <see cref="WorkQueueUid"/> entry to delete.</param>
        protected virtual void DeleteWorkQueueUid(WorkQueueUid sop)
        {
            DBUpdateTime.Add(
                TimeSpanStatisticsHelper.Measure(
                        delegate
                            {
                                using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                                {
                                    IWorkQueueUidEntityBroker delete = updateContext.GetBroker<IWorkQueueUidEntityBroker>();

                                    delete.Delete(sop.GetKey());

                                    updateContext.Commit();
                                }
                            }));
        }

        /// <summary>
        /// Update an entry in the <see cref="WorkQueueUid"/> table.
        /// </summary>
        /// <remarks>
        /// Note that just the Duplicate, Failed, FailureCount, and Extension columns are updated from the
        /// input parameter <paramref name="sop"/>.
        /// </remarks>
        /// <param name="sop">The <see cref="WorkQueueUid"/> entry to update.</param>
        protected virtual void UpdateWorkQueueUid(WorkQueueUid sop)
        {
            DBUpdateTime.Add(
                TimeSpanStatisticsHelper.Measure(
                delegate
                {
                     using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                     {
                         IWorkQueueUidEntityBroker update = updateContext.GetBroker<IWorkQueueUidEntityBroker>();

                         WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();

                         columns.Duplicate = sop.Duplicate;
                         columns.Failed = sop.Failed;
                         columns.FailureCount = sop.FailureCount;
                         if (sop.Extension != null)
                             columns.Extension = sop.Extension;

                         update.Update(sop.GetKey(), columns);

                         updateContext.Commit();
                     }
                }));

            
        }

        /// <summary>
        /// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="location">The location a study is stored.</param>
        /// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
        protected virtual StudyXml LoadStudyXml(StudyStorageLocation location)
        {
            StudyXml theXml = new StudyXml();

            StudyXmlLoadTime.Add(
                delegate
                    {
                        String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");
                        if (File.Exists(streamFile))
                        {
                            using (Stream fileStream = new FileStream(streamFile, FileMode.Open))
                            {
                                XmlDocument theDoc = new XmlDocument();

                                StudyXmlIo.Read(theDoc, fileStream);

                                theXml.SetMemento(theDoc);

                                fileStream.Close();
                            }
                        }
                    }
                );

           return theXml;
        }

        /// <summary>
        /// Called before the <see cref="WorkQueue"/> item is processed
        /// </summary>
        /// <param name="item">The work queue item to be processed.</param>
        protected virtual void OnProcessItemBegin(Model.WorkQueue item)
        {
            //NOOP
        }

        /// <summary>
        /// Called after the <see cref="WorkQueue"/> item has been processed
        /// </summary>
        /// <param name="item">The work queue item which has been processed.</param>
        protected virtual void OnProcessItemEnd(Model.WorkQueue item)
        {
            // NOOP
        }

        protected abstract void ProcessItem(Model.WorkQueue item);

        #endregion

        #region IWorkQueueItemProcessor Members

        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public void Dispose()
        {
            if (_readContext != null)
            {
                _readContext.Dispose();
                _readContext = null;
            }
        }

        #region IWorkQueueItemProcessor members

        public void Process(Model.WorkQueue item)
        {
            OnProcessItemBegin(item);
                        
            ProcessTime.Add(
                delegate
                    {
                        ProcessItem(item);
                    }
                );
            OnProcessItemEnd(item);
        }


        #endregion IWorkQueueItemProcessor members

        #endregion
    }
}
