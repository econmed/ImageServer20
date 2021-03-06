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

using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Detailed view of a <see cref="Study"/> in the context of the WorkQueue configuration UI.
    /// </summary>
    /// <remarks>
    /// A <see cref="StudyDetails"/> contains detailed information of a <see cref="Study"/> and related information 
    /// to be displayed within the WorkQueue configuration UI.
    /// <para>
    /// A <see cref="StudyDetails"/> can be created using a <see cref="StudyDetailsAssembler"/> object.
    /// </para>
    /// </remarks>
    /// <seealso cref="WorkQueueDetails"/>
    public class StudyDetails
    {
        #region Public Properties

        public string StudyInstanceUID { get; set; }

        public string Status { get; set; }

        public string PatientName { get; set; }

        public string AccessionNumber { get; set; }

        public string PatientID { get; set; }

        public string StudyDescription { get; set; }

        public string StudyDate { get; set; }

        public string StudyTime { get; set; }

        public string Modalities { get; set; }

        public bool? WriteLock { get; set; }

		public short ReadLock { get; set; }

        #endregion Public Properties
    }
}