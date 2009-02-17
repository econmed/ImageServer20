#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class StudyHistory: ServerEntity
    {
        #region Constructors
        public StudyHistory():base("StudyHistory")
        {}
        public StudyHistory(
             System.Xml.XmlDocument _changeDescription_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _destStudyStorageKey_
            ,System.DateTime _insertTime_
            ,System.Xml.XmlDocument _studyData_
            ,StudyHistoryTypeEnum _studyHistoryTypeEnum_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _studyStorageKey_
            ):base("StudyHistory")
        {
            _changeDescription = _changeDescription_;
            _destStudyStorageKey = _destStudyStorageKey_;
            _insertTime = _insertTime_;
            _studyData = _studyData_;
            _studyHistoryTypeEnum = _studyHistoryTypeEnum_;
            _studyStorageKey = _studyStorageKey_;
        }
        #endregion

        #region Private Members
        private System.Xml.XmlDocument _changeDescription;
        private ClearCanvas.ImageServer.Enterprise.ServerEntityKey _destStudyStorageKey;
        private System.DateTime _insertTime;
        private System.Xml.XmlDocument _studyData;
        private StudyHistoryTypeEnum _studyHistoryTypeEnum;
        private ClearCanvas.ImageServer.Enterprise.ServerEntityKey _studyStorageKey;
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="ChangeDescription")]
        public System.Xml.XmlDocument ChangeDescription
        {
        get { return _changeDescription; }
        set { _changeDescription = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="DestStudyStorageGUID")]
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey DestStudyStorageKey
        {
        get { return _destStudyStorageKey; }
        set { _destStudyStorageKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="InsertTime")]
        public System.DateTime InsertTime
        {
        get { return _insertTime; }
        set { _insertTime = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="StudyData")]
        public System.Xml.XmlDocument StudyData
        {
        get { return _studyData; }
        set { _studyData = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="StudyHistoryTypeEnum")]
        public StudyHistoryTypeEnum StudyHistoryTypeEnum
        {
        get { return _studyHistoryTypeEnum; }
        set { _studyHistoryTypeEnum = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyHistory", ColumnName="StudyStorageGUID")]
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey StudyStorageKey
        {
        get { return _studyStorageKey; }
        set { _studyStorageKey = value; }
        }
        #endregion

        #region Static Methods
        static public StudyHistory Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public StudyHistory Load(IReadContext read, ServerEntityKey key)
        {
            IStudyHistoryEntityBroker broker = read.GetBroker<IStudyHistoryEntityBroker>();
            StudyHistory theObject = broker.Load(key);
            return theObject;
        }
        static public StudyHistory Insert(StudyHistory table)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                return Insert(update, table);
            }
        }
        static public StudyHistory Insert(IUpdateContext update, StudyHistory table)
        {
            IStudyHistoryEntityBroker broker = update.GetBroker<IStudyHistoryEntityBroker>();
            StudyHistoryUpdateColumns updateColumns = new StudyHistoryUpdateColumns();
            updateColumns.ChangeDescription = table.ChangeDescription;
            updateColumns.DestStudyStorageKey = table.DestStudyStorageKey;
            updateColumns.InsertTime = table.InsertTime;
            updateColumns.StudyData = table.StudyData;
            updateColumns.StudyHistoryTypeEnum = table.StudyHistoryTypeEnum;
            updateColumns.StudyStorageKey = table.StudyStorageKey;
            StudyHistory theObject = broker.Insert(updateColumns);
            update.Commit();
            return theObject;
        }
        #endregion
    }
}
