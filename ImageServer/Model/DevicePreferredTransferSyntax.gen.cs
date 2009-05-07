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
    public partial class DevicePreferredTransferSyntax: ServerEntity
    {
        #region Constructors
        public DevicePreferredTransferSyntax():base("DevicePreferredTransferSyntax")
        {}
        public DevicePreferredTransferSyntax(
             ClearCanvas.ImageServer.Enterprise.ServerEntityKey _deviceKey_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _serverSopClassKey_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _serverTransferSyntaxKey_
            ):base("DevicePreferredTransferSyntax")
        {
            _deviceKey = _deviceKey_;
            _serverSopClassKey = _serverSopClassKey_;
            _serverTransferSyntaxKey = _serverTransferSyntaxKey_;
        }
        #endregion

        #region Private Members
        private ServerEntityKey _deviceKey;
        private ServerEntityKey _serverSopClassKey;
        private ServerEntityKey _serverTransferSyntaxKey;
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="DevicePreferredTransferSyntax", ColumnName="DeviceGUID")]
        public ServerEntityKey DeviceKey
        {
        get { return _deviceKey; }
        set { _deviceKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="DevicePreferredTransferSyntax", ColumnName="ServerSopClassGUID")]
        public ServerEntityKey ServerSopClassKey
        {
        get { return _serverSopClassKey; }
        set { _serverSopClassKey = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="DevicePreferredTransferSyntax", ColumnName="ServerTransferSyntaxGUID")]
        public ServerEntityKey ServerTransferSyntaxKey
        {
        get { return _serverTransferSyntaxKey; }
        set { _serverTransferSyntaxKey = value; }
        }
        #endregion

        #region Static Methods
        static public DevicePreferredTransferSyntax Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public DevicePreferredTransferSyntax Load(IPersistenceContext read, ServerEntityKey key)
        {
            IDevicePreferredTransferSyntaxEntityBroker broker = read.GetBroker<IDevicePreferredTransferSyntaxEntityBroker>();
            DevicePreferredTransferSyntax theObject = broker.Load(key);
            return theObject;
        }
        static public DevicePreferredTransferSyntax Insert(DevicePreferredTransferSyntax entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                DevicePreferredTransferSyntax newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public DevicePreferredTransferSyntax Insert(IUpdateContext update, DevicePreferredTransferSyntax entity)
        {
            IDevicePreferredTransferSyntaxEntityBroker broker = update.GetBroker<IDevicePreferredTransferSyntaxEntityBroker>();
            DevicePreferredTransferSyntaxUpdateColumns updateColumns = new DevicePreferredTransferSyntaxUpdateColumns();
            updateColumns.DeviceKey = entity.DeviceKey;
            updateColumns.ServerSopClassKey = entity.ServerSopClassKey;
            updateColumns.ServerTransferSyntaxKey = entity.ServerTransferSyntaxKey;
            DevicePreferredTransferSyntax newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
