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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Engine for acquiring WorkQueue items and finding plugins to process them.
    /// </summary>
    public class WorkQueueProcessor
    {
        #region Members
        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private readonly Dictionary<WorkQueueTypeEnum, IWorkQueueProcessorFactory> _extensions = new Dictionary<WorkQueueTypeEnum, IWorkQueueProcessorFactory>();
		private readonly ItemProcessingThreadPool<Model.WorkQueue> _threadPool;
        private readonly ManualResetEvent _threadStop;
        private bool _stop = false;

    	private readonly List<WorkQueueTypeEnum> _supportedTypesList = new List<WorkQueueTypeEnum>();
    	private readonly List<WorkQueueTypeEnum> _unsupportedTypesList = new List<WorkQueueTypeEnum>();
        #endregion

        #region Constructor
        public WorkQueueProcessor(int numberThreads, ManualResetEvent threadStop, string name)
        {
        	_threadStop = threadStop;
			_threadPool = new ItemProcessingThreadPool<Model.WorkQueue>(numberThreads);
        	_threadPool.ThreadPoolName = name + " Pool";

            WorkQueueFactoryExtensionPoint ep = new WorkQueueFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the workqueue processor. 
                Platform.Log(LogLevel.Warn, "No WorkQueueFactory Extension found.");
            }
            else
            {
                foreach (object obj in factories)
                {
                    IWorkQueueProcessorFactory factory = obj as IWorkQueueProcessorFactory;
                    if (factory != null)
                    {
                        WorkQueueTypeEnum type = factory.GetWorkQueueType();
                        _extensions.Add(type, factory);
                    }
                    else
                        Platform.Log(LogLevel.Error, "Unexpected incorrect type loaded for extension: {0}",
                                     obj.GetType());
                }
            }
	    }
		#endregion

		#region Public Properties

		public List<WorkQueueTypeEnum> SupportedTypesList
    	{
    		get { return _supportedTypesList; }
    	}

    	public List<WorkQueueTypeEnum> UnsupportedTypesList
    	{
    		get { return _unsupportedTypesList; }
    	}

    	#endregion

        #region Methods
		public string GetWorkQueueTypeString()
		{
			string typeString = String.Empty;
			if (_supportedTypesList.Count > 0)
			{
				foreach (WorkQueueTypeEnum type in _supportedTypesList)
				{
					if (typeString.Length > 0)
						typeString += "," + type.Enum;
					else
						typeString = type.Enum.ToString();
				}
			}

			if (_unsupportedTypesList.Count > 0)
			{
				foreach (WorkQueueTypeEnum defaultType in WorkQueueTypeEnum.GetAll())
				{
					bool found = false;
					foreach (WorkQueueTypeEnum unsupportedType in _unsupportedTypesList)
					{
						if (unsupportedType.Equals(defaultType))
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						if (typeString.Length > 0)
							typeString += "," + defaultType.Enum;
						else
							typeString = defaultType.Enum.ToString();						
					}
				}				
			}
			return typeString;
		}

    	/// <summary>
        /// Stop the WorkQueue processor
        /// </summary>
		public void Stop()
        {
        	_stop = true;
        	if (_threadPool.Active)
        		_threadPool.Stop();
        }

    	/// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
		/// <param name="failureDescription">The reason for the failure.</param>
        public void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters();
                parms.ProcessorID = ServiceTools.ProcessorId;

                parms.WorkQueueKey = item.GetKey();
                parms.StudyStorageKey = item.StudyStorageKey;
                parms.FailureCount = item.FailureCount + 1;
				parms.FailureDescription = failureDescription;

				WorkQueueSettings settings = WorkQueueSettings.Instance;
                if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
                {
                    Platform.Log(LogLevel.Error,
                                 "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}",
                                 item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
                    parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                    parms.ScheduledTime = Platform.Time;
                    parms.ExpirationTime = Platform.Time.AddDays(1);
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

        /// <summary>
        /// The processing thread.
        /// </summary>
        /// <remarks>
        /// This method queries the database for WorkQueue entries to work on, and then uses
        /// a thread pool to process the entries.
        /// </remarks>
        public void Run()
        {
			if (!_threadPool.Active)
				_threadPool.Start();

        	string workQueueTypes = GetWorkQueueTypeString();

            while (true)
            {
				if ((_threadPool.QueueCount + _threadPool.ActiveCount) < _threadPool.Concurrency)
                {
                    try
                    {
                        Model.WorkQueue queueListItem;

                        using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                        {
                            IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
                            WorkQueueQueryParameters parms = new WorkQueueQueryParameters();
                            parms.ProcessorID = ServiceTools.ProcessorId;
							if (workQueueTypes.Length > 0)
								parms.WorkQueueTypeEnumList = workQueueTypes;

							queueListItem = select.FindOne(parms);

							if (queueListItem != null)
                                updateContext.Commit();
                        }

						if (queueListItem == null)
						{
							/* No result found */
							_threadStop.WaitOne(WorkQueueSettings.Instance.WorkQueueQueryDelay, false);
							_threadStop.Reset();
						}
						else
						{
							if (!_extensions.ContainsKey(queueListItem.WorkQueueTypeEnum))
							{
								Platform.Log(LogLevel.Error,
											 "No extensions loaded for WorkQueue item type: {0}.  Failing item.",
											 queueListItem.WorkQueueTypeEnum);

								//Just fail the WorkQueue item, not much else we can do
								FailQueueItem(queueListItem, "No plugin to handle WorkQueue type: " + queueListItem.WorkQueueTypeEnum);
								continue;
							}

							IWorkQueueProcessorFactory factory = _extensions[queueListItem.WorkQueueTypeEnum];

							IWorkQueueItemProcessor processor;
							try
							{
								processor = factory.GetItemProcessor();
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Error, e, "Unexpected exception creating WorkQueue processor.");
								FailQueueItem(queueListItem, "Failure getting WorkQueue processor: " + e.Message);
								continue;
							}

							// Enqueue the actual processing of the item to the 
							// thread pool.  
							_threadPool.Enqueue(queueListItem, delegate(Model.WorkQueue queueItem)
													{
														try
														{
															processor.Process(queueItem);
														}
														catch (Exception e)
														{
															Platform.Log(LogLevel.Error, e,
																		 "Unexpected exception when processing WorkQueue item of type {0}.  Failing Queue item. (GUID: {1})",
																		 queueItem.WorkQueueTypeEnum,
																		 queueItem.GetKey());
															if (e.InnerException != null)
																FailQueueItem(queueItem, e.InnerException.Message);
															else
																FailQueueItem(queueItem, e.Message);



															ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error,
																				processor.Name, AlertTypeCodes.UnableToProcess,
																				"Work Queue item failed: Type={0}, GUID={1}",
																				 queueItem.WorkQueueTypeEnum,
																				 queueItem.GetKey());
														}

														// Cleanup the processor
														processor.Dispose();
													});

						}
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception when processing WorkQueue.");
                    }
                }
                else
                {
                    // Wait for only 1.5 seconds when the thread pool is all in use.
                    _threadStop.WaitOne(1500, false);
                    _threadStop.Reset();
                }
                if (_stop)
                    return;
            }
        }
        #endregion
    }
}
