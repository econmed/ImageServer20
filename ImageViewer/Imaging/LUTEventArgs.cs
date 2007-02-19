using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class LUTEventArgs : CollectionEventArgs<IComposableLUT>
	{
		public LUTEventArgs()
		{

		}

		public LUTEventArgs(IComposableLUT lut)
		{
			Platform.CheckForNullReference(lut, "lut");

			base.Item = lut;
		}

		public IComposableLUT Lut { get { return base.Item; } }
	}
}
