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

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
    [RisApplicationService]
    [ServiceContract]
    public interface ICannedTextService
    {
		/// <summary>
		/// List all the canned text subscribe by the current user.
		/// </summary>
		/// <param name="request"><see cref="ListCannedTextForUserRequest"/></param>
		/// <returns><see cref="ListCannedTextForUserResponse"/></returns>
		[OperationContract]
		ListCannedTextForUserResponse ListCannedTextForUser(ListCannedTextForUserRequest request);

		/// <summary>
		/// Loads all form data needed to edit a canned text.
		/// </summary>
		/// <param name="request"><see cref="GetCannedTextEditFormDataRequest"/></param>
		/// <returns><see cref="GetCannedTextEditFormDataResponse"/></returns>
		[OperationContract]
		GetCannedTextEditFormDataResponse GetCannedTextEditFormData(GetCannedTextEditFormDataRequest request);

		/// <summary>
		/// Load details for a specified canned text.
		/// </summary>
		/// <param name="request"><see cref="LoadCannedTextForEditRequest"/></param>
		/// <returns><see cref="LoadCannedTextForEditResponse"/></returns>
		[OperationContract]
		LoadCannedTextForEditResponse LoadCannedTextForEdit(LoadCannedTextForEditRequest request);

		/// <summary>
		/// Adds a new canned text.
		/// </summary>
		/// <param name="request"><see cref="AddCannedTextRequest"/></param>
		/// <returns><see cref="AddCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddCannedTextResponse AddCannedText(AddCannedTextRequest request);

		/// <summary>
		/// Updates an existing canned text.
		/// </summary>
		/// <param name="request"><see cref="UpdateCannedTextRequest"/></param>
		/// <returns><see cref="UpdateCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UpdateCannedTextResponse UpdateCannedText(UpdateCannedTextRequest request);

		/// <summary>
		/// Deletes an existing canned text.
		/// </summary>
		/// <param name="request"><see cref="DeleteCannedTextRequest"/></param>
		/// <returns><see cref="DeleteCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteCannedTextResponse DeleteCannedText(DeleteCannedTextRequest request);

		/// <summary>
		/// Modifies the category of a set of existing canned texts.
		/// </summary>
		/// <param name="request"><see cref="EditCannedTextCategoriesRequest"/></param>
		/// <returns><see cref="EditCannedTextCategoriesResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		EditCannedTextCategoriesResponse EditCannedTextCategories(EditCannedTextCategoriesRequest request);
	}
}
