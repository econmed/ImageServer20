using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	public interface IDicomServerContext
	{
		string AETitle { get; }
		string Host { get; }
		int Port { get; }
		string InterimStorageDirectory { get; }
	}

	public abstract class ScpExtension : IDicomScp<IDicomServerContext>
	{
		private IDicomServerContext _context;
		private readonly List<SupportedSop> _supportedSops;

		protected ScpExtension(IEnumerable<SupportedSop> supportedSops)
		{
			_supportedSops = new List<SupportedSop>(supportedSops);
		}

		protected IDicomServerContext Context
		{
			get { return _context; }
		}

		#region IDicomScp<ServerContext> Members

		public DicomPresContextResult VerifyAssociation(AssociationParameters association, byte pcid)
		{
			DicomRejectResult result;
			DicomRejectReason reason;
			if (!AssociationVerifier.VerifyAssociation(Context, association, out result, out reason))
				return DicomPresContextResult.RejectUser;

			return OnVerifyAssociation(association, pcid);
		}

		public virtual DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
		{
			return DicomPresContextResult.Accept;
		}

		public abstract bool OnReceiveRequest(Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message);

		public IList<SupportedSop> GetSupportedSopClasses()
		{
			return _supportedSops;
		}

		public void SetContext(IDicomServerContext context)
		{
			_context = context;	
		}

		#endregion
	}
}
