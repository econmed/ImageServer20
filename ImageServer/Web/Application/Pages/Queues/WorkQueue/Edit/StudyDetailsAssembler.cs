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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Assembles an instance of  <see cref="StudyDetails"/> based on a <see cref="Study"/> object.
    /// </summary>
    public class StudyDetailsAssembler
    {
        /// <summary>
        /// Creates an instance of <see cref="StudyDetails"/> base on a <see cref="Study"/> object.
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public StudyDetails CreateStudyDetail(Model.Study study)
        {
            StudyDetails details = new StudyDetails();
            details.StudyInstanceUID = study.StudyInstanceUid;
            details.Status = study.StudyStatusEnum.ToString();
            details.PatientName = study.PatientsName;
            details.AccessionNumber = study.AccessionNumber;
            details.PatientID = study.PatientId;

            details.StudyDescription = study.StudyDescription;


            if (study.StudyInstanceUid != null)
            {
                StudyStorageAdaptor adaptor = new StudyStorageAdaptor();
                StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
                criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
                criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

                StudyStorage storages = adaptor.GetFirst(criteria);
                if (storages != null)
                    details.Lock = storages.Lock;
            }


            return details;
        }
    }
}