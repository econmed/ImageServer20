using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Summary description for IGrayscaleLUT.
	/// </summary>
	public interface IGrayscaleLUT : ILUT
	{
		int MinInputValue
		{
			get;
		}

		int MaxInputValue
		{
			get;
		}

		int MinOutputValue
		{
			get;
		}

		int MaxOutputValue
		{
			get;
		}
	}
}
