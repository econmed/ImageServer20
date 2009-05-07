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
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class WorkQueue: ServerEntity
    {
        #region Constructors
        public WorkQueue():base("WorkQueue")
        {}
        public WorkQueue(
             System.Xml.XmlDocument _data_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _deviceKey_
            ,System.DateTime _expirationTime_
            ,System.Int32 _failureCount_
            ,System.String _failureDescription_
            ,System.String _groupID_
            ,System.DateTime _insertTime_
            ,System.String _processorID_
            ,System.DateTime _scheduledTime_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _serverPartitionKey_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _studyHistoryKey_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _studyStorageKey_
            ,WorkQueuePriorityEnum _workQueuePriorityEnum_
            ,WorkQueueStatusEnum _workQueueStatusEnum_
            ,WorkQueueTypeEnum _workQueueTypeEnum_
            ):base("WorkQueue")
        {
            _data = _data_;
            _deviceKey = _deviceKey_;
            _expirationTime = _expirationTime_;
            _failureCount = _failureCount_;
            _failureDescription = _failureDescription_;
            _groupID = _groupID_;
            _insertTime = _insertTime_;
            _processorID = _processorID_;
            _scheduledTime = _scheduledTime_;
            _serverPartitionKey = _serverPartitionKey_;
            _studyHistoryKey = _studyHistoryKey_;
            _studyStorageKey = _studyStorageKey_;
            _workQueuePriorityEnum = _workQueuePriorityEnum_;
            _workQueueStatusEnum = _workQueueStatusEnum_;
            _workQueueTypeEnum = _workQueueTypeEnum_;
        }
        #endregion

        #region Private Members
        private XmlDocument _data;
        private ServerEntityKey _deviceKey;
        private DateTime _expirationTime;
        private Int32 _failureCount;
        private String _failureDescription;
        private String _groupID;
        private DateTime _insertTime;
        private String _processorID;
        private DateTime _scheduledTime;
        private ServerEntityKey _serverPartitionKey;
        private ServerEntityKey _studyHistoryKey;
        private ServerEntityKey _studyStorageKey;
        private WorkQueuePriorityEnum _workQueuePriorityEnum;
        private WorkQueueStatusEnum _workQueueStatusEnum;
        private WorkQueueTypeEnum _workQueueTypeEnum;
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="Data")]
        public XmlDocument Data
        {
        get { return _data; }
        set { _data = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="DeviceGUID")]
        public ServerEntityKey DeviceKey
        {
        get { return _deviceKey; }
        set { _deviceKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="ExpirationTime")]
        public DateTime ExpirationTime
        {
        get { return _expirationTime; }
        set { _expirationTime = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="FailureCount")]
        public Int32 FailureCount
        {
        get { return _failureCount; }
        set { _failureCount = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="FailureDescription")]
        public String FailureDescription
        {
        get { return _failureDescription; }
        set { _failureDescription = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="GroupID")]
        public String GroupID
        {
        get { return _groupID; }
        set { _groupID = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="InsertTime")]
        public DateTime InsertTime
        {
        get { return _insertTime; }
        set { _insertTime = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="ProcessorID")]
        public String ProcessorID
        {
        get { return _processorID; }
        set { _processorID = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="ScheduledTime")]
        public DateTime ScheduledTime
        {
        get { return _scheduledTime; }
        set { _scheduledTime = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="ServerPartitionGUID")]
        public ServerEntityKey ServerPartitionKey
        {
        get { return _serverPartitionKey; }
        set { _serverPartitionKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="StudyHistoryGUID")]
        public ServerEntityKey StudyHistoryKey
        {
        get { return _studyHistoryKey; }
        set { _studyHistoryKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="StudyStorageGUID")]
        public ServerEntityKey StudyStorageKey
        {
        get { return _studyStorageKey; }
        set { _studyStorageKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="WorkQueuePriorityEnum")]
        public WorkQueuePriorityEnum WorkQueuePriorityEnum
        {
        get { return _workQueuePriorityEnum; }
        set { _workQueuePriorityEnum = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="WorkQueueStatusEnum")]
        public WorkQueueStatusEnum WorkQueueStatusEnum
        {
        get { return _workQueueStatusEnum; }
        set { _workQueueStatusEnum = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="WorkQueueTypeEnum")]
        public WorkQueueTypeEnum WorkQueueTypeEnum
        {
        get { return _workQueueTypeEnum; }
        set { _workQueueTypeEnum = value; }
        }
        #endregion

        #region Static Methods
        static public WorkQueue Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public WorkQueue Load(IPersistenceContext read, ServerEntityKey key)
        {
            IWorkQueueEntityBroker broker = read.GetBroker<IWorkQueueEntityBroker>();
            WorkQueue theObject = broker.Load(key);
            return theObject;
        }
        static public WorkQueue Insert(WorkQueue entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                WorkQueue newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public WorkQueue Insert(IUpdateContext update, WorkQueue entity)
        {
            IWorkQueueEntityBroker broker = update.GetBroker<IWorkQueueEntityBroker>();
            WorkQueueUpdateColumns updateColumns = new WorkQueueUpdateColumns();
            updateColumns.Data = entity.Data;
            updateColumns.DeviceKey = entity.DeviceKey;
            updateColumns.ExpirationTime = entity.ExpirationTime;
            updateColumns.FailureCount = entity.FailureCount;
            updateColumns.FailureDescription = entity.FailureDescription;
            updateColumns.GroupID = entity.GroupID;
            updateColumns.InsertTime = entity.InsertTime;
            updateColumns.ProcessorID = entity.ProcessorID;
            updateColumns.ScheduledTime = entity.ScheduledTime;
            updateColumns.ServerPartitionKey = entity.ServerPartitionKey;
            updateColumns.StudyHistoryKey = entity.StudyHistoryKey;
            updateColumns.StudyStorageKey = entity.StudyStorageKey;
            updateColumns.WorkQueuePriorityEnum = entity.WorkQueuePriorityEnum;
            updateColumns.WorkQueueStatusEnum = entity.WorkQueueStatusEnum;
            updateColumns.WorkQueueTypeEnum = entity.WorkQueueTypeEnum;
            WorkQueue newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
