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

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Xml;
    using ClearCanvas.Dicom;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class StudyIntegrityQueueUid: ServerEntity
    {
        #region Constructors
        public StudyIntegrityQueueUid():base("StudyIntegrityQueueUid")
        {}
        public StudyIntegrityQueueUid(
             System.String _relativePath_
            ,System.String _seriesDescription_
            ,System.String _seriesInstanceUid_
            ,System.String _sopInstanceUid_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _studyIntegrityQueueKey_
            ):base("StudyIntegrityQueueUid")
        {
            _relativePath = _relativePath_;
            _seriesDescription = _seriesDescription_;
            _seriesInstanceUid = _seriesInstanceUid_;
            _sopInstanceUid = _sopInstanceUid_;
            _studyIntegrityQueueKey = _studyIntegrityQueueKey_;
        }
        #endregion

        #region Private Members
        private String _relativePath;
        private String _seriesDescription;
        private String _seriesInstanceUid;
        private String _sopInstanceUid;
        private ServerEntityKey _studyIntegrityQueueKey;
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueueUid", ColumnName="RelativePath")]
        public String RelativePath
        {
        get { return _relativePath; }
        set { _relativePath = value; }
        }
        [DicomField(DicomTags.SeriesDescription, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueueUid", ColumnName="SeriesDescription")]
        public String SeriesDescription
        {
        get { return _seriesDescription; }
        set { _seriesDescription = value; }
        }
        [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueueUid", ColumnName="SeriesInstanceUid")]
        public String SeriesInstanceUid
        {
        get { return _seriesInstanceUid; }
        set { _seriesInstanceUid = value; }
        }
        [DicomField(DicomTags.SopInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueueUid", ColumnName="SopInstanceUid")]
        public String SopInstanceUid
        {
        get { return _sopInstanceUid; }
        set { _sopInstanceUid = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueueUid", ColumnName="StudyIntegrityQueueGUID")]
        public ServerEntityKey StudyIntegrityQueueKey
        {
        get { return _studyIntegrityQueueKey; }
        set { _studyIntegrityQueueKey = value; }
        }
        #endregion

        #region Static Methods
        static public StudyIntegrityQueueUid Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public StudyIntegrityQueueUid Load(IPersistenceContext read, ServerEntityKey key)
        {
            IStudyIntegrityQueueUidEntityBroker broker = read.GetBroker<IStudyIntegrityQueueUidEntityBroker>();
            StudyIntegrityQueueUid theObject = broker.Load(key);
            return theObject;
        }
        static public StudyIntegrityQueueUid Insert(StudyIntegrityQueueUid entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                StudyIntegrityQueueUid newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public StudyIntegrityQueueUid Insert(IUpdateContext update, StudyIntegrityQueueUid entity)
        {
            IStudyIntegrityQueueUidEntityBroker broker = update.GetBroker<IStudyIntegrityQueueUidEntityBroker>();
            StudyIntegrityQueueUidUpdateColumns updateColumns = new StudyIntegrityQueueUidUpdateColumns();
            updateColumns.RelativePath = entity.RelativePath;
            updateColumns.SeriesDescription = entity.SeriesDescription;
            updateColumns.SeriesInstanceUid = entity.SeriesInstanceUid;
            updateColumns.SopInstanceUid = entity.SopInstanceUid;
            updateColumns.StudyIntegrityQueueKey = entity.StudyIntegrityQueueKey;
            StudyIntegrityQueueUid newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
