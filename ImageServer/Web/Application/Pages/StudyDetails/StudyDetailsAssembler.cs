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

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails
{
    /// <summary>
    /// Assembles an instance of  <see cref="StudyDetails"/> based on a <see cref="Study"/> object.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    static public class StudyDetailsAssembler
    {
        /// <summary>
        /// Returns an instance of <see cref="StudyDetails"/> based on a <see cref="Study"/> object.
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        static public StudyDetails CreateStudyDetails(Model.Study study)
        {
            Platform.CheckForNullReference(study, "study");

            StudyDetails details = new StudyDetails();

            details.AccessionNumber = study.AccessionNumber;
            details.NumberOfStudyRelatedInstances = study.NumberOfStudyRelatedInstances;
            details.NumberOfStudyRelatedSeries = study.NumberOfStudyRelatedSeries;
            details.ReferringPhysicianName = study.ReferringPhysiciansName;

            details.Status = study.StudyStatusEnum.ToString();
            details.StudyDate = study.StudyDate;
            details.StudyDescription = study.StudyDescription;
            details.StudyId = study.StudyId;
            details.StudyInstanceUid = study.StudyInstanceUid;
            details.StudyTime = study.StudyTime;

            StudyController studyController = new StudyController();
            details.ScheduledForDelete = studyController.IsScheduledForDelete(study);

            return details;
        }

    }
}
