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
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    class IncompleteDemographicDataAlert : PatientProfileAlertBase
    {
        public override AlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
            IDictionary<string, ISpecification> specs;

            try
            {
                IncompleteDemographicDataAlertSettings settings = new IncompleteDemographicDataAlertSettings();
                using (TextReader xml = new StringReader(settings.ValidationRules))
                {
                    SpecificationFactory specFactory = new SpecificationFactory(xml);
                    specs = specFactory.GetAllSpecifications();
                }
            }
            catch (Exception)
            {
                // no cfg file for this component
                specs = new Dictionary<string, ISpecification>();
            }
            
            List<string> reasons = new List<string>();
            foreach (KeyValuePair<string, ISpecification> kvp in specs)
            {
                TestResult result = kvp.Value.Test(profile);
                if (result.Success == false)
                {
                    List<string> failureMessages = new List<string>();
                    ExtractFailureMessage(result.Reasons, failureMessages);
                    reasons.AddRange(failureMessages);
                }
            }

            if (reasons.Count > 0)
                return new AlertNotification(this.GetType(), reasons);

            return null;
        }

        #region Private Helpers

        private void ExtractFailureMessage(TestResultReason[] reasons, List<string> failureMessages)
        {
            foreach (TestResultReason reason in reasons)
            {
                if (!string.IsNullOrEmpty(reason.Message))
                    failureMessages.Add(reason.Message);

                ExtractFailureMessage(reason.Reasons, failureMessages);
            }
        }

        #endregion

    }
}
