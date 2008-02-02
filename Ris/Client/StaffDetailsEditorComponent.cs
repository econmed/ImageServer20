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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="StaffDetailsEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffDetailsEditorComponent class
    /// </summary>
    [AssociateView(typeof(StaffDetailsEditorComponentViewExtensionPoint))]
    public class StaffDetailsEditorComponent : ApplicationComponent
    {
        private StaffDetail _staffDetail;
        private bool _isNew;
        private IList<EnumValueInfo> _staffTypeChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffDetailsEditorComponent(bool isNew, IList<EnumValueInfo> staffTypeChoices)
        {
            _staffDetail = new StaffDetail();
            _isNew = isNew;
            _staffTypeChoices = staffTypeChoices;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public StaffDetail StaffDetail
        {
            get { return _staffDetail; }
            set 
            { 
                _staffDetail = value;
            }
        }

        #region Presentation Model

        public string StaffType
        {
            get { return _staffDetail.StaffType.Value; }
            set
            {
                _staffDetail.StaffType = EnumValueUtils.MapDisplayValue(_staffTypeChoices, value);

                this.Modified = true;
            }
        }

        public List<string> StaffTypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_staffTypeChoices); }
        }

        public string StaffId
        {
            get { return _staffDetail.StaffId; }
            set
            {
                _staffDetail.StaffId = value;
                this.Modified = true;
            }
        }

        public string FamilyName
        {
            get { return _staffDetail.Name.FamilyName; }
            set 
            {
                _staffDetail.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        public string GivenName
        {
            get { return _staffDetail.Name.GivenName; }
            set
            {
                _staffDetail.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _staffDetail.Name.MiddleName; }
            set
            {
                _staffDetail.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Prefix
        {
            get { return _staffDetail.Name.Prefix; }
            set
            {
                _staffDetail.Name.Prefix = value;
                this.Modified = true;
            }
        }

        public string Suffix
        {
            get { return _staffDetail.Name.Suffix; }
            set
            {
                _staffDetail.Name.Suffix = value;
                this.Modified = true;
            }
        }

        public string Degree
        {
            get { return _staffDetail.Name.Degree; }
            set
            {
                _staffDetail.Name.Degree = value;
                this.Modified = true;
            }
        }

        public string LicenseNumber
        {
            get { return _staffDetail.LicenseNumber; }
            set
            {
                _staffDetail.LicenseNumber = value;
                this.Modified = true;
            }
        }

        public string BillingNumber
        {
            get { return _staffDetail.BillingNumber; }
            set
            {
                _staffDetail.BillingNumber = value;
                this.Modified = true;
            }
        }

        #endregion
    }
}
