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

using System.ServiceModel;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [RisServiceProvider]
    [ServiceContract]
	[ServiceKnownType(typeof(ReportingWorklistItem))]
	public interface IReportingWorkflowService : IWorklistService<ReportingWorklistItem>, IWorkflowService
    {
        /// <summary>
        /// Start an interpretation step
        /// </summary>
        /// <param name="request"><see cref="StartInterpretationRequest"/></param>
        /// <returns><see cref="StartInterpretationResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        StartInterpretationResponse StartInterpretation(StartInterpretationRequest request);

        /// <summary>
        /// Complete an interpretation step and create a transcription step.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationForTranscriptionRequest"/></param>
        /// <returns><see cref="CompleteInterpretationForTranscriptionResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request);

        /// <summary>
        /// Complete an interpretation step and create a verification step.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationForVerificationRequest"/></param>
        /// <returns><see cref="CompleteInterpretationForVerificationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request);

        /// <summary>
        /// Complete an interpretation step and verify it.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationAndVerifyRequest"/></param>
        /// <returns><see cref="CompleteInterpretationAndVerifyResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request);

        /// <summary>
        /// Cancel a reporting step and create a new interpretation step.
        /// </summary>
        /// <param name="request"><see cref="CancelReportingStepRequest"/></param>
        /// <returns><see cref="CancelReportingStepResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        CancelReportingStepResponse CancelReportingStep(CancelReportingStepRequest request);

        /// <summary>
        /// Cancel a verification step and create a new interpretation step with the same report part.
        /// This is used by the resident to revise the report that is currently waiting to be verified by radiologist
        /// </summary>
        /// <param name="request"><see cref="ReviseResidentReportRequest"/></param>
        /// <returns><see cref="ReviseResidentReportResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        ReviseResidentReportResponse ReviseResidentReport(ReviseResidentReportRequest request);

        /// <summary>
        /// Start an verification step
        /// </summary>
        /// <param name="request"><see cref="StartVerificationRequest"/></param>
        /// <returns><see cref="StartVerificationResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        StartVerificationResponse StartVerification(StartVerificationRequest request);

        /// <summary>
        /// Complete a verification step
        /// </summary>
        /// <param name="request"><see cref="CompleteVerificationRequest"/></param>
        /// <returns><see cref="CompleteVerificationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request);

        /// <summary>
        /// Start an addendum step
        /// </summary>
        /// <param name="request"><see cref="CreateAddendumRequest"/></param>
        /// <returns><see cref="CreateAddendumResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        CreateAddendumResponse CreateAddendum(CreateAddendumRequest request);

        /// <summary>
        /// Cancel a publication step and create a new verification step with the same report part.
        /// This is used by the radiologist to revise the report that is still unpublished.
        /// </summary>
        /// <param name="request"><see cref="ReviseUnpublishedReportRequest"/></param>
        /// <returns><see cref="ReviseUnpublishedReportResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        ReviseUnpublishedReportResponse ReviseUnpublishedReport(ReviseUnpublishedReportRequest request);

        /// <summary>
        /// This provide a mean to complete a publication step.  It is meant for testing only. 
        /// </summary>
        /// TODO: to be removed
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        PublishReportResponse PublishReport(PublishReportRequest request);

        /// <summary>
        /// Load the report of a given reporting step
        /// </summary>
        /// <param name="request"><see cref="LoadReportForEditRequest"/></param>
        /// <returns><see cref="LoadReportForEditResponse"/></returns>
        [OperationContract]
        LoadReportForEditResponse LoadReportForEdit(LoadReportForEditRequest request);

        /// <summary>
        /// Obtains the set of scheduled interpretations that can optionally be linked to the specified interpretation
        /// so that the report is shared.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetLinkableInterpretationsResponse GetLinkableInterpretations(GetLinkableInterpretationsRequest request);

        /// <summary>
        /// Save the report of a given reporting step
        /// </summary>
        /// <param name="request"><see cref="SaveReportRequest"/></param>
        /// <returns><see cref="SaveReportResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
        SaveReportResponse SaveReport(SaveReportRequest request);

        /// <summary>
        /// Get all the reports of a given patient
        /// </summary>
        /// <param name="request"><see cref="GetPriorsRequest"/></param>
        /// <returns><see cref="GetPriorsResponse"/></returns>
        [OperationContract]
        GetPriorsResponse GetPriors(GetPriorsRequest request);
    }
}
