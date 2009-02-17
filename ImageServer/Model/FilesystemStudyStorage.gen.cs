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
    public partial class FilesystemStudyStorage: ServerEntity
    {
        #region Constructors
        public FilesystemStudyStorage():base("FilesystemStudyStorage")
        {}
        public FilesystemStudyStorage(
             ClearCanvas.ImageServer.Enterprise.ServerEntityKey _filesystemKey_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _serverTransferSyntaxKey_
            ,System.String _studyFolder_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _studyStorageKey_
            ):base("FilesystemStudyStorage")
        {
            _filesystemKey = _filesystemKey_;
            _serverTransferSyntaxKey = _serverTransferSyntaxKey_;
            _studyFolder = _studyFolder_;
            _studyStorageKey = _studyStorageKey_;
        }
        #endregion

        #region Private Members
        private ClearCanvas.ImageServer.Enterprise.ServerEntityKey _filesystemKey;
        private ClearCanvas.ImageServer.Enterprise.ServerEntityKey _serverTransferSyntaxKey;
        private System.String _studyFolder;
        private ClearCanvas.ImageServer.Enterprise.ServerEntityKey _studyStorageKey;
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemStudyStorage", ColumnName="FilesystemGUID")]
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey FilesystemKey
        {
        get { return _filesystemKey; }
        set { _filesystemKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemStudyStorage", ColumnName="ServerTransferSyntaxGUID")]
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey ServerTransferSyntaxKey
        {
        get { return _serverTransferSyntaxKey; }
        set { _serverTransferSyntaxKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemStudyStorage", ColumnName="StudyFolder")]
        public System.String StudyFolder
        {
        get { return _studyFolder; }
        set { _studyFolder = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemStudyStorage", ColumnName="StudyStorageGUID")]
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey StudyStorageKey
        {
        get { return _studyStorageKey; }
        set { _studyStorageKey = value; }
        }
        #endregion

        #region Static Methods
        static public FilesystemStudyStorage Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public FilesystemStudyStorage Load(IReadContext read, ServerEntityKey key)
        {
            IFilesystemStudyStorageEntityBroker broker = read.GetBroker<IFilesystemStudyStorageEntityBroker>();
            FilesystemStudyStorage theObject = broker.Load(key);
            return theObject;
        }
        static public FilesystemStudyStorage Insert(FilesystemStudyStorage table)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                return Insert(update, table);
            }
        }
        static public FilesystemStudyStorage Insert(IUpdateContext update, FilesystemStudyStorage table)
        {
            IFilesystemStudyStorageEntityBroker broker = update.GetBroker<IFilesystemStudyStorageEntityBroker>();
            FilesystemStudyStorageUpdateColumns updateColumns = new FilesystemStudyStorageUpdateColumns();
            updateColumns.FilesystemKey = table.FilesystemKey;
            updateColumns.ServerTransferSyntaxKey = table.ServerTransferSyntaxKey;
            updateColumns.StudyFolder = table.StudyFolder;
            updateColumns.StudyStorageKey = table.StudyStorageKey;
            FilesystemStudyStorage theObject = broker.Insert(updateColumns);
            update.Commit();
            return theObject;
        }
        #endregion
    }
}
