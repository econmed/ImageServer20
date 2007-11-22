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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// Search criteria for <see cref="ProcedureStep"/> entity
    /// This file is machine generated - changes will be lost.
    /// </summary>
	public partial class ProcedureStepSearchCriteria : ActivitySearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ProcedureStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ProcedureStepSearchCriteria(string key)
			:base(key)
		{
		}

		
		/// <summary>
		/// Constructor to search by EntityRef
		/// </summary>
		public ProcedureStepSearchCriteria(EntityRef entityRef)
		{
			this.SubCriteria["OID"] = new SearchCondition<object>("OID");
            ((ISearchCondition<object>)this.SubCriteria["OID"]).EqualTo(EntityRefUtils.GetOID(entityRef));
		}

        public new ProcedureStepSchedulingSearchCriteria Scheduling
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Scheduling"))
                {
                    this.SubCriteria["Scheduling"] = new ProcedureStepSchedulingSearchCriteria("Scheduling");
                }
                return (ProcedureStepSchedulingSearchCriteria)this.SubCriteria["Scheduling"];
            }
        }

        public ISearchCondition<RequestedProcedure> Procedure
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Procedure"))
	  			{
	  				this.SubCriteria["Procedure"] = new SearchCondition<RequestedProcedure>("Procedure");
	  			}
	  			return (ISearchCondition<RequestedProcedure>)this.SubCriteria["Procedure"];
	  		}
	  	}

        public ProcedureStepPerformerSearchCriteria Performer
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Performer"))
                {
                    this.SubCriteria["Performer"] = new ProcedureStepPerformerSearchCriteria("Performer");
                }
                return (ProcedureStepPerformerSearchCriteria)this.SubCriteria["Procedure"];
            }
        }

	}
}
