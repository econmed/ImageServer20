using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A class that wraps a <see cref="DataLut"/> inside an <see cref="IBasicVoiLutLinear"/>, in
	/// order to allow 'window/levelling' of the <see cref="DataLut"/>.  
	/// </summary>
	/// <remarks>
	/// Internally, this will be treated like any other linear lut, except that
	/// <see cref="GetDescription"/> expresses the Window Width/Center as a percentage of 
	/// the full window, since the true values won't necessarily have any real meaning.
	/// </remarks>
	[Cloneable(true)]
	public class AdjustableDataLut : ComposableLut, IBasicVoiLutLinear
	{
		private class Memento
		{
			public readonly object DataLutMemento;
			public readonly object LinearLutMemento;
			
			public Memento(object dataLutMemento, object linearLutMemento)
			{
				DataLutMemento = dataLutMemento;
				LinearLutMemento = linearLutMemento;
			}

			public override bool Equals(object obj)
			{
				if (Object.ReferenceEquals(obj, this))
					return true;

				if (obj is Memento)
				{
					Memento other = obj as Memento;
					return Object.Equals(other.DataLutMemento, DataLutMemento) && 
							Object.Equals(other.LinearLutMemento, LinearLutMemento);
				}

				return false;
			}
		}

		#region Private Fields

		private readonly DataLut _dataLut;
		private readonly BasicVoiLutLinear _linearLut;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public AdjustableDataLut(DataLut dataLut)
		{
			_dataLut = dataLut;
			_dataLut.LutChanged += OnDataLutChanged;

			_linearLut = new BasicVoiLutLinear();
			_linearLut.LutChanged += OnLinearLutChanged;

			Reset();
		}

		private AdjustableDataLut()
		{
		}

		#endregion

		#region Private Properties

		private double FullWindow
		{
			get { return _linearLut.MaxInputValue - _linearLut.MinInputValue + 1; }
		}

		private double BrightnessPercent
		{
			get { return 100 - (WindowCenter - _linearLut.MinInputValue) / FullWindow * 100; }
		}

		private double ContrastPercent
		{
			get { return WindowWidth / FullWindow * 100; }
		}

		#endregion

		#region Private Methods

		private void OnDataLutChanged(object sender, EventArgs e)
		{
			UpdateMinMaxInputLinear();
			base.OnLutChanged();
		}

		private void OnLinearLutChanged(object sender, EventArgs e)
		{
			base.OnLutChanged();
		}

		private void UpdateMinMaxInputLinear()
		{
			_linearLut.MinInputValue = _dataLut.MinOutputValue;
			_linearLut.MaxInputValue = _dataLut.MaxOutputValue;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the underlying data lut.
		/// </summary>
		public DataLut DataLut
		{
			get { return _dataLut; }	
		}

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override int MinInputValue
		{
			get { return _dataLut.MinInputValue; }
			set { _dataLut.MinInputValue = value; }
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override int MaxInputValue
		{
			get { return _dataLut.MaxInputValue; }
			set { _dataLut.MaxInputValue = value; }
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public override int MinOutputValue
		{
			get { return _linearLut.MinOutputValue; }
			protected set { _linearLut.MinOutputValue = value; }
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public override int MaxOutputValue
		{
			get { return _linearLut.MaxOutputValue; }
			protected set { _linearLut.MaxOutputValue = value; }
		}

		#region IBasicVoiLutLinear Members

		/// <summary>
		/// Gets or sets the Window Width.
		/// </summary>
		public double WindowWidth
		{
			get { return _linearLut.WindowWidth; }
			set { _linearLut.WindowWidth = value; }
		}

		/// <summary>
		/// Gets or sets the Window Center.
		/// </summary>
		public double WindowCenter
		{
			get { return _linearLut.WindowCenter; }
			set { _linearLut.WindowCenter = value; }
		}

		#endregion

		#endregion

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		public override int this[int index]
		{
			get
			{
				return _linearLut[_dataLut[index]];
			}
			protected set
			{
				throw new InvalidOperationException("This lut type is read-only.");
			}
		}

		#region Public Methods

		/// <summary>
		/// Resets the 
		/// </summary>
		public void Reset()
		{
			UpdateMinMaxInputLinear();

			_linearLut.WindowWidth = FullWindow;
			_linearLut.WindowCenter = _dataLut.MinOutputValue + FullWindow / 2;

			base.OnLutChanged();
		}

		/// <summary>
		/// Gets a string key that identifies this particular Lut's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some Luts can be
		/// dependent upon the actual image to which it belongs.  The method should simply 
		/// be used to determine if a lut in the <see cref="ComposedLutPool"/> is the same 
		/// as an existing one.
		/// </remarks>
		public override string GetKey()
		{
			return String.Format("{0}:{1}", _dataLut.GetKey(), _linearLut.GetKey());
		}

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return String.Format(SR.FormatAdjustableDataLutDescription, _dataLut.GetDescription(), ContrastPercent, BrightnessPercent);
		}

		public override object CreateMemento()
		{
			return new Memento(_dataLut.CreateMemento(), _linearLut.CreateMemento());
		}

		public override void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			Memento lutMemento = memento as Memento;
			Platform.CheckForInvalidCast(lutMemento, "memento", typeof(Memento).FullName);

			if (lutMemento.DataLutMemento != null)
				_dataLut.SetMemento(lutMemento.DataLutMemento);
			
			if (lutMemento.LinearLutMemento != null)
				_linearLut.SetMemento(lutMemento.LinearLutMemento);
		}

		#endregion
	}
}