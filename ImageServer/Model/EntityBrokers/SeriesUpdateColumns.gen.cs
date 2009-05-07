#region License

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

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    using System;
    using System.Xml;
    using ClearCanvas.Dicom;
    using ClearCanvas.ImageServer.Enterprise;

   public class SeriesUpdateColumns : EntityUpdateColumns
   {
       public SeriesUpdateColumns()
       : base("Series")
       {}
       [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="Modality")]
        public String Modality
        {
            set { SubParameters["Modality"] = new EntityUpdateColumn<String>("Modality", value); }
        }
       [DicomField(DicomTags.NumberOfSeriesRelatedInstances, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="NumberOfSeriesRelatedInstances")]
        public Int32 NumberOfSeriesRelatedInstances
        {
            set { SubParameters["NumberOfSeriesRelatedInstances"] = new EntityUpdateColumn<Int32>("NumberOfSeriesRelatedInstances", value); }
        }
       [DicomField(DicomTags.PerformedProcedureStepStartDate, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="PerformedProcedureStepStartDate")]
        public String PerformedProcedureStepStartDate
        {
            set { SubParameters["PerformedProcedureStepStartDate"] = new EntityUpdateColumn<String>("PerformedProcedureStepStartDate", value); }
        }
       [DicomField(DicomTags.PerformedProcedureStepStartTime, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="PerformedProcedureStepStartTime")]
        public String PerformedProcedureStepStartTime
        {
            set { SubParameters["PerformedProcedureStepStartTime"] = new EntityUpdateColumn<String>("PerformedProcedureStepStartTime", value); }
        }
       [DicomField(DicomTags.SeriesDescription, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="SeriesDescription")]
        public String SeriesDescription
        {
            set { SubParameters["SeriesDescription"] = new EntityUpdateColumn<String>("SeriesDescription", value); }
        }
       [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="SeriesInstanceUid")]
        public String SeriesInstanceUid
        {
            set { SubParameters["SeriesInstanceUid"] = new EntityUpdateColumn<String>("SeriesInstanceUid", value); }
        }
       [DicomField(DicomTags.SeriesNumber, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="SeriesNumber")]
        public String SeriesNumber
        {
            set { SubParameters["SeriesNumber"] = new EntityUpdateColumn<String>("SeriesNumber", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="ServerPartitionGUID")]
        public ServerEntityKey ServerPartitionKey
        {
            set { SubParameters["ServerPartitionKey"] = new EntityUpdateColumn<ServerEntityKey>("ServerPartitionKey", value); }
        }
       [DicomField(DicomTags.SourceApplicationEntityTitle, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="SourceApplicationEntityTitle")]
        public String SourceApplicationEntityTitle
        {
            set { SubParameters["SourceApplicationEntityTitle"] = new EntityUpdateColumn<String>("SourceApplicationEntityTitle", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="StudyGUID")]
        public ServerEntityKey StudyKey
        {
            set { SubParameters["StudyKey"] = new EntityUpdateColumn<ServerEntityKey>("StudyKey", value); }
        }
    }
}
