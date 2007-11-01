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

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    [ExtensionPoint]
    public class TechnologistContainerFolderExtensionPoint : ExtensionPoint<IContainerFolder>
    {
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistScheduledContainerFolder : ContainerFolder
    {
        public TechnologistScheduledContainerFolder()
            : base("Scheduled", typeof(ScheduledTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistCheckedInContainerFolder : ContainerFolder
    {
        public TechnologistCheckedInContainerFolder() 
            : base("Checked In", typeof(CheckedInTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistInProgressContainerFolder : ContainerFolder
    {
        public TechnologistInProgressContainerFolder()
            : base("In Progress", typeof(InProgressTechnologistWorkflowFolder)) { }
    }

    //[ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistSuspendedContainerFolder : ContainerFolder
    {
        public TechnologistSuspendedContainerFolder()
            : base("Suspended", typeof(SuspendedTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistCompletedContainerFolder : ContainerFolder
    {
        public TechnologistCompletedContainerFolder() 
            : base("Completed", typeof(CompletedTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistCancelledContainerFolder : ContainerFolder
    {
        public TechnologistCancelledContainerFolder() 
            : base("Cancelled", typeof(CancelledTechnologistWorkflowFolder)) { }
    }

    [ExtensionOf(typeof(TechnologistContainerFolderExtensionPoint))]
    public class TechnologistUndocumentedContainerFolder : ContainerFolder
    {
        public TechnologistUndocumentedContainerFolder()
            : base("Undocumented", typeof(UndocumentedTechnologistWorkflowFolder)) { }
    }
}
