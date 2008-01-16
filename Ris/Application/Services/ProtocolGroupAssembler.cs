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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProtocolGroupAssembler
    {
        public ProtocolGroupSummary GetProtocolGroupSummary(ProtocolGroup protocolGroup)
        {
            return new ProtocolGroupSummary(protocolGroup.GetRef(), protocolGroup.Name, protocolGroup.Description);
        }

        public ProtocolGroupDetail GetProtocolGroupDetail(ProtocolGroup group, IPersistenceContext context)
        {
            List<ProtocolCodeDetail> codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeDetail>(
                group.Codes,
                delegate(ProtocolCode code) { return new ProtocolCodeDetail(code.GetRef(), code.Name, code.Description);});

            ProcedureTypeGroupAssembler assembler = new ProcedureTypeGroupAssembler();
            List<ProcedureTypeGroupSummary> readingGroups = CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
                group.ReadingGroups,
                delegate(ProcedureTypeGroup readingGroup) { return assembler.GetProcedureTypeGroupSummary(readingGroup, context); });
            
            return new ProtocolGroupDetail(group.Name, group.Description, codes, readingGroups);
        }

        public void UpdateProtocolGroup(ProtocolGroup group, ProtocolGroupDetail detail, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;

            group.Codes.Clear();
            detail.Codes.ForEach(delegate(ProtocolCodeDetail protocolCodeDetail)
            {
                group.Codes.Add(context.Load<ProtocolCode>(protocolCodeDetail.EntityRef));
            });

            group.ReadingGroups.Clear();
            detail.ReadingGroups.ForEach(delegate(ProcedureTypeGroupSummary procedureTypeGroupSummary)
            {
                group.ReadingGroups.Add(context.Load<ProcedureTypeGroup>(procedureTypeGroupSummary.EntityRef));
            });
        }
    }
}
