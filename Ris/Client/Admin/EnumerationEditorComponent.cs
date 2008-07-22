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
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="EnumerationEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EnumerationEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EnumerationEditorComponent class
    /// </summary>
    [AssociateView(typeof(EnumerationEditorComponentViewExtensionPoint))]
    public class EnumerationEditorComponent : ApplicationComponent
    {
        private readonly object InsertAtBeginning = new object();

        private EnumValueInfo _enumValue;
        private IList<EnumValueInfo> _otherValues;
        private EnumValueInfo _insertAfter;

        private bool _isNew;
        private string _enumerationClassName;
        
        /// <summary>
        /// Constructor for add.
        /// </summary>
        public EnumerationEditorComponent(string enumerationClassName, IList<EnumValueInfo> allValues)
        {
            _isNew = true;
            _enumerationClassName = enumerationClassName;
            _enumValue = new EnumValueInfo();
            _otherValues = allValues;

            // default insertAfter to last value in list
            _insertAfter = _otherValues.Count == 0 ? null : _otherValues[_otherValues.Count - 1];
        }

        /// <summary>
        /// Constructor for edit.
        /// </summary>
        /// <param name="enumerationName"></param>
        /// <param name="value"></param>
        /// <param name="allValues"></param>
        public EnumerationEditorComponent(string enumerationName, EnumValueInfo value, IList<EnumValueInfo> allValues)
        {
            _isNew = false;
            _enumerationClassName = enumerationName;
            _enumValue = value;
            _otherValues = CollectionUtils.Reject<EnumValueInfo>(allValues, delegate(EnumValueInfo v) { return v.Equals(value); });

            int index = allValues.IndexOf(value);
            _insertAfter = index > 0 ? allValues[index - 1] : null;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public EnumValueInfo EnumValue
        {
            get { return _enumValue; }
        }

        #region Presentation Model

        public bool IsCodeReadOnly
        {
            get { return !_isNew; }
        }

        [ValidateNotNull]
        [ValidateRegex(@"^\w+$", Message = "MessageEnumCodeContainsInvalidChars", AllowNull = true)]
        public string Code
        {
            get { return _enumValue.Code; }
            set
            {
                _enumValue.Code = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string DisplayValue
        {
            get { return _enumValue.Value; }
            set
            {
                _enumValue.Value = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _enumValue.Description; }
            set
            {
                _enumValue.Description = value;
                this.Modified = true;
            }
        }

        public IList InsertAfterChoices
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(InsertAtBeginning);
                foreach (EnumValueInfo info in _otherValues)
                {
                    list.Add(info);
                }
                return list;
            }
        }

        public string FormatInsertAfterChoice(object item)
        {
            if (item == InsertAtBeginning)
                return "(Insert at beginning)";

            EnumValueInfo info = (EnumValueInfo)item;
            return string.Format("{0} - {1}", info.Code, info.Value);
        }

        public object InsertAfter
        {
            get { return _insertAfter ?? InsertAtBeginning; }
            set
            {
                EnumValueInfo choice = value == InsertAtBeginning ? null : (EnumValueInfo)value;
                if(!Equals(choice, _insertAfter))
                {
                    _insertAfter = choice;
                    this.Modified = true;
                }
            }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            try
            {
                Platform.GetService<IEnumerationAdminService>(
                    delegate(IEnumerationAdminService service)
                    {
                        if (_isNew)
                        {
                            service.AddValue(new AddValueRequest(_enumerationClassName, _enumValue, _insertAfter, false));
                        }
                        else
                        {
                            service.EditValue(new EditValueRequest(_enumerationClassName, _enumValue, _insertAfter, false));
                        }

                    });

                Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, _isNew ? SR.ExceptionEnumValueAdd : SR.ExceptionEnumValueUpdate, this.Host.DesktopWindow,
                    delegate()
                    {
                        Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        public void Cancel()
        {
            Exit(ApplicationComponentExitCode.None);
        }

        #endregion

    }
}
