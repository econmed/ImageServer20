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

using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ReportPartDetail : DataContractBase
    {
        public ReportPartDetail(EntityRef reportPartRef, int index, bool isAddendum, EnumValueInfo status,
            StaffSummary supervisor, StaffSummary interpretedBy, StaffSummary transcribedBy, StaffSummary verifiedBy,
            Dictionary<string, string> extendedProperties)
        {
            this.ReportPartRef = reportPartRef;
            this.Index = index;
            this.IsAddendum = isAddendum;
            this.Status = status;
            this.Supervisor = supervisor;
            this.InterpretedBy = interpretedBy;
            this.TranscribedBy = transcribedBy;
            this.VerifiedBy = verifiedBy;
            this.ExtendedProperties = extendedProperties;
        }

        public static string ReportContentKey = "ReportContent";

        [DataMember]
        public EntityRef ReportPartRef;

        [DataMember]
        public int Index;

        [DataMember]
        public bool IsAddendum;

        [DataMember]
        public EnumValueInfo Status;

        [DataMember]
        public StaffSummary Supervisor;

        [DataMember]
        public StaffSummary InterpretedBy;

        [DataMember]
        public StaffSummary TranscribedBy;

        [DataMember]
        public StaffSummary VerifiedBy;

        [DataMember]
        public Dictionary<string, string> ExtendedProperties;

        public string Content
        {
            get
            {
                if (ExtendedProperties == null || !ExtendedProperties.ContainsKey(ReportContentKey))
                    return null;

                return ExtendedProperties[ReportContentKey];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (ExtendedProperties != null && ExtendedProperties.ContainsKey(ReportContentKey))
                    {
                        ExtendedProperties.Remove(ReportContentKey);
                    }
                }
                else
                {
                    if (ExtendedProperties == null)
                        ExtendedProperties = new Dictionary<string, string>();

                    ExtendedProperties[ReportContentKey] = value;                
                }
            }
        }
    }
}