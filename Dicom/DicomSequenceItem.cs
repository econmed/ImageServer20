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

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// A class representing a DICOM Sequence Item.
    /// </summary>
    public class DicomSequenceItem : DicomAttributeCollection
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DicomSequenceItem() : base(0x00000000,0xFFFFFFFF)
        {
        }

        /// <summary>
        /// Internal constructor used when making a copy of a <see cref="DicomAttributeCollection"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="copyBinary"></param>
        internal DicomSequenceItem(DicomAttributeCollection source, bool copyBinary)
            : base(source, copyBinary)
        {
        }
        #endregion

        #region Public Overridden Methods
        /// <summary>
        /// Create a copy of this DicomSequenceItem.
        /// </summary>
        /// <returns>The copied DicomSequenceItem.</returns>
        public override DicomAttributeCollection Copy()
        {
            return Copy(true);
        }

        /// <summary>
        /// Creates a copy of this DicomSequenceItem.
        /// </summary>
        /// <param name="copyBinary">When set to false, the copy will not include <see cref="DicomAttribute"/>
        /// instances that are of type <see cref="DicomAttributeOB"/>, <see cref="DicomAttributeOW"/>,
        /// or <see cref="DicomAttributeOF"/>.</param>
        /// <returns>The copied DicomSequenceItem.</returns>
        public override DicomAttributeCollection Copy(bool copyBinary)
        {
            return new DicomSequenceItem(this,copyBinary);
        }
        #endregion

    }
    #region DirectoryRecordSequenceItem Class
    internal class DirectoryRecordSequenceItem : DicomSequenceItem
    {
        private DirectoryRecordSequenceItem _lowerLevelRecord;
        private DirectoryRecordSequenceItem _nextRecord;
        private uint _offset;

        public DirectoryRecordSequenceItem LowerLevelRecord
        {
            get { return _lowerLevelRecord; }
            set { _lowerLevelRecord = value; }
        }
        public DirectoryRecordSequenceItem NextRecord
        {
            get { return _nextRecord; }
            set { _nextRecord = value; }
        }

        public uint Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public override string ToString()
        {
            string toString = String.Empty;
            string recordType = base[DicomTags.DirectoryRecordType].GetString(0, "");
            if (recordType == DicomDirectoryWriter.DirectoryRecordTypeImage)
                toString = base[DicomTags.ReferencedSopInstanceUidInFile].GetString(0, "");
            else if (recordType == DicomDirectoryWriter.DirectoryRecordTypeSeries)
                toString = base[DicomTags.SeriesInstanceUid].GetString(0, "");
            else if (recordType == DicomDirectoryWriter.DirectoryRecordTypeStudy)
                toString = base[DicomTags.StudyInstanceUid].GetString(0, "");
            else if (recordType == DicomDirectoryWriter.DirectoryRecordTypePatient)
                toString = base[DicomTags.PatientId].GetString(0, "") + " " + base[DicomTags.PatientsName].GetString(0, "");

            return recordType + " " + toString;
        }
    }
    #endregion

}
