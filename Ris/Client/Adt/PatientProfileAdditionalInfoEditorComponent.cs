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

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientProfileAdditionalInfoEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientProfileAdditionalInfoEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientProfileAdditionalInfoEditorComponent class
    /// </summary>
    [AssociateView(typeof(PatientProfileAdditionalInfoEditorComponentViewExtensionPoint))]
    public class PatientProfileAdditionalInfoEditorComponent : ApplicationComponent
    {
        private PatientProfileDetail _profile;

        private IList<EnumValueInfo> _religionChoices;
        private IList<EnumValueInfo> _languageChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfileAdditionalInfoEditorComponent(IList<EnumValueInfo> religionChoices, IList<EnumValueInfo> languageChoices)
        {
            _languageChoices = languageChoices;
            _religionChoices = religionChoices;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public PatientProfileDetail Subject
        {
            get { return _profile; }
            set { _profile = value; }
        }

        #region Presentation Model

        public string Religion
        {
            get { return _profile.Religion.Value; }
            set
            {
                _profile.Religion = EnumValueUtils.MapDisplayValue(_religionChoices, value);
                this.Modified = true;
            }
        }

        public IList<string> ReligionChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_religionChoices); }
        }

        public string Language
        {
            get { return _profile.PrimaryLanguage.Value; }
            set
            {
                _profile.PrimaryLanguage = EnumValueUtils.MapDisplayValue(_languageChoices, value);
                this.Modified = true;
            }
        }

        public IList<string> LanguageChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_languageChoices); }
        }

        #endregion
    }
}
