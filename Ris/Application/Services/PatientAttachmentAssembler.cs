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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class PatientAttachmentAssembler
    {
        class PatientAttachmentSynchronizeHelper : SynchronizeHelper<PatientAttachment, PatientAttachmentSummary>
        {
            private readonly PatientAttachmentAssembler _assembler;
            private readonly IPersistenceContext _context;

            public PatientAttachmentSynchronizeHelper(PatientAttachmentAssembler assembler, IPersistenceContext context)
            {
                _assembler = assembler;
                _context = context;

                _allowUpdate = true;
                _allowRemove = true;
            }

            protected override bool CompareItems(PatientAttachment domainItem, PatientAttachmentSummary sourceItem)
            {
                return Equals(domainItem.Document.GetRef(), sourceItem.Document.DocumentRef);
            }

            protected override PatientAttachment CreateDomainItem(PatientAttachmentSummary sourceItem)
            {
                return _assembler.CreatePatientAttachment(sourceItem, _context);
            }

            protected override void UpdateDomainItem(PatientAttachment domainItem, PatientAttachmentSummary sourceItem)
            {
                _assembler.UpdatePatientAttachment(domainItem, sourceItem, _context);
            }
        }

        public void Synchronize(IList<PatientAttachment> domainList, IList<PatientAttachmentSummary> sourceList, IPersistenceContext context)
        {
            PatientAttachmentSynchronizeHelper synchronizer = new PatientAttachmentSynchronizeHelper(this, context);
            synchronizer.Synchronize(domainList, sourceList);
        }

        public PatientAttachmentSummary CreatePatientAttachmentSummary(PatientAttachment attachment)
        {
            MimeDocumentAssembler mimeDocAssembler = new MimeDocumentAssembler();
            return new PatientAttachmentSummary(
                EnumUtils.GetEnumValueInfo(attachment.Category),
                mimeDocAssembler.CreateMimeDocumentSummary(attachment.Document));
        }

        public PatientAttachment CreatePatientAttachment(PatientAttachmentSummary summary, IPersistenceContext context)
        {
            return new PatientAttachment(
                EnumUtils.GetEnumValue<PatientAttachmentCategoryEnum>(summary.Category, context),
                context.Load<MimeDocument>(summary.Document.DocumentRef));
        }

        public void UpdatePatientAttachment(PatientAttachment attachment, PatientAttachmentSummary summary, IPersistenceContext context)
        {
            MimeDocumentAssembler mimeDocAssembler = new MimeDocumentAssembler();
            attachment.Category = EnumUtils.GetEnumValue<PatientAttachmentCategoryEnum>(summary.Category, context);
            mimeDocAssembler.UpdateMimeDocument(attachment.Document, summary.Document);
        }
    }
}
