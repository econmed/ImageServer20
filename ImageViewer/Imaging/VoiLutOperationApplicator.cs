using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class VoiLutOperationApplicator : ImageOperationApplicator
	{
		/// <summary>
		/// Initializes a new instance of <see cref="VoiLutOperationApplicator"/>
		/// with the specified <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="image">an image</param>
		public VoiLutOperationApplicator(IPresentationImage image)
			: base(image)
		{ 
		}

		/// <summary>
		/// Gets the <see cref="IVoiLutManager"/> which should be the originator for <b>all</b> Voi Lut undoable operations.
		/// </summary>
		/// <param name="image">The image the memento is associated with</param>
		/// <returns>the <see cref="IVoiLutManager"/> which should be the originator for <b>all</b> Voi Lut undoable operation.</returns>
		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			IVoiLutManagerProvider provider = image as IVoiLutManagerProvider;
			if (provider == null)
				throw new Exception("Presentation image does not support IVoiLutManagerProvider");

			return provider.VoiLutManager;
		}
	}
}
