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
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
    public class Rule
    {
        #region Private Members
        private ISpecification _conditions;
        private IActionSet<ServerActionContext> _actions;
        private readonly ServerRule _serverRule;
        #endregion

        #region Constructors
        public Rule(ServerRule serverRule)
        {
            _serverRule = serverRule;
        }
        #endregion

        #region Public Properties
        public ServerRule ServerRule
        {
            get { return _serverRule; }
        }
        #endregion

        #region Public Methods

        public void Compile(XmlSpecificationCompiler specCompiler, XmlActionCompiler<ServerActionContext> actionCompiler)
        {
            XmlNode ruleNode =
                CollectionUtils.SelectFirst<XmlNode>(_serverRule.RuleXml.ChildNodes,
                                                     delegate(XmlNode child)
                                                         { return child.Name.Equals("rule"); });

            XmlNode conditionNode =
                CollectionUtils.SelectFirst<XmlNode>(ruleNode.ChildNodes,
                                                     delegate(XmlNode child)
                                                     { return child.Name.Equals("condition"); });

            _conditions = specCompiler.Compile(conditionNode as XmlElement);

            XmlNode actionNode =
                CollectionUtils.SelectFirst<XmlNode>(ruleNode.ChildNodes,
                                                     delegate(XmlNode child)
                                                     { return child.Name.Equals("action"); });


            _actions = actionCompiler.Compile(actionNode as XmlElement);
        }

        public void Execute(ServerActionContext context, bool defaultRule, out bool ruleApplied, out bool ruleSuccess)
        {
            ruleApplied = false;
            ruleSuccess = true;

            TestResult result;
            if (defaultRule) // just skip the evaluation
                result = new TestResult(true);
            else
                result = _conditions.Test(context.Message);
            
            if (result.Success)
            {
                ruleApplied = true;
                TestResult actionResult = _actions.Execute(context);
                if (actionResult.Fail)
                {
                    foreach (TestResultReason reason in actionResult.Reasons)
                    {
                        Platform.Log(LogLevel.Error, "Unexpected error performing action: {0}", reason.Message);
                    }
                    ruleSuccess = false;
                }
            }
        }

        #endregion

        #region Static Public Methods

        /// <summary>
        /// Method for validating proper format of a ServerRule.
        /// </summary>
        /// <param name="rule">The rule to validate</param>
        /// <returns>true on successful validation, otherwise false.</returns>
        public static bool ValidateRule(XmlDocument rule)
        {
            XmlSpecificationCompiler specCompiler = new XmlSpecificationCompiler("dicom");
            XmlActionCompiler<ServerActionContext> actionCompiler = new XmlActionCompiler<ServerActionContext>();

            ServerRule theServerRule = new ServerRule();
            theServerRule.RuleXml = rule;

            Rule theRule = new Rule(theServerRule);
            try
            {
                theRule.Compile(specCompiler, actionCompiler);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
