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
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    /// <summary>
    /// Provides operations to administer facilities
    /// </summary>
    [ServiceContract]
    public interface IFacilityAdminService
    {
        /// <summary>
        /// Summary list of all facilities
        /// </summary>
        /// <param name="request"><see cref="ListAllFacilitiesRequest"/></param>
        /// <returns><see cref="ListAllFacilitiesResponse"/></returns>
        [OperationContract]
        ListAllFacilitiesResponse ListAllFacilities(ListAllFacilitiesRequest request);

        /// <summary>
        /// Add a new facility.  A facility with the same code as an existing facility cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddFacilityRequest"/></param>
        /// <returns><see cref="AddFacilityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddFacilityResponse AddFacility(AddFacilityRequest request);

        /// <summary>
        /// Update a new facility.  A facility with the same code as an existing facility cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateFacilityRequest"/></param>
        /// <returns><see cref="UpdateFacilityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateFacilityResponse UpdateFacility(UpdateFacilityRequest request);

        /// <summary>
        /// Load details for a specified facility
        /// </summary>
        /// <param name="request"><see cref="GetDataForCancelOrderTableRequest"/></param>
        /// <returns><see cref="GetDataForCancelOrderTableResponse"/></returns>
        [OperationContract]
        LoadFacilityForEditResponse LoadFacilityForEdit(LoadFacilityForEditRequest request);
    }
}
