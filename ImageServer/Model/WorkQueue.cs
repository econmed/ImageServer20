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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class WorkQueue
    {
        #region Private Members
        protected StudyStorage _studyStorage;
        private Study _study;
        #endregion

        

        private void LoadRelatedEntities()
        {
            if (_study==null || _studyStorage==null)
            {
                using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    lock (SyncRoot)
                    {
                        if (_study == null)
                            _study = LoadStudy(context);

                        if (_studyStorage == null)
                            _studyStorage = LoadStudyStorage(context);
                    }

                }    
            }
            
        }


        /// <summary>
        /// Delete the Work Queue record from the system.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Delete(IPersistenceContext context)
        {
            IWorkQueueUidEntityBroker workQueueUidBroker = context.GetBroker<IWorkQueueUidEntityBroker>();
            WorkQueueUidSelectCriteria criteria = new WorkQueueUidSelectCriteria();
            criteria.WorkQueueKey.EqualTo(GetKey());
            workQueueUidBroker.Delete(criteria);

            IWorkQueueEntityBroker workQueueBroker = context.GetBroker<IWorkQueueEntityBroker>();
            return workQueueBroker.Delete(GetKey());
        }

        /// <summary>
        /// Loads the related <see cref="Study"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Study LoadStudy(IPersistenceContext context)
        {
            if (_study == null)
            {
                lock (SyncRoot)
                {
                    _study = Study.Find(context, StudyStorageKey);
                }
            }
            return _study;
        }

        /// <summary>
        /// Loads the related <see cref="StudyStorage"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private StudyStorage LoadStudyStorage(IPersistenceContext context)
        {
            if (_studyStorage==null)
            {
                lock (SyncRoot)
                {
                    _studyStorage = StudyStorage.Load(context, StudyStorageKey); 
                }
            }
            return _studyStorage;
        }

        public IList<StudyStorageLocation> LoadStudyLocations(IPersistenceContext context)
        {
            StudyStorage storage = LoadStudyStorage(context);
            return StudyStorageLocation.FindStorageLocations(context, storage);
        }

        public StudyStorage StudyStorage
        {
            get
            {
                LoadRelatedEntities();
                return _studyStorage;
            }
        }

        public Study Study
        {
            get
            {
                LoadRelatedEntities();
                return _study;
            }
            set { _study = value; }
        }
    }
}
