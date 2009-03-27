#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Object use for adding SOP Class information to the ParticipatingObjectDescription field in an Audit Message.
	/// </summary>
	public class AuditSopClass
	{
		private readonly string _uid;
		private readonly int _numberOfInstances;

		public AuditSopClass(string uid, int numberOfInstances)
		{
			_uid = uid;
			_numberOfInstances = numberOfInstances;
		}

		public string UID
		{
			get { return _uid; }
		}
		public int NumberOfInstances
		{
			get { return _numberOfInstances; }
		}
	}

	/// <summary>
	/// Base class for Audit helpers.
	/// </summary>
	public abstract class DicomAuditHelper
	{
		#region Static Members
		private static string _processId;
		private static readonly object _syncLock = new object();
		private static string _application;
		private static string _processName;
		#endregion

		#region Members
		private readonly AuditMessage _message = new AuditMessage();
		protected readonly List<AuditMessageActiveParticipant> _participantList = new List<AuditMessageActiveParticipant>(3);
		protected readonly List<AuditSourceIdentificationType> _auditSourceList = new List<AuditSourceIdentificationType>(1);
		protected readonly List<ParticipantObjectIdentificationType> _participantObjectList = new List<ParticipantObjectIdentificationType>();
		#endregion

		#region Static Properties
		public static string ProcessId
		{
			get
			{
				lock (_syncLock)
				{
					if (_processId == null)
					{
						string hostName = Dns.GetHostName();
						IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
						foreach (IPAddress ip in ipAddresses)
						{
							if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
							{
								_processId = ip.ToString();
							}
							else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
							{
								_processId = ip.ToString();
							}
						}
					}
					return _processId;
				}
			}
		}
		public static string ProcessName
		{
			get
			{
				lock (_syncLock)
				{
					if (_processName == null) _processName = Process.GetCurrentProcess().ProcessName;
					return _processName;
				}
			}
		
		}
		
		public static string Application
		{
			get
			{
				lock (_syncLock)
					return _application;
			}
			set
			{
				lock (_syncLock)
				{
					_application = value;
				}
			}
		}
		#endregion

		#region Properties
		protected AuditMessage AuditMessage
		{
			get { return _message; }
		}
		#endregion


		#region Public Methods
		public bool Verify(out string failureMessage)
		{
			XmlSchema schema;

			using (Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "DicomAuditMessageSchema.xsd"))
			{
				if (stream == null)
					throw new DicomException("Unable to load script resource (is the script an embedded resource?): " + "DicomAuditMessageSchema.xsd");

				schema = XmlSchema.Read(stream, null);
			}
	
			try
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.Schemas = new XmlSchemaSet();
				xmlReaderSettings.Schemas.Add(schema);
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

				XmlReader xmlReader = XmlTextReader.Create(new StringReader(Serialize()), xmlReaderSettings);
				while (xmlReader.Read()) ;
				xmlReader.Close();
			}
			catch (Exception e)
			{
				failureMessage = e.Message;
				return false;
			}

			failureMessage = string.Empty;
			return true;
		}

		public string Serialize()
		{
			return Serialize(false);
		}

		public string Serialize(bool format)
		{
			AuditMessage.ActiveParticipant = _participantList.ToArray();
			AuditMessage.AuditSourceIdentification = _auditSourceList.ToArray();
			AuditMessage.ParticipantObjectIdentification = _participantObjectList.ToArray();

			TextWriter tw = new StringWriter();
			XmlWriter writer = XmlWriter.Create(tw);
			
			writer.Settings.Encoding = Encoding.UTF8;
			if (format)
			{
				writer.Settings.NewLineOnAttributes = false;
				writer.Settings.Indent = true;
				writer.Settings.IndentChars = "  ";
			}
			else
			{
				writer.Settings.NewLineOnAttributes = false;
				writer.Settings.Indent = false;
			}

			XmlSerializer serializer = new XmlSerializer(typeof(AuditMessage));
			serializer.Serialize(writer, AuditMessage);
			return tw.ToString();
		}
		#endregion

		#region Protected Methods
	

		protected void InternalAddActiveDicomParticipant(AssociationParameters parms)
		{
			if (parms is ClientAssociationParameters)
			{
				_participantList.Add(
					new AuditMessageActiveParticipant(CodedValueType.Source, "AETITLE=" + parms.CallingAE, null, null,
													  parms.LocalEndPoint.Address.ToString(), NetworkAccessPointTypeEnum.IpAddress, null));
				_participantList.Add(
					new AuditMessageActiveParticipant(CodedValueType.Destination, "AETITLE=" + parms.CalledAE, null, null,
													  parms.RemoteEndPoint.Address.ToString(), NetworkAccessPointTypeEnum.IpAddress, null));
			}
			else
			{
				_participantList.Add(
					new AuditMessageActiveParticipant(CodedValueType.Source, "AETITLE=" + parms.CallingAE, null, null,
													  parms.RemoteEndPoint.Address.ToString(), NetworkAccessPointTypeEnum.IpAddress, null));
				_participantList.Add(
					new AuditMessageActiveParticipant(CodedValueType.Destination, "AETITLE=" + parms.CalledAE, null,null,
													  parms.LocalEndPoint.Address.ToString(), NetworkAccessPointTypeEnum.IpAddress, null));
			}
		}

		protected void InternalAddAuditSource(string enterpriseId, string sourceId, AuditSourceTypeCodeEnum e)
		{
			_auditSourceList.Add(new AuditSourceIdentificationType(enterpriseId,sourceId,e));
		}

		protected void InternalAddAuditSource(string sourceId)
		{
			_auditSourceList.Add(new AuditSourceIdentificationType(sourceId));
		}

		protected void InternalAddPatientParticipantObject(string patientId, string patientName)
		{
			ParticipantObjectIdentificationType o = new ParticipantObjectIdentificationType(ParticipantObjectTypeCodeEnum.Person,
																							ParticipantObjectTypeCodeRoleEnum.Patient,
																							null, patientId,
																							ParticipateObjectIdTypeCodeEnum.PatientNumber);
			o.Item = patientName;
			_participantObjectList.Add(o);
		}


		protected void InternalAddStudyParticipantObject(string studyInstanceUid)
		{
			ParticipantObjectIdentificationType o = new ParticipantObjectIdentificationType(ParticipantObjectTypeCodeEnum.SystemObject,
																							ParticipantObjectTypeCodeRoleEnum.Report,
																							ParticipantObjectDataLifeCycleEnum.OriginationCreation,
																							studyInstanceUid,
																							CodedValueType.StudyInstanceUID);
			_participantObjectList.Add(o);
		}

		protected void InternalAddStudyParticipantObject(string studyInstanceUid, string mppsUid, string accessionNumber, AuditSopClass[] sopClasses)
		{
			ParticipantObjectIdentificationType o = new ParticipantObjectIdentificationType(ParticipantObjectTypeCodeEnum.SystemObject,
																							ParticipantObjectTypeCodeRoleEnum.Report,
																							ParticipantObjectDataLifeCycleEnum.OriginationCreation,
																							studyInstanceUid,
																							CodedValueType.StudyInstanceUID);
			ParticipantObjectDescriptionType description = new ParticipantObjectDescriptionType();
			if (!String.IsNullOrEmpty(accessionNumber))
				description.Accession = new ParticipantObjectDescriptionTypeAccession[] { new ParticipantObjectDescriptionTypeAccession(accessionNumber) };
			if (!String.IsNullOrEmpty(mppsUid))
				description.MPPS = new ParticipantObjectDescriptionTypeMPPS[] { new ParticipantObjectDescriptionTypeMPPS(mppsUid) };

			if (sopClasses != null)
			{
				description.SOPClass = new ParticipantObjectDescriptionTypeSOPClass[sopClasses.Length];
				for (int i = 0; i < sopClasses.Length; i++)
				{
					description.SOPClass[i] =
						new ParticipantObjectDescriptionTypeSOPClass(sopClasses[i].UID, sopClasses[i].NumberOfInstances);
				}
			}

			o.ParticipantObjectDescription = new ParticipantObjectDescriptionType[] { description };

			_participantObjectList.Add(o);
		}

		protected void InternalAddActiveParticipant(CodedValueType roleId, string userId, string alternateUserId, string userName)
		{
			_participantList.Add(
				new AuditMessageActiveParticipant(null, userId, null, userName, ProcessId, NetworkAccessPointTypeEnum.IpAddress, null));
		}
		#endregion

	}
}