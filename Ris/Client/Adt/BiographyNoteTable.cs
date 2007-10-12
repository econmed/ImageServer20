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
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public class BiographyNoteTable : DecoratedTable<NoteDetail>
    {
        private static readonly uint NumRows = 2;
        private static readonly uint NoteCommentRow = 1;

        public BiographyNoteTable()
            : this(NumRows)
        {
        }

        private BiographyNoteTable(uint cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<NoteDetail, string>("Severity",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Severity.Code); }, 0.05f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Category",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Name); }, 0.2f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Description",
                delegate(NoteDetail n) { return (n.Category == null ? "" : n.Category.Description); }, 0.4f));
            this.Columns.Add(new TableColumn<NoteDetail, string>("Created By",
                delegate(NoteDetail n) { return String.Format("{0}, {1}", n.CreatedBy.Name.FamilyName, n.CreatedBy.Name.GivenName); }, 0.2f));
            this.Columns.Add(new TableColumn<NoteDetail, string>(SR.ColumnCreatedOn,
                delegate(NoteDetail n) { return Format.Date(n.TimeStamp); }, 0.1f));
            this.Columns.Add(new DecoratedTableColumn<NoteDetail, string>("Comment",
                delegate(NoteDetail n) { return (n.Comment != null && n.Comment.Length > 0 ? String.Format("Comment: {0}", n.Comment) : ""); }, 0.1f, NoteCommentRow));
        }
    }
}
