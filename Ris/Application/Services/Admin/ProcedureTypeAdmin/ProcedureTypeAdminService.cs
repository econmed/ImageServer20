using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.ProcedureTypeAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IProcedureTypeAdminService))]
	public class ProcedureTypeAdminService : ApplicationServiceBase, IProcedureTypeAdminService
	{
		#region IProcedureTypeAdminService Members

		[ReadOperation]
		public ListProcedureTypesResponse ListProcedureTypes(ListProcedureTypesRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
			where.Id.SortAsc(0);
			if (!string.IsNullOrEmpty(request.Id))
				where.Id.StartsWith(request.Id);
			if (!string.IsNullOrEmpty(request.Name))
				where.Name.Like(string.Format("%{0}%", request.Name));

			IProcedureTypeBroker broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
			IList<ProcedureType> items = broker.Find(where, request.Page);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			return new ListProcedureTypesResponse(
				CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(items,
					delegate(ProcedureType item)
					{
						return assembler.CreateSummary(item);
					})
				);
		}

		[ReadOperation]
		public LoadProcedureTypeForEditResponse LoadProcedureTypeForEdit(LoadProcedureTypeForEditRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureTypeRef, "request.ProcedureTypeRef");

			ProcedureType item = PersistenceContext.Load<ProcedureType>(request.ProcedureTypeRef);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			return new LoadProcedureTypeForEditResponse(assembler.CreateDetail(item));
		}

		[ReadOperation]
		public LoadProcedureTypeEditorFormDataResponse LoadProcedureTypeEditorFormData(LoadProcedureTypeEditorFormDataRequest request)
		{
			ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
			where.Id.SortAsc(0);
			where.Deactivated.EqualTo(false);

			IList<ProcedureType> procTypes = PersistenceContext.GetBroker<IProcedureTypeBroker>().Find(where);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			return new LoadProcedureTypeEditorFormDataResponse(
				CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(procTypes,
					delegate(ProcedureType pt) { return assembler.CreateSummary(pt); }));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public AddProcedureTypeResponse AddProcedureType(AddProcedureTypeRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureType, "request.ProcedureType");

			ProcedureType item = new ProcedureType();

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			assembler.UpdateProcedureType(item, request.ProcedureType, PersistenceContext);

			PersistenceContext.Lock(item, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddProcedureTypeResponse(assembler.CreateSummary(item));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public UpdateProcedureTypeResponse UpdateProcedureType(UpdateProcedureTypeRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureType, "request.ProcedureType");
			Platform.CheckMemberIsSet(request.ProcedureType.ProcedureTypeRef, "request.ProcedureType.ProcedureTypeRef");

			ProcedureType item = PersistenceContext.Load<ProcedureType>(request.ProcedureType.ProcedureTypeRef);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			assembler.UpdateProcedureType(item, request.ProcedureType, PersistenceContext);

			PersistenceContext.SynchState();

			return new UpdateProcedureTypeResponse(assembler.CreateSummary(item));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public DeleteProcedureTypeResponse DeleteProcedureType(DeleteProcedureTypeRequest request)
		{
			try
			{
				IProcedureTypeBroker broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
				ProcedureType item = broker.Load(request.ProcedureTypeRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteProcedureTypeResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, typeof(ProcedureType).Name));
			}
		}

		#endregion
	}
}
