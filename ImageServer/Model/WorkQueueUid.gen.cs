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
    public partial class WorkQueueUid: ServerEntity
    {
        #region Constructors
        public WorkQueueUid():base("WorkQueueUid")
        {}
        public WorkQueueUid(
             System.Boolean _duplicate_
            ,System.String _extension_
            ,System.Boolean _failed_
            ,System.Int16 _failureCount_
            ,System.String _groupID_
            ,System.String _relativePath_
            ,System.String _seriesInstanceUid_
            ,System.String _sopInstanceUid_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _workQueueKey_
            ):base("WorkQueueUid")
        {
            _duplicate = _duplicate_;
            _extension = _extension_;
            _failed = _failed_;
            _failureCount = _failureCount_;
            _groupID = _groupID_;
            _relativePath = _relativePath_;
            _seriesInstanceUid = _seriesInstanceUid_;
            _sopInstanceUid = _sopInstanceUid_;
            _workQueueKey = _workQueueKey_;
        }
        #endregion

        #region Private Members
        private Boolean _duplicate;
        private String _extension;
        private Boolean _failed;
        private Int16 _failureCount;
        private String _groupID;
        private String _relativePath;
        private String _seriesInstanceUid;
        private String _sopInstanceUid;
        private ServerEntityKey _workQueueKey;
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Duplicate")]
        public Boolean Duplicate
        {
        get { return _duplicate; }
        set { _duplicate = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Extension")]
        public String Extension
        {
        get { return _extension; }
        set { _extension = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Failed")]
        public Boolean Failed
        {
        get { return _failed; }
        set { _failed = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="FailureCount")]
        public Int16 FailureCount
        {
        get { return _failureCount; }
        set { _failureCount = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="GroupID")]
        public String GroupID
        {
        get { return _groupID; }
        set { _groupID = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="RelativePath")]
        public String RelativePath
        {
        get { return _relativePath; }
        set { _relativePath = value; }
        }
        [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="SeriesInstanceUid")]
        public String SeriesInstanceUid
        {
        get { return _seriesInstanceUid; }
        set { _seriesInstanceUid = value; }
        }
        [DicomField(DicomTags.SopInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="SopInstanceUid")]
        public String SopInstanceUid
        {
        get { return _sopInstanceUid; }
        set { _sopInstanceUid = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="WorkQueueGUID")]
        public ServerEntityKey WorkQueueKey
        {
        get { return _workQueueKey; }
        set { _workQueueKey = value; }
        }
        #endregion

        #region Static Methods
        static public WorkQueueUid Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public WorkQueueUid Load(IPersistenceContext read, ServerEntityKey key)
        {
            IWorkQueueUidEntityBroker broker = read.GetBroker<IWorkQueueUidEntityBroker>();
            WorkQueueUid theObject = broker.Load(key);
            return theObject;
        }
        static public WorkQueueUid Insert(WorkQueueUid entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                WorkQueueUid newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public WorkQueueUid Insert(IUpdateContext update, WorkQueueUid entity)
        {
            IWorkQueueUidEntityBroker broker = update.GetBroker<IWorkQueueUidEntityBroker>();
            WorkQueueUidUpdateColumns updateColumns = new WorkQueueUidUpdateColumns();
            updateColumns.Duplicate = entity.Duplicate;
            updateColumns.Extension = entity.Extension;
            updateColumns.Failed = entity.Failed;
            updateColumns.FailureCount = entity.FailureCount;
            updateColumns.GroupID = entity.GroupID;
            updateColumns.RelativePath = entity.RelativePath;
            updateColumns.SeriesInstanceUid = entity.SeriesInstanceUid;
            updateColumns.SopInstanceUid = entity.SopInstanceUid;
            updateColumns.WorkQueueKey = entity.WorkQueueKey;
            WorkQueueUid newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
