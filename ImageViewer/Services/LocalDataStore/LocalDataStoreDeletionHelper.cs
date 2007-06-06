using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ServiceModel;
using ClearCanvas.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	public static class LocalDataStoreDeletionHelper
	{
		[Serializable]
		public class ConnectionLostException : CommunicationException
		{
			internal ConnectionLostException(string message)
				: base(message)
			{
			}

			protected ConnectionLostException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}

		public class DeletionProgressInformation
		{
			private TimeSpan _elapsedTime;
			private int _numberDeleted;
			private int _numberRemaining;
			private string _lastDeletedUid;
			private string _lastErrorMessage;

			internal DeletionProgressInformation()
			{ 
			}
			
			public TimeSpan ElapsedTime 
			{
				get { return _elapsedTime;  }
				internal set { _elapsedTime = value;  }
			}

			public int NumberDeleted
			{
				get { return _numberDeleted; }
				internal set { _numberDeleted = value; }
			}

			public int NumberRemaining
			{
				get { return _numberRemaining; }
				internal set { _numberRemaining = value; }
			}

			public string LastDeletedUid
			{
				get { return _lastDeletedUid; }
				internal set { _lastDeletedUid = value; }
			}

			public string LastErrorMessage
			{
				get { return _lastErrorMessage; }
				internal set { _lastErrorMessage = value; }
			}
		}

		public delegate bool DeletionProgressCallback(DeletionProgressInformation progressInformation);

		[CallbackBehavior(UseSynchronizationContext = false)]
		private class WaitDeleteInstancesCallback : ILocalDataStoreActivityMonitorServiceCallback
		{
			private object _waitLock = new object();
			private InstanceLevel _instanceLevel;
			private Dictionary<string, string> _waitDeleteInstanceUids;
			private int _startingNumber;
			
			private string _lastDeletedUid;
			private string _lastErrorMessage;

			private Exception _error;

			public WaitDeleteInstancesCallback(DeleteInstancesRequest request)
			{
				_instanceLevel = request.InstanceLevel;
				_waitDeleteInstanceUids = new Dictionary<string, string>();
				foreach(string instanceUid in request.InstanceUids)
					_waitDeleteInstanceUids[instanceUid] = instanceUid;

				_startingNumber = _waitDeleteInstanceUids.Count;
			}

			#region ILocalDataStoreActivityMonitorServiceCallback Members

			public void ReceiveProgressChanged(ReceiveProgressItem progressItem) { }
			public void SendProgressChanged(SendProgressItem progressItem) { }
			public void ImportProgressChanged(ImportProgressItem progressItem) { }
			public void ReindexProgressChanged(ReindexProgressItem progressItem) { }
			public void SopInstanceImported(ImportedSopInstanceInformation information) { }

			public void InstanceDeleted(DeletedInstanceInformation information)
			{
				if (information.InstanceLevel != _instanceLevel || String.IsNullOrEmpty(information.InstanceUid))
					return;

				lock (_waitLock)
				{ 
					_waitDeleteInstanceUids.Remove(information.InstanceUid);
					_lastDeletedUid = information.InstanceUid;
					_lastErrorMessage = information.ErrorMessage;

					Monitor.Pulse(_waitLock);
				}
			}

			#endregion

			public void OnLostConnection(object sender, EventArgs e)
			{
				lock (_waitLock)
				{
					_waitDeleteInstanceUids.Clear();
					_error = new ConnectionLostException(SR.ExceptionActivityMonitorConnectionLost);
					Monitor.Pulse(_waitLock);
				}
			}

			public void Wait(int callbackPeriod, DeletionProgressCallback progressCallback)
			{
				DateTime startTime = Platform.Time;

				DeletionProgressInformation information = new DeletionProgressInformation();
 
				lock (_waitLock)
				{
					while(_waitDeleteInstanceUids.Count > 0)
					{
						Monitor.Wait(_waitLock, callbackPeriod);
						
						if (_error != null)
							throw _error;

						information.ElapsedTime = Platform.Time - startTime;
						information.NumberRemaining = _waitDeleteInstanceUids.Count;
						information.NumberDeleted = _startingNumber - information.NumberRemaining;
						information.LastDeletedUid = _lastDeletedUid;
						information.LastErrorMessage = _lastErrorMessage;

						if (!progressCallback(information))
							break;
					}						
				}
			}
		}

		private static void SendDeleteRequest(DeleteInstancesRequest request)
		{
			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();

			try
			{
				client.Open();

				const int batchSize = 500;

				DeleteInstancesRequest batchRequest = new DeleteInstancesRequest();
				batchRequest.DeletePriority = request.DeletePriority;
				batchRequest.InstanceLevel = request.InstanceLevel;
				List<string> allUids = new List<string>(request.InstanceUids);
				List<string> batchUids = new List<string>();

				while (allUids.Count > 0)
				{
					batchUids.Add(allUids[0]);
					allUids.RemoveAt(0);

					if (batchUids.Count == batchSize || allUids.Count == 0)
					{
						batchRequest.InstanceUids = batchUids;
						client.DeleteInstances(batchRequest);
						batchUids.Clear();
					}
				}

				client.Close();
				
			}
			catch
			{
				client.Abort();
				throw;
			}
		}

		public static void DeleteInstancesNoWait(DeleteInstancesRequest request)
		{
			SendDeleteRequest(request);
		}

		public static void DeleteInstancesAndWait(DeleteInstancesRequest request, int callbackPeriod, DeletionProgressCallback progressCallback)
		{
			Platform.CheckPositive(callbackPeriod, "callbackPeriod");
			Platform.CheckForNullReference(progressCallback, "progressCallback");
 
			WaitDeleteInstancesCallback callback = new WaitDeleteInstancesCallback(request);
			LocalDataStoreActivityMonitorServiceClient monitor = new LocalDataStoreActivityMonitorServiceClient(new InstanceContext(callback));
			try
			{
				monitor.Open();
				monitor.Subscribe("InstanceDeleted");

				monitor.InnerChannel.Faulted += new EventHandler(callback.OnLostConnection);
				monitor.InnerChannel.Closed += new EventHandler(callback.OnLostConnection);
			}
			catch
			{
				monitor.Abort();
				throw;
			}

			try
			{
				SendDeleteRequest(request);
				callback.Wait(callbackPeriod, progressCallback);
			}
			catch
			{
				throw;
			}
			finally
			{
				try
				{
					monitor.InnerChannel.Faulted -= new EventHandler(callback.OnLostConnection);
					monitor.InnerChannel.Closed -= new EventHandler(callback.OnLostConnection);

					monitor.Unsubscribe("InstanceDeleted");
					monitor.Close();
				}
				catch
				{
					// The call to callback.Wait will throw a "Lost Connection" exception,
					// so we'll just ignore this because an exception is already being thrown.
					monitor.Abort();
				}
			}
		}
	}
}
