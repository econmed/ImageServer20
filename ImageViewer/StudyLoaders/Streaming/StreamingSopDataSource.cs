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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public interface IStreamingSopDataSource : IDicomMessageSopDataSource
	{
		new IStreamingSopFrameData GetFrameData(int frameNumber);
	}

	internal partial class StreamingSopDataSource : DicomMessageSopDataSource, IStreamingSopDataSource
	{
		private readonly string _host;
		private readonly string _aeTitle;
		private readonly string _wadoUriPrefix;
		private readonly int _wadoServicePort;
		private volatile bool _fullHeaderRetrieved = false;

		public StreamingSopDataSource(InstanceXml instanceXml, string host, string aeTitle, string wadoUriPrefix, int wadoServicePort)
			: base(new DicomFile("", new DicomAttributeCollection(), instanceXml.Collection))
		{
			//These don't get set properly for instance xml.
			DicomFile sourceFile = (DicomFile)SourceMessage;
			sourceFile.TransferSyntaxUid = instanceXml.TransferSyntax.UidString;
			sourceFile.MediaStorageSopInstanceUid = instanceXml.SopInstanceUid;
			sourceFile.MetaInfo[DicomTags.SopClassUid].SetString(0, instanceXml.SopClass.Uid);

			_host = host;
			_aeTitle = aeTitle;
			_wadoUriPrefix = wadoUriPrefix;
			_wadoServicePort = wadoServicePort;
		}

		private InstanceXmlDicomAttributeCollection AttributeCollection
		{
			get { return (InstanceXmlDicomAttributeCollection)SourceMessage.DataSet; }
		}

		#region IStreamingSopDataSource Members

		public new IStreamingSopFrameData GetFrameData(int frameNumber)
		{
			return (IStreamingSopFrameData) base.GetFrameData(frameNumber);
		}

		#endregion

		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				lock(SyncLock)
				{
					if (NeedFullHeader(tag.TagValue))
						GetFullHeader();

					return base[tag];
				}
			}
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				lock (SyncLock)
				{
					if (NeedFullHeader(tag))
						GetFullHeader();

					return base[tag];
				}
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock(SyncLock)
			{
				if (NeedFullHeader(tag.TagValue))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
		}

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			return new StreamingSopFrameData(frameNumber, this);
		}

		private bool NeedFullHeader(uint tag)
		{
			if (_fullHeaderRetrieved)
				return false;

			if (CollectionUtils.Contains(AttributeCollection.ExcludedTags, 
				delegate(DicomTag dicomTag) { return dicomTag.TagValue == tag; }))
			{
				return true;
			}

			DicomAttribute attribute = base[tag];
			if (attribute is DicomAttributeSQ)
			{
				DicomSequenceItem[] items = attribute.Values as DicomSequenceItem[];
				if (items != null)
				{
					foreach (DicomSequenceItem item in items)
					{
						if (item is InstanceXmlDicomSequenceItem)
						{
							if (((InstanceXmlDicomSequenceItem) item).HasExcludedTags(true))
								return true;
						}
					}
				}
			}

			return false;
		}

		private void GetFullHeader()
		{
			if (!_fullHeaderRetrieved)
			{
				Exception retrieveException;
				DicomFile imageHeader = TryClientRetrieveImageHeader(out retrieveException);

				if (imageHeader != null)
				{
					base.SourceMessage = imageHeader;
					_fullHeaderRetrieved = true;
				}

				// if no result was returned, then the throw an exception with an appropriate, user-friendly message
				throw TranslateStreamingException(retrieveException);
			}
		}

		private DicomFile TryClientRetrieveImageHeader(out Exception lastRetrieveException)
		{
			// retry parameters
			const int retryTimeout = 1500;
			int retryDelay = 50;
			int retryCounter = 0;

			Uri uri = new Uri(string.Format(StreamingSettings.Default.FormatWadoUriPrefix, _host, _wadoServicePort));
			StreamingClient client = new StreamingClient(uri);
			DicomFile result = null;
			lastRetrieveException = null;

			CodeClock timeoutClock = new CodeClock();
			timeoutClock.Start();

			while (true)
			{
				try
				{
					if (retryCounter > 0)
						Platform.Log(LogLevel.Info, "Retrying retrieve headers for Sop '{0}' (Attempt #{1})", this.SopInstanceUid, retryCounter);

					CodeClock statsClock = new CodeClock();
					statsClock.Start();

					using (Stream imageHeaderStream = client.RetrieveImageHeader(_aeTitle, this.StudyInstanceUid, this.SeriesInstanceUid, this.SopInstanceUid))
					{
						DicomFile imageHeader = new DicomFile();
						imageHeader.Load(imageHeaderStream);
						result = imageHeader;
					}

					statsClock.Stop();

					break;
				}
				catch (Exception ex)
				{
					lastRetrieveException = ex;

					timeoutClock.Stop();
					if (timeoutClock.Seconds*1000 >= retryTimeout)
					{
						// log an alert that we are aborting (exception trace at debug level only)
						int elapsed = (int) (1000*timeoutClock.Seconds);
						Platform.Log(LogLevel.Warn, "Failed to retrieve headers for Sop '{0}'; Aborting after {1} attempts in {2} ms", this.SopInstanceUid, retryCounter, elapsed);
						Platform.Log(LogLevel.Debug, ex, "[GetHeaders Fail-Abort] Sop: {0}, Retry Attempts: {1}, Elapsed: {2} ms", this.SopInstanceUid, retryCounter, elapsed);
						break;
					}
					timeoutClock.Start();

					retryCounter++;

					// log the retry (exception trace at debug level only)
					Platform.Log(LogLevel.Warn, "Failed to retrieve headers for Sop '{0}'; Retrying in {1} ms", this.SopInstanceUid, retryDelay);
					Platform.Log(LogLevel.Debug, ex, "[GetHeaders Fail-Retry] Sop: {0}, Retry in: {1} ms", this.SopInstanceUid, retryDelay);
					Thread.Sleep(retryDelay);

					retryDelay *= 2;
				}
			}

			return result;
		}

		/// <summary>
		/// Translates possible exceptions thrown by <see cref="StreamingClient"/> and related classes into standardized, user-friendly error messages.
		/// </summary>
		private static Exception TranslateStreamingException(Exception exception)
		{
			if (exception is StreamingClientException)
			{
				switch (((StreamingClientException) exception).Type)
				{
					case StreamingClientExceptionType.Access:
						return new InvalidOperationException(SR.MessageStreamingAccessException, exception);
					case StreamingClientExceptionType.Network:
						return new IOException(SR.MessageStreamingNetworkException, exception);
					case StreamingClientExceptionType.Protocol:
					case StreamingClientExceptionType.Server:
					case StreamingClientExceptionType.UnexpectedResponse:
					case StreamingClientExceptionType.Generic:
					default:
						return new Exception(SR.MessageStreamingGenericException, exception);
				}
			}
			return new Exception(SR.MessageStreamingGenericException, exception);
		}
	}
}
