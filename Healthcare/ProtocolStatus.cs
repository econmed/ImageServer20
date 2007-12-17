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

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ProtocolStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(ProtocolStatusEnum))]
	public enum ProtocolStatus
	{
        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending", Description = "Protocol is pending")]
        PN,

        /// <summary>
        /// Protocolled
        /// </summary>
        [EnumValue("Protocolled", Description = "Protocol assigned and order accepted")]
        PR,

        /// <summary>
        /// Protocolled
        /// </summary>
        [EnumValue("Rejected", Description = "Protocol assigned and order rejected")]
        RJ,

        /// <summary>
        /// Protocolled
        /// </summary>
        [EnumValue("Suspended", Description = "Protocol suspended pending further order information")]
        SU,

        /// <summary>
        /// Awaiting Approval
        /// </summary>
        [EnumValue("Awaiting Approval", Description = "Protocol submitted for approval by resident")]
        AA
    }
}