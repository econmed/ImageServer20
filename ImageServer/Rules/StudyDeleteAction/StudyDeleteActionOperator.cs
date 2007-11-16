#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;

namespace ClearCanvas.ImageServer.Rules.StudyDeleteAction
{
    [ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ServerActionContext>))]
    public class StudyDeleteActionOperator : IXmlActionCompilerOperator<ServerActionContext>
    {
        public string OperatorTag
        {
            get { return "study-delete"; }
        }

        public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
        {
            if (xmlNode.Attributes["time"] == null)
                throw new XmlActionCompilerException("Unexpected missing time attribute for study-delete action");
            if (xmlNode.Attributes["timeUnits"] == null)
                throw new XmlActionCompilerException("Unexpected missing timeUnits attribute for study-delete action");

            double time;
            if (false == double.TryParse(xmlNode.Attributes["time"].Value, out time))
                throw new XmlActionCompilerException("Unable to parse time value for study-delete rule");
            
            string timeUnits = xmlNode.Attributes["timeUnits"].Value;

            return new StudyDeleteActionItem(time, timeUnits);
        }
    }
}
