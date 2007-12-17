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

using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    /// <summary>
    /// Criteria for selects against the <see cref="Series"/> table.
    /// </summary>
    public class SeriesSelectCriteria : SelectCriteria
    {
        public SeriesSelectCriteria()
            : base("Series")
        {}

        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ServerPartitionKey"))
                {
                    this.SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
                }
                return (ISearchCondition<ServerEntityKey>)this.SubCriteria["ServerPartitionKey"];
            }
        }
        public ISearchCondition<ServerEntityKey> StudyKey
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyKey"))
                {
                    this.SubCriteria["StudyKey"] = new SearchCondition<ServerEntityKey>("StudyKey");
                }
                return (ISearchCondition<ServerEntityKey>)this.SubCriteria["StudyKey"];
            }
        }

        [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> SeriesInstanceUid
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("SeriesInstanceUid"))
                {
                    this.SubCriteria["SeriesInstanceUid"] = new SearchCondition<string>("SeriesInstanceUid");
                }
                return (ISearchCondition<string>)this.SubCriteria["SeriesInstanceUid"];
            }
        }

        [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> Modality
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Modality"))
                {
                    this.SubCriteria["Modality"] = new SearchCondition<string>("Modality");
                }
                return (ISearchCondition<string>)this.SubCriteria["Modality"];
            }
        }

        [DicomField(DicomTags.SeriesNumber, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> SeriesNumber
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("SeriesNumber"))
                {
                    this.SubCriteria["SeriesNumber"] = new SearchCondition<string>("SeriesNumber");
                }
                return (ISearchCondition<string>)this.SubCriteria["SeriesNumber"];
            }
        }

        [DicomField(DicomTags.SeriesDescription, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> SeriesDescription
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("SeriesDescription"))
                {
                    this.SubCriteria["SeriesDescription"] = new SearchCondition<string>("SeriesDescription");
                }
                return (ISearchCondition<string>)this.SubCriteria["SeriesDescription"];
            }
        }
        [DicomField(DicomTags.PerformedProcedureStepStartDate, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PerformedProcedureStepStartDate
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PerformedProcedureStepStartDate"))
                {
                    this.SubCriteria["PerformedProcedureStepStartDate"] = new SearchCondition<string>("PerformedProcedureStepStartDate");
                }
                return (ISearchCondition<string>)this.SubCriteria["PerformedProcedureStepStartDate"];
            }
        }
        [DicomField(DicomTags.PerformedProcedureStepStartTime, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PerformedProcedureStepStartTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PerformedProcedureStepStartTime"))
                {
                    this.SubCriteria["PerformedProcedureStepStartTime"] = new SearchCondition<string>("PerformedProcedureStepStartTime");
                }
                return (ISearchCondition<string>)this.SubCriteria["PerformedProcedureStepStartTime"];
            }
        }
    }
}
