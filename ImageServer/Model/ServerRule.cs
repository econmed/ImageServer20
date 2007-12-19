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

using System;
using System.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Model
{
    [Serializable]
    public class ServerRule : ServerEntity
    {
        #region Constructors
        public ServerRule()
            : base("ServerRule")
        {
        }
        #endregion

        #region Private Members
        private string _ruleName;
        private ServerEntityKey _serverPartitionKey;
        private ServerRuleTypeEnum _serverRuleTypeEnum;
        private ServerRuleApplyTimeEnum _serverRuleApplyTimeEnum;
        private bool _enabled;
        private bool _defaultRule;
        private XmlDocument _ruleXml;
        #endregion

        #region Public Properties
        public string RuleName
        {
            get { return _ruleName; }
            set { _ruleName = value; }
        }
        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        public ServerRuleTypeEnum ServerRuleTypeEnum
        {
            get { return _serverRuleTypeEnum; }
            set { _serverRuleTypeEnum = value; }
        }
        public ServerRuleApplyTimeEnum ServerRuleApplyTimeEnum
        {
            get { return _serverRuleApplyTimeEnum; }
            set { _serverRuleApplyTimeEnum = value; }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        public bool DefaultRule
        {
            get { return _defaultRule; }
            set { _defaultRule = value; }
        }
        public XmlDocument RuleXml
        {
            get { return _ruleXml; }
            set { _ruleXml = value; }
        }
        #endregion

        #region Static Methods
        static public ServerRule Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public ServerRule Load(IReadContext read, ServerEntityKey key)
        {
            ISelectServerRule broker = read.GetBroker<ISelectServerRule>();
            ServerRule theItem = broker.Load(key);
            return theItem;
        }

        #endregion
    }
}
