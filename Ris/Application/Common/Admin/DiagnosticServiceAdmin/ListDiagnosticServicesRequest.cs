using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
	[DataContract]
	public class ListDiagnosticServicesRequest : PagedDataContractBase
	{
		public ListDiagnosticServicesRequest()
		{
		}

		public ListDiagnosticServicesRequest(SearchResultPage page)
			: base(page)
		{
		}
	}
}
