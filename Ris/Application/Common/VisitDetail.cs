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
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class VisitDetail : DataContractBase
    {
        public VisitDetail()
        {
            this.VisitNumber = new CompositeIdentifierDetail();
            this.Locations = new List<VisitLocationDetail>();
            this.Practitioners = new List<VisitPractitionerDetail>();
            this.AmbulatoryStatuses = new List<EnumValueInfo>();
			this.ExtendedProperties = new Dictionary<string, string>();
        }

        [DataMember]
        public EntityRef VisitRef;
        
        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public CompositeIdentifierDetail VisitNumber;

        [DataMember]
        public EnumValueInfo PatientClass;

        [DataMember]
        public EnumValueInfo PatientType;

        [DataMember]
        public EnumValueInfo AdmissionType;

        [DataMember]
        public EnumValueInfo Status;

        [DataMember]
        public DateTime? AdmitTime;

        [DataMember]
        public DateTime? DischargeTime;

        [DataMember]
        public FacilitySummary Facility;

		[DataMember]
		public LocationSummary CurrentLocation;

		[DataMember]
        public string DischargeDisposition;

        [DataMember]
        public List<VisitLocationDetail> Locations;

        [DataMember]
        public List<VisitPractitionerDetail> Practitioners;

        [DataMember]
        public bool VipIndicator;

        [DataMember]
        public string PreadmitNumber;

        [DataMember]
        public List<EnumValueInfo> AmbulatoryStatuses;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;

        public VisitSummary GetSummary()
        {
            return new VisitSummary(this.VisitRef, this.PatientRef, this.VisitNumber,
                this.PatientClass, this.PatientType, this.AdmissionType,
                this.Status, this.AdmitTime, this.DischargeTime, this.Facility, this.CurrentLocation);
        }
    }
}
