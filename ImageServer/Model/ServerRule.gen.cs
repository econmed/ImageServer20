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
    public partial class ServerRule: ServerEntity
    {
        #region Constructors
        public ServerRule():base("ServerRule")
        {}
        public ServerRule(
             String _ruleName_
            ,ServerEntityKey _serverPartitionKey_
            ,ServerRuleTypeEnum _serverRuleTypeEnum_
            ,ServerRuleApplyTimeEnum _serverRuleApplyTimeEnum_
            ,Boolean _enabled_
            ,Boolean _defaultRule_
            ,Boolean _exemptRule_
            ,XmlDocument _ruleXml_
            ):base("ServerRule")
        {
            RuleName = _ruleName_;
            ServerPartitionKey = _serverPartitionKey_;
            ServerRuleTypeEnum = _serverRuleTypeEnum_;
            ServerRuleApplyTimeEnum = _serverRuleApplyTimeEnum_;
            Enabled = _enabled_;
            DefaultRule = _defaultRule_;
            ExemptRule = _exemptRule_;
            RuleXml = _ruleXml_;
        }
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="ServerRule", ColumnName="RuleName")]
        public String RuleName
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerRule", ColumnName="ServerPartitionGUID")]
        public ServerEntityKey ServerPartitionKey
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerRule", ColumnName="ServerRuleTypeEnum")]
        public ServerRuleTypeEnum ServerRuleTypeEnum
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerRule", ColumnName="ServerRuleApplyTimeEnum")]
        public ServerRuleApplyTimeEnum ServerRuleApplyTimeEnum
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerRule", ColumnName="Enabled")]
        public Boolean Enabled
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerRule", ColumnName="DefaultRule")]
        public Boolean DefaultRule
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerRule", ColumnName="ExemptRule")]
        public Boolean ExemptRule
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerRule", ColumnName="RuleXml")]
        public XmlDocument RuleXml
        { get; set; }
        #endregion

        #region Static Methods
        static public ServerRule Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public ServerRule Load(IPersistenceContext read, ServerEntityKey key)
        {
            IServerRuleEntityBroker broker = read.GetBroker<IServerRuleEntityBroker>();
            ServerRule theObject = broker.Load(key);
            return theObject;
        }
        static public ServerRule Insert(ServerRule entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ServerRule newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public ServerRule Insert(IUpdateContext update, ServerRule entity)
        {
            IServerRuleEntityBroker broker = update.GetBroker<IServerRuleEntityBroker>();
            ServerRuleUpdateColumns updateColumns = new ServerRuleUpdateColumns();
            updateColumns.RuleName = entity.RuleName;
            updateColumns.ServerPartitionKey = entity.ServerPartitionKey;
            updateColumns.ServerRuleTypeEnum = entity.ServerRuleTypeEnum;
            updateColumns.ServerRuleApplyTimeEnum = entity.ServerRuleApplyTimeEnum;
            updateColumns.Enabled = entity.Enabled;
            updateColumns.DefaultRule = entity.DefaultRule;
            updateColumns.ExemptRule = entity.ExemptRule;
            updateColumns.RuleXml = entity.RuleXml;
            ServerRule newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
