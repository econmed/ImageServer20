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

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using ClearCanvas.Dicom;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class Series: ServerEntity
    {
        #region Constructors
        public Series():base("Series")
        {}
        #endregion

        #region Private Members
        private System.String _modality;
        private System.Int32 _numberOfSeriesRelatedInstances;
        private System.String _performedProcedureStepStartDate;
        private System.String _performedProcedureStepStartTime;
        private System.String _seriesDescription;
        private System.String _seriesInstanceUid;
        private System.String _seriesNumber;
        private ClearCanvas.ImageServer.Enterprise.ServerEntityKey _serverPartitionKey;
        private System.String _sourceApplicationEntityTitle;
        private ClearCanvas.ImageServer.Enterprise.ServerEntityKey _studyKey;
        #endregion

        #region Public Properties
        [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="Modality")]
        public System.String Modality
        {
        get { return _modality; }
        set { _modality = value; }
        }
        [DicomField(DicomTags.NumberOfSeriesRelatedInstances, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="NumberOfSeriesRelatedInstances")]
        public System.Int32 NumberOfSeriesRelatedInstances
        {
        get { return _numberOfSeriesRelatedInstances; }
        set { _numberOfSeriesRelatedInstances = value; }
        }
        [DicomField(DicomTags.PerformedProcedureStepStartDate, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="PerformedProcedureStepStartDate")]
        public System.String PerformedProcedureStepStartDate
        {
        get { return _performedProcedureStepStartDate; }
        set { _performedProcedureStepStartDate = value; }
        }
        [DicomField(DicomTags.PerformedProcedureStepStartTime, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="PerformedProcedureStepStartTime")]
        public System.String PerformedProcedureStepStartTime
        {
        get { return _performedProcedureStepStartTime; }
        set { _performedProcedureStepStartTime = value; }
        }
        [DicomField(DicomTags.SeriesDescription, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="SeriesDescription")]
        public System.String SeriesDescription
        {
        get { return _seriesDescription; }
        set { _seriesDescription = value; }
        }
        [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="SeriesInstanceUid")]
        public System.String SeriesInstanceUid
        {
        get { return _seriesInstanceUid; }
        set { _seriesInstanceUid = value; }
        }
        [DicomField(DicomTags.SeriesNumber, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="SeriesNumber")]
        public System.String SeriesNumber
        {
        get { return _seriesNumber; }
        set { _seriesNumber = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="ServerPartitionGUID")]
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey ServerPartitionKey
        {
        get { return _serverPartitionKey; }
        set { _serverPartitionKey = value; }
        }
        [DicomField(DicomTags.SourceApplicationEntityTitle, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="SourceApplicationEntityTitle")]
        public System.String SourceApplicationEntityTitle
        {
        get { return _sourceApplicationEntityTitle; }
        set { _sourceApplicationEntityTitle = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Series", ColumnName="StudyGUID")]
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey StudyKey
        {
        get { return _studyKey; }
        set { _studyKey = value; }
        }
        #endregion

        #region Static Methods
        static public Series Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public Series Load(IReadContext read, ServerEntityKey key)
        {
            ISeriesEntityBroker broker = read.GetBroker<ISeriesEntityBroker>();
            Series theObject = broker.Load(key);
            return theObject;
        }
        #endregion
    }
}
