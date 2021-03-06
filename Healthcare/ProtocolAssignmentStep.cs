﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{

    [ExtensionOf(typeof(ProcedureStepBuilderExtensionPoint))]
    public class ProtocolAssignmentStepBuilder : ProcedureStepBuilderBase
    {

        public override Type ProcedureStepClass
        {
            get { return typeof(ProtocolAssignmentStep); }
        }

        public override ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure)
        {
            Protocol protocol = new Protocol(procedure);
            ProtocolAssignmentStep step = new ProtocolAssignmentStep(protocol);

            //note: this is not ideal but there is no other way to save the protocol object
            PersistenceScope.CurrentContext.Lock(protocol, DirtyState.New);

            return step;
        }

        public override void SaveInstance(ProcedureStep prototype, XmlElement xmlNode)
        {
            // nothing to do
        }
    }

    /// <summary>
    /// ProtocolAssignmentStep entity
    /// </summary>
    public partial class ProtocolAssignmentStep : ProtocolProcedureStep
    {
        public ProtocolAssignmentStep(Protocol protocol)
            : base(protocol)
        {
        }

        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        public virtual bool CanAccept
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanReject
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanSuspend
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanSave
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanApprove
        {
            get { return (this.State == ActivityStatus.SC || this.State == ActivityStatus.IP) && this.Protocol.Status == ProtocolStatus.AA; }
        }

        public bool CanEdit(Staff staff)
        {
            return this.State == ActivityStatus.IP && this.PerformingStaff == staff;
        }

        public override string Name
        {
            get { return "Protocol Assignment"; }
        }

		protected override void LinkProcedure(Procedure procedure)
		{
			if (this.Protocol == null)
				throw new WorkflowException("This step must be associated with a Protocol before procedures can be linked.");

			this.Protocol.LinkProcedure(procedure);
		}

        protected override ProcedureStep CreateScheduledCopy()
        {
            ProtocolAssignmentStep newStep = new ProtocolAssignmentStep(this.Protocol);
            this.Procedure.AddProcedureStep(newStep);
            return newStep;
        }
    }
}