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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Common.Utilities;
using System.IO;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal delegate void SendOperationProgressCallback(ISendOperation operation);

	internal interface ISendOperation
	{
		SendOperationReference Reference { get; }
		bool Canceled { get; }

		int RemainingSubOperations { get; }
		int FailureSubOperations { get; }
		int SuccessSubOperations { get; }
		int WarningSubOperations { get; }

		ICollection<StorageInstance> StorageInstances { get; }
	}

	//TODO: Later, remove IDicomServerService and replace with something like this.
	internal interface ISendService
	{
		SendOperationReference SendStudies(SendStudiesRequest request, SendOperationProgressCallback callback);
		SendOperationReference SendSeries(SendSeriesRequest request, SendOperationProgressCallback callback);
		SendOperationReference SendSopInstances(SendSopInstancesRequest request, SendOperationProgressCallback callback);
		SendOperationReference SendFiles(SendFilesRequest request, SendOperationProgressCallback callback);

		void Cancel(SendOperationReference sendOperation);
	}

	internal class DicomSendManager : ISendService
	{
		public static readonly DicomSendManager Instance = new DicomSendManager();

		#region Private Fields

		private readonly object _syncLock = new object();
		private readonly List<SendScu> _scus = new List<SendScu>();
		private bool _active;

		#endregion

		private DicomSendManager()
		{
			_active = false;
		}

		#region SendScu class

		private class SendScu : StorageScu, ISendOperation
		{
			#region Private Fields

			private SendOperationReference _reference;
			private Dictionary<string, SendStudyInformation> _studies;
			private Thread _thread;
			private SendFilesRequest _sendFilesRequest;
			private SendOperationProgressCallback _callback;

			#endregion

			public SendScu(string localAETitle, SendFilesRequest sendFilesRequest, SendOperationProgressCallback callback)
				: base(localAETitle, sendFilesRequest.DestinationAEInformation.AETitle, 
									sendFilesRequest.DestinationAEInformation.HostName, 
									sendFilesRequest.DestinationAEInformation.Port)
			{
				Platform.CheckForEmptyString(localAETitle, "localAETitle");

				_sendFilesRequest = sendFilesRequest;
				Initialize(callback);
			}

			public SendScu(string localAETitle, AEInformation destinationAEInfo, IEnumerable<ISopInstance> instancesToSend, SendOperationProgressCallback callback)
				: base(localAETitle, destinationAEInfo.AETitle, destinationAEInfo.HostName, destinationAEInfo.Port)
			{
				Platform.CheckForEmptyString(localAETitle, "localAETitle");
				Platform.CheckForEmptyString(destinationAEInfo.AETitle, "destinationAEInfo.AETitle");
				Platform.CheckForEmptyString(destinationAEInfo.HostName, "destinationAEInfo.HostName");
				Platform.CheckForNullReference(instancesToSend, "instancesToSend");

				Initialize(callback);
				AddInstances(instancesToSend);
			}

			private void Initialize(SendOperationProgressCallback callback)
			{
				_studies = new Dictionary<string, SendStudyInformation>();
				_reference = new SendOperationReference(Guid.NewGuid());

				_thread = new Thread(DoSend);
				_thread.Name = String.Format("Send to {0}/{1}:{2}", base.RemoteHost, base.RemoteAE, base.RemotePort);

				_callback = callback;
			}

			private void AddInstances(IEnumerable<ISopInstance> instancesToSend)
			{
				foreach (ISopInstance sop in instancesToSend)
				{
					AddStudy(sop.GetParentSeries().GetParentStudy());
					AddStorageInstance(new StorageInstance(sop.GetLocationUri().LocalDiskPath));
				}
			}

			private void AddStudyInformation(DicomFile file)
			{
				string studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid];
				if (_studies.ContainsKey(studyInstanceUid))
					return;

				SendStudyInformation info = new SendStudyInformation();
				info.SendOperationReference = Reference;
				info.ToAETitle = RemoteAE;

				info.StudyInformation = new StudyInformation();
				info.StudyInformation.PatientId = file.DataSet[DicomTags.PatientId].GetString(0, "");
				info.StudyInformation.PatientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");

				string studyDate = file.DataSet[DicomTags.StudyDate].GetString(0, "");

				info.StudyInformation.StudyDate = DateParser.Parse(studyDate);
				info.StudyInformation.StudyDescription = file.DataSet[DicomTags.StudyDescription].GetString(0, ""); ;
				info.StudyInformation.StudyInstanceUid = studyInstanceUid;

				_studies[studyInstanceUid] = info;
			}

			private void AddStudy(IStudy study)
			{
				if (_studies.ContainsKey(study.StudyInstanceUid))
					return;
				
				SendStudyInformation info = new SendStudyInformation();
				info.SendOperationReference = Reference;
				info.ToAETitle = RemoteAE;

				info.StudyInformation = new StudyInformation();
				info.StudyInformation.PatientId = study.PatientId;
				info.StudyInformation.PatientsName = study.PatientsName;
				info.StudyInformation.StudyDate = DateParser.Parse(study.StudyDate);
				info.StudyInformation.StudyDescription = study.StudyDescription;
				info.StudyInformation.StudyInstanceUid = study.StudyInstanceUid;

				_studies[study.StudyInstanceUid] = info;
			}

			private void DoSend()
			{
				try
				{
					SendInternal();
					AuditSendOperation(true);
				}
				catch (Exception e)
				{
					if (base.Status == ScuOperationStatus.ConnectFailed)
					{
						OnSendError(String.Format("Unable to connect to remote server ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else
					{
						OnSendError(String.Format("An unexpected error occurred while processing the Store operation ({0}).", e.Message));
					}

					AuditSendOperation(false);
				}

				Instance.OnSendComplete(this);
			}

			private void AuditSendOperation(bool noExceptions)
			{
				if (noExceptions)
				{
					AuditedInstances sentInstances = new AuditedInstances();
					AuditedInstances failedInstances = new AuditedInstances();
					foreach (StorageInstance instance in this.StorageInstances)
					{
						if (instance.SendStatus.Status == DicomState.Success)
							sentInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
						else
							failedInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
					}
					AuditHelper.LogSentInstances(this.RemoteAE, this.RemoteHost, sentInstances, EventSource.CurrentProcess, EventResult.Success);
					AuditHelper.LogSentInstances(this.RemoteAE, this.RemoteHost, failedInstances, EventSource.CurrentProcess, EventResult.MinorFailure);
				}
				else
				{
					AuditedInstances sentInstances = new AuditedInstances();
					foreach (StorageInstance instance in this.StorageInstances)
						sentInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
					AuditHelper.LogSentInstances(this.RemoteAE, this.RemoteHost, sentInstances, EventSource.CurrentProcess, EventResult.MajorFailure);
				}
			}

			private void SendInternal()
			{
					OnBeginSend();

					base.Send();

					Join(new TimeSpan(0, 0, 0, 0, 1000));

					if (base.Status == ScuOperationStatus.Canceled)
					{
						OnSendError(String.Format("The Store operation has been cancelled ({0}).", RemoteAE));
					}
					else if (base.Status == ScuOperationStatus.ConnectFailed)
					{
						OnSendError(String.Format("Unable to connect to remote server ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else if (base.Status == ScuOperationStatus.AssociationRejected)
					{
						OnSendError(String.Format("Association rejected ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else if (base.Status == ScuOperationStatus.Failed)
					{
						OnSendError(String.Format("The Store operation failed ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else if (base.Status == ScuOperationStatus.TimeoutExpired)
					{
						OnSendError(String.Format("The connection timeout has expired ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else if (base.Status == ScuOperationStatus.UnexpectedMessage)
					{
						OnSendError("Unexpected message received; aborted association.");
					}
					else if (base.Status == ScuOperationStatus.NetworkError)
					{
						OnSendError("An unexpected network error has occurred.");
					}
			}

			private void OnFileSent(StorageInstance storageInstance)
			{
				StoreScuSentFileInformation info = new StoreScuSentFileInformation();
				
				StudyInformationFieldExchanger exchanger = new StudyInformationFieldExchanger();
				DicomFile file = storageInstance.LoadFile();
				file.DataSet.LoadDicomFields(exchanger);
				info.StudyInformation = exchanger;

				info.ToAETitle = RemoteAE;
				info.FileName = storageInstance.Filename;
				info.SendOperationReference = Reference;
				
				LocalDataStoreEventPublisher.Instance.FileSent(info);
			}

			protected override void OnImageStoreCompleted(StorageInstance storageInstance)
			{
				base.OnImageStoreCompleted(storageInstance);

				if (storageInstance.SendStatus.Status == DicomState.Success)
				{
					OnFileSent(storageInstance);
				}
				else if (storageInstance.SendStatus.Status != DicomState.Pending)
				{
					string severity = "Error";
					if (storageInstance.SendStatus.Status == DicomState.Warning)
					{
						severity = "Warning";
						OnFileSent(storageInstance);
					}

					string description = storageInstance.ExtendedFailureDescription;
					if (String.IsNullOrEmpty(description))
						description = storageInstance.SendStatus.ToString();

					string msg = String.Format("{0} encountered while sending file {1} ({2}: {3}).",
						severity, storageInstance.Filename, RemoteAE, description);

					Platform.Log(LogLevel.Error, msg);

					OnSendError(String.Format("{0} encountered while sending file ({1}: {2}).",
											  severity, RemoteAE, description));
				}

				if (_callback != null)
				{
					try
					{
						_callback(this);
					}
					catch(Exception e)
					{
						Platform.Log(LogLevel.Error, e, "Unexpected error thrown from SendScu callback.");
					}
				}

				if (_sendFilesRequest != null && _sendFilesRequest.DeletionBehaviour != DeletionBehaviour.None)
				{
					bool deleteFile = false;
					if (storageInstance.SendStatus.Status != DicomState.Failure)
						deleteFile = true;
					else if (_sendFilesRequest.DeletionBehaviour == DeletionBehaviour.DeleteAlways)
						deleteFile = true;

					if (deleteFile)
					{
						try
						{
							File.Delete(storageInstance.Filename);
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Warn, e, "Failed to delete file after storage: {0}", storageInstance.Filename);
						}
					}
				}
			}

			private void OnBeginSend()
			{
				if (_sendFilesRequest != null)
				{
					List<string> filesToSend = GetFilesToSend();
					if (filesToSend.Count == 0)
						throw new Exception("No valid files were found to send.");

					foreach (string file in filesToSend)
					{
						StorageInstance instance = new StorageInstance(file);
						DicomFile dicomFile = instance.LoadFile();
						AddStudyInformation(dicomFile);
						AddStorageInstance(instance);
					}
				}

				//later, we could queue it up to limit the number of active scus.
				foreach (SendStudyInformation info in _studies.Values)
					LocalDataStoreEventPublisher.Instance.SendStarted(info);
			}

			private List<string> GetFilesToSend()
			{
				List<string> extensions = new List<string>();
				if (_sendFilesRequest.FileExtensions != null)
				{
					foreach (string extension in _sendFilesRequest.FileExtensions)
					{
						if (!extension.StartsWith("."))
							extensions.Add("." + extension);
						else
							extensions.Add(extension);
					}
				}

				List<string> files = new List<string>();
				foreach (string path in _sendFilesRequest.FilePaths)
				{
					DirectoryInfo info = new DirectoryInfo(path);
					if (info.Exists)
					{
						SearchOption searchOption = SearchOption.TopDirectoryOnly;
						if (_sendFilesRequest.Recursive)
							searchOption = SearchOption.AllDirectories;
						
						FileInfo[] fileInfos = info.GetFiles("*.*", searchOption);
						if (extensions.Count > 0)
						{
							foreach (FileInfo fileInfo in fileInfos)
							{
								foreach (string extension in extensions)
								{
									if (0 == string.Compare(extension, fileInfo.Extension, true))
										files.Add(fileInfo.FullName);
								}
							}
						}
						else
						{
							foreach (FileInfo fileInfo in fileInfos)
								files.Add(fileInfo.FullName);
						}
					}
					else
					{
						FileInfo fileInfo = new FileInfo(path);
						if (fileInfo.Exists)
							files.Add(fileInfo.FullName);
					}
				}

				return files;
			}

			private void OnSendError(string message)
			{
				if (_studies.Count == 0)
				{
					SendErrorInformation dummyError = new SendErrorInformation();
					dummyError.SendOperationReference = Reference;
					dummyError.ToAETitle = RemoteAE;
					dummyError.StudyInformation = new StudyInformation();
					dummyError.ErrorMessage = message;
					LocalDataStoreEventPublisher.Instance.SendError(dummyError);
				}
				else
				{
					foreach (SendStudyInformation info in _studies.Values)
					{
						SendErrorInformation error = new SendErrorInformation();
						error.SendOperationReference = Reference;
						error.ToAETitle = info.ToAETitle;
						error.StudyInformation = info.StudyInformation;
						error.ErrorMessage = message;
						LocalDataStoreEventPublisher.Instance.SendError(error);
					}
				}
			}

			#region IStorageScu Members

			public SendOperationReference Reference
			{
				get { return _reference; }	
			}

			public ICollection<StorageInstance> StorageInstances
			{
				get { return StorageInstanceList; }
			}

			#endregion

			public new void Send()
			{
				//do this rather than use BeginSend b/c it uses thread pool threads which can be exhausted.
				_thread.Start();
			}

			public void Cancel(bool wait)
			{
				Cancel();
				if (wait)
					_thread.Join();
			}
		}

		#endregion

		#region Private Methods

		private SendOperationReference Send(SendFilesRequest request, SendOperationProgressCallback callback)
		{
			lock (_syncLock)
			{
				if (!_active)
					throw new InvalidOperationException("The Dicom Send service is not active.");

				DicomServerConfiguration configuration = DicomServerManager.Instance.GetServerConfiguration();
				SendScu scu = new SendScu(configuration.AETitle, request, callback);
				scu.Reference.IsBackground = request.IsBackground;
				_scus.Add(scu);
				scu.Send();
				return scu.Reference;
			}
		}

		private SendOperationReference Send(AEInformation destinationAEInformation, bool isBackground, IEnumerable<ISopInstance> instancesToSend, SendOperationProgressCallback callback)
		{
			lock (_syncLock)
			{
				if (!_active)
					throw new InvalidOperationException("The Dicom Send service is not active.");

				DicomServerConfiguration configuration = DicomServerManager.Instance.GetServerConfiguration();
				SendScu scu = new SendScu(configuration.AETitle, destinationAEInformation, instancesToSend, callback);
				scu.Reference.IsBackground = isBackground;
				_scus.Add(scu);
				scu.Send();

				return scu.Reference;
			}
		}

		private void OnSendComplete(SendScu sendScu)
		{
			lock (_syncLock)
			{
				_scus.Remove(sendScu);
				sendScu.Dispose();
			}
		}

		#endregion

		#region public Methods

		public void Start()
		{
			lock(_syncLock)
			{
				_active = true;
			}
		}

		public void Stop()
		{
			List<SendScu> scus;
			lock (_syncLock)
			{
				_active = false;
				scus = new List<SendScu>(_scus);
			}

			scus.ForEach(delegate(SendScu scu) { scu.Cancel(true); });
		}

		#endregion

		#region ISendService Members

		public SendOperationReference SendStudies(SendStudiesRequest request, SendOperationProgressCallback callback)
		{
			return Send(request.DestinationAEInformation, request.IsBackground,
				DataStoreQueryHelper.GetStudySopInstances(request.StudyInstanceUids), callback);
		}

		public SendOperationReference SendSeries(SendSeriesRequest request, SendOperationProgressCallback callback)
		{
			return Send(request.DestinationAEInformation, request.IsBackground,
				DataStoreQueryHelper.GetSeriesSopInstances(request.StudyInstanceUid, request.SeriesInstanceUids), callback);
		}

		public SendOperationReference SendSopInstances(SendSopInstancesRequest request, SendOperationProgressCallback callback)
		{
			return Send(request.DestinationAEInformation, request.IsBackground,
				DataStoreQueryHelper.GetSopInstances(request.StudyInstanceUid, request.SeriesInstanceUid, request.SopInstanceUids), callback);
		}

		public SendOperationReference SendFiles(SendFilesRequest request, SendOperationProgressCallback callback)
		{
			return Send(request, callback);
		}

		public void Cancel(SendOperationReference sendOperation)
		{
			lock (_syncLock)
			{
				SendScu scu = _scus.Find(delegate(SendScu test) { return test.Reference == sendOperation; });
				if (scu != null)
					scu.Cancel(false);
			}
		}

		#endregion

	}
}
