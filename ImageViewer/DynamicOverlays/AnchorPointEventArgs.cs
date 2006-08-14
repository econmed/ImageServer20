using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for AnchorPointEventArgs.
	/// </summary>
	public class AnchorPointEventArgs : CollectionEventArgs<PointF>
	{
		private int _anchorPointIndex;

		public AnchorPointEventArgs()
		{
		}

		public AnchorPointEventArgs(int anchorPointIndex, PointF anchorPoint)
		{
			Platform.CheckNonNegative(anchorPointIndex, "anchorPointIndex");

			_anchorPointIndex = anchorPointIndex;
			base.Item = anchorPoint;
		}

		public int AnchorPointIndex
		{
			get { return _anchorPointIndex; }
			set { _anchorPointIndex = value; }
		}

		public PointF AnchorPoint { get { return base.Item; } }
	}
}
