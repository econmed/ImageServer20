using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	/// <summary>
	/// Extension point for views onto <see cref="PathProfileComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PathProfileComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PathProfileComponent class
	/// </summary>
	[AssociateView(typeof(PathProfileComponentViewExtensionPoint))]
	public class PathProfileComponent : RoiAnalysisComponent
	{
		private int[] _pixelIndices;
		private int[] _pixelValues;

		/// <summary>
		/// Constructor
		/// </summary>
		public PathProfileComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext)
		{
		}

		public int[] PixelIndices
		{
			get { return _pixelIndices; }
		}

		public int[] PixelValues
		{
			get { return _pixelValues; }
		}

		public override void Start()
		{
			// TODO prepare the component for its live phase
			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public bool ComputeProfile()
		{
			PolyLineInteractiveGraphic polyLine = GetSelectedPolyLine();

			// For now, make sure the ROI is a polyline
			if (polyLine == null)
			{
				this.Enabled = false;
				return false;
			}

			IImageGraphicProvider imageGraphicProvider =
				polyLine.ParentPresentationImage as IImageGraphicProvider;

			if (imageGraphicProvider == null)
			{
				this.Enabled = false;
				return false;
			}

			// For now, only allow ROIs of grayscale images
			GrayscaleImageGraphic image = imageGraphicProvider.ImageGraphic as GrayscaleImageGraphic;

			if (image == null)
			{
				this.Enabled = false;
				return false;
			}

			polyLine.CoordinateSystem = CoordinateSystem.Source;
			Point pt1 = new Point((int)polyLine.AnchorPoints[0].X, (int)polyLine.AnchorPoints[0].Y);
			Point pt2 = new Point((int)polyLine.AnchorPoints[1].X, (int)polyLine.AnchorPoints[1].Y);

			if (pt1.X < 0 || pt1.X > image.Columns - 1 ||
				pt2.X < 0 || pt2.X > image.Columns - 1 ||
				pt1.Y < 0 || pt1.Y > image.Rows - 1 ||
				pt2.Y < 0 || pt2.Y > image.Rows - 1)
			{
				this.Enabled = false;
				return false;
			}


			List<Point> pixels = BresenhamLine(pt1, pt2);

			_pixelIndices = new int[pixels.Count];
			_pixelValues = new int[pixels.Count];

			int i = 0;

			foreach (Point pixel in pixels)
			{
				int rawPixelValue = image.PixelData.GetPixel(pixel.X, pixel.Y);
				_pixelIndices[i] = i;
				_pixelValues[i] = image.ModalityLut[rawPixelValue];
				i++;
			}

			this.Enabled = true;
			return true;
		}

		protected override bool CanAnalyzeSelectedRoi()
		{
			return GetSelectedPolyLine() == null ? false : true;
		}

		private PolyLineInteractiveGraphic GetSelectedPolyLine()
		{
			ROIGraphic graphic = GetSelectedRoi();

			if (graphic == null)
				return null;

			PolyLineInteractiveGraphic polyLine = graphic.Roi as PolyLineInteractiveGraphic;

			if (polyLine == null)
				return null;

			return polyLine;
		}


		// Swap the values of A and B
		private void Swap<T>(ref T a, ref T b)
		{
			T c = a;
			a = b;
			b = c;
		}

		// Returns the list of points from p0 to p1 
		private List<Point> BresenhamLine(Point p0, Point p1)
		{
			return BresenhamLine(p0.X, p0.Y, p1.X, p1.Y);
		}

		// Returns the list of points from (x0, y0) to (x1, y1)
		private List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
		{
			// Optimization: it would be preferable to calculate in
			// advance the size of "result" and to use a fixed-size array
			// instead of a list.
			List<Point> result = new List<Point>();

			bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (steep)
			{
				Swap(ref x0, ref y0);
				Swap(ref x1, ref y1);
			}
			if (x0 > x1)
			{
				Swap(ref x0, ref x1);
				Swap(ref y0, ref y1);
			}

			int deltax = x1 - x0;
			int deltay = Math.Abs(y1 - y0);
			int error = 0;
			int ystep;
			int y = y0;
			if (y0 < y1) ystep = 1; else ystep = -1;
			for (int x = x0; x <= x1; x++)
			{
				if (steep) result.Add(new Point(y, x));
				else result.Add(new Point(x, y));
				error += deltay;
				if (2 * error >= deltax)
				{
					y += ystep;
					error -= deltax;
				}
			}

			return result;
		}
	}
}
