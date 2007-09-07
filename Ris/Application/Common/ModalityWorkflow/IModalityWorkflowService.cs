﻿using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [ServiceContract]
    public interface IModalityWorkflowService
    {
        [OperationContract]
        ListWorklistsResponse ListWorklists(ListWorklistsRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        GetWorklistResponse GetWorklist(GetWorklistRequest request);

        [OperationContract]
        GetWorklistCountResponse GetWorklistCount(GetWorklistCountRequest request);

        [OperationContract]
        GetWorklistItemResponse GetWorklistItem(GetWorklistItemRequest request);

        [OperationContract]
        LoadWorklistItemPreviewResponse LoadWorklistItemPreview(LoadWorklistItemPreviewRequest request);

        [OperationContract]
        GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request);

        [OperationContract]
        StartProcedureResponse StartProcedure(StartProcedureRequest request);

        [OperationContract]
        CompleteProcedureResponse CompleteProcedure(CompleteProcedureRequest request);

        [OperationContract]
        CancelProcedureResponse CancelProcedure(CancelProcedureRequest request);
    }
}
