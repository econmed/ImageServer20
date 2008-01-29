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
using System.ComponentModel;
using System.IO;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Application.ValidationServerProxy;

namespace ClearCanvas.ImageServer.Web.Application.Services
{
    /// <summary>
    /// Provides data validation services
    /// </summary>
    [WebService(Namespace = "http://www.clearcanvas.ca/ImageServer/Services/ValidationServices.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [GenerateScriptType(typeof (ValidationResult))]
    [ScriptService]
    public class ValidationServices : WebService
    {
        /// <summary>
        /// Validate the existence of the specified path on the network.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [WebMethod]
        public ValidationResult ValidateFilesystemPath(string path)
        {
            // This web service in turns call a WCF service which resides on the same or different systems.

            ValidationResult res = new  ValidationResult();

            try
            {
                ValidationServiceClient service = new ValidationServiceClient();
                service.Open();
                res = service.CheckPath(path);
                service.Close();
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Error, "ValidationService ValidateFilesystemPath failed: {0}", ex.StackTrace);

                res.Success = false;
                res.ErrorCode = -5000;
                res.ErrorText = "Validation Service is not available at this time.";
                
            }
            
            return res;
        }

        /// <summary>
        /// Validate a ServerRule for proper formatting.
        /// </summary>
        /// <param name="serverRule">A string representing the rule.</param>
        /// <returns>The result of the validation.</returns>
        [WebMethod]
        public ValidationResult ValidateServerRule(string serverRule)
        {
            ValidationResult result = new ValidationResult();

            if (String.IsNullOrEmpty(serverRule))
            {
                result.ErrorText = "Server Rule XML must be specified";
                result.Success = false;
                result.ErrorCode = -5000;
                return result;
            }

            XmlDocument theDoc = new XmlDocument();

            theDoc.Load(new StringReader(serverRule));

            if (false == ClearCanvas.ImageServer.Rules.Rule.ValidateRule(theDoc))
            {
                result.ErrorText = "Unable to compile Server Rule.";
                result.Success = false;
                result.ErrorCode = -5000;
            }
            else
                result.Success = true;

            return result;
        }
    }
}
