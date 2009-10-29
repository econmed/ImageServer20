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
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;

    public partial class WorkQueueSelectCriteria : EntitySelectCriteria
    {
        public WorkQueueSelectCriteria()
        : base("WorkQueue")
        {}
        public WorkQueueSelectCriteria(WorkQueueSelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new WorkQueueSelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="ServerPartitionGUID")]
        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("ServerPartitionKey"))
              {
                 SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["ServerPartitionKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="StudyStorageGUID")]
        public ISearchCondition<ServerEntityKey> StudyStorageKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("StudyStorageKey"))
              {
                 SubCriteria["StudyStorageKey"] = new SearchCondition<ServerEntityKey>("StudyStorageKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["StudyStorageKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="WorkQueueTypeEnum")]
        public ISearchCondition<WorkQueueTypeEnum> WorkQueueTypeEnum
        {
            get
            {
              if (!SubCriteria.ContainsKey("WorkQueueTypeEnum"))
              {
                 SubCriteria["WorkQueueTypeEnum"] = new SearchCondition<WorkQueueTypeEnum>("WorkQueueTypeEnum");
              }
              return (ISearchCondition<WorkQueueTypeEnum>)SubCriteria["WorkQueueTypeEnum"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="WorkQueueStatusEnum")]
        public ISearchCondition<WorkQueueStatusEnum> WorkQueueStatusEnum
        {
            get
            {
              if (!SubCriteria.ContainsKey("WorkQueueStatusEnum"))
              {
                 SubCriteria["WorkQueueStatusEnum"] = new SearchCondition<WorkQueueStatusEnum>("WorkQueueStatusEnum");
              }
              return (ISearchCondition<WorkQueueStatusEnum>)SubCriteria["WorkQueueStatusEnum"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="WorkQueuePriorityEnum")]
        public ISearchCondition<WorkQueuePriorityEnum> WorkQueuePriorityEnum
        {
            get
            {
              if (!SubCriteria.ContainsKey("WorkQueuePriorityEnum"))
              {
                 SubCriteria["WorkQueuePriorityEnum"] = new SearchCondition<WorkQueuePriorityEnum>("WorkQueuePriorityEnum");
              }
              return (ISearchCondition<WorkQueuePriorityEnum>)SubCriteria["WorkQueuePriorityEnum"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="ScheduledTime")]
        public ISearchCondition<DateTime> ScheduledTime
        {
            get
            {
              if (!SubCriteria.ContainsKey("ScheduledTime"))
              {
                 SubCriteria["ScheduledTime"] = new SearchCondition<DateTime>("ScheduledTime");
              }
              return (ISearchCondition<DateTime>)SubCriteria["ScheduledTime"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="InsertTime")]
        public ISearchCondition<DateTime> InsertTime
        {
            get
            {
              if (!SubCriteria.ContainsKey("InsertTime"))
              {
                 SubCriteria["InsertTime"] = new SearchCondition<DateTime>("InsertTime");
              }
              return (ISearchCondition<DateTime>)SubCriteria["InsertTime"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="FailureCount")]
        public ISearchCondition<Int32> FailureCount
        {
            get
            {
              if (!SubCriteria.ContainsKey("FailureCount"))
              {
                 SubCriteria["FailureCount"] = new SearchCondition<Int32>("FailureCount");
              }
              return (ISearchCondition<Int32>)SubCriteria["FailureCount"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="FailureDescription")]
        public ISearchCondition<String> FailureDescription
        {
            get
            {
              if (!SubCriteria.ContainsKey("FailureDescription"))
              {
                 SubCriteria["FailureDescription"] = new SearchCondition<String>("FailureDescription");
              }
              return (ISearchCondition<String>)SubCriteria["FailureDescription"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="Data")]
        public ISearchCondition<XmlDocument> Data
        {
            get
            {
              if (!SubCriteria.ContainsKey("Data"))
              {
                 SubCriteria["Data"] = new SearchCondition<XmlDocument>("Data");
              }
              return (ISearchCondition<XmlDocument>)SubCriteria["Data"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="LastUpdatedTime")]
        public ISearchCondition<DateTime> LastUpdatedTime
        {
            get
            {
              if (!SubCriteria.ContainsKey("LastUpdatedTime"))
              {
                 SubCriteria["LastUpdatedTime"] = new SearchCondition<DateTime>("LastUpdatedTime");
              }
              return (ISearchCondition<DateTime>)SubCriteria["LastUpdatedTime"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="ProcessorID")]
        public ISearchCondition<String> ProcessorID
        {
            get
            {
              if (!SubCriteria.ContainsKey("ProcessorID"))
              {
                 SubCriteria["ProcessorID"] = new SearchCondition<String>("ProcessorID");
              }
              return (ISearchCondition<String>)SubCriteria["ProcessorID"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="GroupID")]
        public ISearchCondition<String> GroupID
        {
            get
            {
              if (!SubCriteria.ContainsKey("GroupID"))
              {
                 SubCriteria["GroupID"] = new SearchCondition<String>("GroupID");
              }
              return (ISearchCondition<String>)SubCriteria["GroupID"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="ExpirationTime")]
        public ISearchCondition<DateTime> ExpirationTime
        {
            get
            {
              if (!SubCriteria.ContainsKey("ExpirationTime"))
              {
                 SubCriteria["ExpirationTime"] = new SearchCondition<DateTime>("ExpirationTime");
              }
              return (ISearchCondition<DateTime>)SubCriteria["ExpirationTime"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="DeviceGUID")]
        public ISearchCondition<ServerEntityKey> DeviceKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("DeviceKey"))
              {
                 SubCriteria["DeviceKey"] = new SearchCondition<ServerEntityKey>("DeviceKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["DeviceKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueue", ColumnName="StudyHistoryGUID")]
        public ISearchCondition<ServerEntityKey> StudyHistoryKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("StudyHistoryKey"))
              {
                 SubCriteria["StudyHistoryKey"] = new SearchCondition<ServerEntityKey>("StudyHistoryKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["StudyHistoryKey"];
            } 
        }
    }
}
