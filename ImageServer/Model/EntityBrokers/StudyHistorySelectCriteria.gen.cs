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

    public partial class StudyHistorySelectCriteria : EntitySelectCriteria
    {
        public StudyHistorySelectCriteria()
        : base("StudyHistory")
        {}
        public StudyHistorySelectCriteria(StudyHistorySelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new StudyHistorySelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="InsertTime")]
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
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="StudyStorageGUID")]
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
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="StudyHistoryTypeEnum")]
        public ISearchCondition<StudyHistoryTypeEnum> StudyHistoryTypeEnum
        {
            get
            {
              if (!SubCriteria.ContainsKey("StudyHistoryTypeEnum"))
              {
                 SubCriteria["StudyHistoryTypeEnum"] = new SearchCondition<StudyHistoryTypeEnum>("StudyHistoryTypeEnum");
              }
              return (ISearchCondition<StudyHistoryTypeEnum>)SubCriteria["StudyHistoryTypeEnum"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="StudyData")]
        public ISearchCondition<XmlDocument> StudyData
        {
            get
            {
              if (!SubCriteria.ContainsKey("StudyData"))
              {
                 SubCriteria["StudyData"] = new SearchCondition<XmlDocument>("StudyData");
              }
              return (ISearchCondition<XmlDocument>)SubCriteria["StudyData"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="ChangeDescription")]
        public ISearchCondition<XmlDocument> ChangeDescription
        {
            get
            {
              if (!SubCriteria.ContainsKey("ChangeDescription"))
              {
                 SubCriteria["ChangeDescription"] = new SearchCondition<XmlDocument>("ChangeDescription");
              }
              return (ISearchCondition<XmlDocument>)SubCriteria["ChangeDescription"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="DestStudyStorageGUID")]
        public ISearchCondition<ServerEntityKey> DestStudyStorageKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("DestStudyStorageKey"))
              {
                 SubCriteria["DestStudyStorageKey"] = new SearchCondition<ServerEntityKey>("DestStudyStorageKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["DestStudyStorageKey"];
            } 
        }
    }
}
