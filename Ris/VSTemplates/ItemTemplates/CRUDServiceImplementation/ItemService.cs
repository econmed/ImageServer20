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
using ClearCanvas.Ris.Application.Common.Admin.$fileinputname$Admin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace $rootnamespace$
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(I$fileinputname$AdminService))]
    public class $fileinputname$AdminService : ApplicationServiceBase, I$fileinputname$AdminService
    {
        #region I$fileinputname$AdminService Members

        [ReadOperation]
        public List$fileinputname$sResponse List$fileinputname$s(List$fileinputname$sRequest request)
        {
            Platform.CheckForNullReference(request, "request");
			
			$fileinputname$SearchCriteria where = new $fileinputname$SearchCriteria();
			//TODO: add sorting
		
            I$fileinputname$Broker broker = PersistenceContext.GetBroker<I$fileinputname$Broker>();
            IList<$fileinputname$> items = broker.Find(where, request.Page);

            $fileinputname$Assembler assembler = new $fileinputname$Assembler();
            return new List$fileinputname$sResponse(
                CollectionUtils.Map<$fileinputname$, $fileinputname$Summary>(items,
                    delegate($fileinputname$ item)
                    {
                        return assembler.CreateSummary(item, PersistenceContext);
                    })
                );
        }

        [ReadOperation]
        public Load$fileinputname$ForEditResponse Load$fileinputname$ForEdit(Load$fileinputname$ForEditRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.$fileinputname$Ref, "request.$fileinputname$Ref");

            $fileinputname$ item = PersistenceContext.Load<$fileinputname$>(request.$fileinputname$Ref);

            $fileinputname$Assembler assembler = new $fileinputname$Assembler();
            return new Load$fileinputname$ForEditResponse(assembler.CreateDetail(item, PersistenceContext));
        }

        [ReadOperation]
        public Load$fileinputname$EditorFormDataResponse Load$fileinputname$EditorFormData(Load$fileinputname$EditorFormDataRequest request)
        {
             return new Load$fileinputname$EditorFormDataResponse();
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.$fileinputname$)]
        public Add$fileinputname$Response Add$fileinputname$(Add$fileinputname$Request request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.$fileinputname$, "request.$fileinputname$");

            $fileinputname$ item = new $fileinputname$();

            $fileinputname$Assembler assembler = new $fileinputname$Assembler();
            assembler.Update$fileinputname$(item, request.$fileinputname$, PersistenceContext);

            PersistenceContext.Lock(item, DirtyState.New);
            PersistenceContext.SynchState();

            return new Add$fileinputname$Response(assembler.CreateSummary(item, PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.$fileinputname$)]
        public Update$fileinputname$Response Update$fileinputname$(Update$fileinputname$Request request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.$fileinputname$, "request.$fileinputname$");
            Platform.CheckMemberIsSet(request.$fileinputname$.$fileinputname$Ref, "request.$fileinputname$.$fileinputname$Ref");

            $fileinputname$ item = PersistenceContext.Load<$fileinputname$>(request.$fileinputname$.$fileinputname$Ref);

            $fileinputname$Assembler assembler = new $fileinputname$Assembler();
            assembler.Update$fileinputname$(item, request.$fileinputname$, PersistenceContext);

            PersistenceContext.SynchState();

            return new Update$fileinputname$Response(assembler.CreateSummary(item, PersistenceContext));
        }

        #endregion
    }
}
