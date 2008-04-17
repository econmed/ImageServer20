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
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderNoteDetail : DataContractBase
    {
        [DataContract]
        public abstract class RecipientDetail : DataContractBase
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="isAcknowledged"></param>
            /// <param name="acknowledgedTime"></param>
            protected RecipientDetail(bool isAcknowledged, DateTime? acknowledgedTime)
            {
                IsAcknowledged = isAcknowledged;
                AcknowledgedTime = acknowledgedTime;
            }

            /// <summary>
            /// Gets a value indicating whether the note has been acknowledged by the recipient.
            /// (For a group recipient, indicates whether a member of the group has acknowledged the note on behalf of the group).
            /// This field is ignored when creating a new note.
            /// </summary>
            [DataMember]
            public bool IsAcknowledged;

            /// <summary>
            /// Gets the time that the note was acknowledged, or null if it was not acknowledged.
            /// (For a group recipient, gets the time when a member of the group acknowledged the note on behalf of the group).
            /// This field is ignored when creating a new note.
            /// </summary>
            [DataMember]
            public DateTime? AcknowledgedTime;
        }

        [DataContract]
        public class StaffRecipientDetail : RecipientDetail
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="staff"></param>
            /// <param name="isAcknowledged"></param>
            /// <param name="acknowledgedTime"></param>
            public StaffRecipientDetail(StaffSummary staff, bool isAcknowledged, DateTime? acknowledgedTime)
                : base(isAcknowledged, acknowledgedTime)
            {
                Staff = staff;
            }

            /// <summary>
            /// Gets the staff recipient.
            /// </summary>
            [DataMember]
            public StaffSummary Staff;
        }

        [DataContract]
        public class GroupRecipientDetail : RecipientDetail
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="isAcknowledged"></param>
            /// <param name="acknowledgedTime"></param>
            /// <param name="group"></param>
            public GroupRecipientDetail(StaffGroupSummary group, bool isAcknowledged, DateTime? acknowledgedTime)
                : base(isAcknowledged, acknowledgedTime)
            {
                Group = group;
            }

            /// <summary>
            /// Gets the group recipient.
            /// </summary>
            [DataMember]
            public StaffGroupSummary Group;
        }

        /// <summary>
        /// Constructor for creating detail for an existing order note.
        /// </summary>
        /// <param name="orderNoteRef"></param>
        /// <param name="category"></param>
        /// <param name="creationTime"></param>
        /// <param name="postTime"></param>
        /// <param name="author"></param>
        /// <param name="staffRecipients"></param>
        /// <param name="groupRecipients"></param>
        /// <param name="noteBody"></param>
        public OrderNoteDetail(EntityRef orderNoteRef, string category, DateTime creationTime, DateTime? postTime, StaffSummary author, List<StaffRecipientDetail> staffRecipients, List<GroupRecipientDetail> groupRecipients, string noteBody)
        {
            OrderNoteRef = orderNoteRef;
            Category = category;
            CreationTime = creationTime;
            PostTime = postTime;
            Author = author;
            StaffRecipients = staffRecipients;
            GroupRecipients = groupRecipients;
            NoteBody = noteBody;
        }

        /// <summary>
        /// Constructor for generating a new order note.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="noteBody"></param>
        /// <param name="staffRecipients"></param>
        /// <param name="groupRecipients"></param>
        public OrderNoteDetail(string category, string noteBody, List<StaffRecipientDetail> staffRecipients, List<GroupRecipientDetail> groupRecipients)
        {
            Category = category;
            NoteBody = noteBody;
            StaffRecipients = staffRecipients;
            GroupRecipients = groupRecipients;
        }

        /// <summary>
        /// Gets a reference to the order note.
        /// This field is ignored when creating a new note.
        /// </summary>
        [DataMember]
        public EntityRef OrderNoteRef;

        /// <summary>
        /// Gets the category of the note.
        /// </summary>
        [DataMember]
        public string Category;

        /// <summary>
        /// Gets the time the note was created.
        /// This field is ignored when creating a new note.
        /// </summary>
        [DataMember]
        public DateTime CreationTime;

        /// <summary>
        /// Gets the time the note was posted.
        /// This field is ignored when creating a new note.
        /// </summary>
        [DataMember]
        public DateTime? PostTime;

        /// <summary>
        /// Gets the note author.
        /// This field is ignored when creating a new note.
        /// </summary>
        [DataMember]
        public StaffSummary Author;

        /// <summary>
        /// Gets the note body text.
        /// </summary>
        [DataMember]
        public string NoteBody;

        /// <summary>
        /// Gets the list of staff recipients.
        /// </summary>
        [DataMember]
        public List<StaffRecipientDetail> StaffRecipients;

        /// <summary>
        /// Gets the list of group recipients.
        /// </summary>
        [DataMember]
        public List<GroupRecipientDetail> GroupRecipients;

    }
}
