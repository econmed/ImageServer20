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

using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;
using System;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Abstract base class for all update parameter classes used in a non-procedural update broker implementing the <see cref="IUpdateBroker"/> interface
    /// </summary>
    public abstract class UpdateBrokerParameterBase 
    {
        #region Protected Members
        protected string _fieldName;
        protected object _value;
        private Dictionary<string, UpdateBrokerParameterBase> _subParameters = new Dictionary<string, UpdateBrokerParameterBase>();

        #endregion  Protected Members

        #region Public Propertieset
        /// <summary>
        /// Returns the list of sub-parameters
        /// </summary>
        public IDictionary<string, UpdateBrokerParameterBase> SubParameters
        {
            get { return _subParameters; }
        }
        
        /// <summary>
        /// Gets the key corresponding to the parameter/field to be updated.
        /// </summary>
        public String FieldName
        {
            get { return _fieldName; }
        }

        /// <summary>
        /// Gets the value of the parameter/field.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        public virtual bool IsEmpty
        {
            get
            {
                foreach (UpdateBrokerParameterBase subParams in _subParameters.Values)
                {
                    if (!subParams.IsEmpty)
                        return false;
                }
                return true;
            }
        }


        #endregion Public Properties


        #region Constructors
        protected UpdateBrokerParameterBase(String fieldName)
        {
            _fieldName = fieldName;

        }

        #endregion Constructors

    }

}
