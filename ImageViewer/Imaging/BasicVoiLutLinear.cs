using ClearCanvas.Desktop;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class BasicVoiLutLinear : VoiLutLinearBase, IBasicVoiLutLinear
	{
		private class WindowLevelMemento : IMemento, IEquatable<WindowLevelMemento>
		{
			public readonly double WindowWidth;
			public readonly double WindowCenter;

			public WindowLevelMemento(double windowWidth, double windowCenter)
			{
				WindowWidth = windowWidth;
				WindowCenter = windowCenter;
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is WindowLevelMemento)
					return this.Equals((WindowLevelMemento) obj);

				return false;
			}

			#region IEquatable<WindowLevelMemento> Members

			public bool Equals(WindowLevelMemento other)
			{
				return this.WindowWidth == other.WindowWidth && this.WindowCenter == other.WindowCenter;
			}

			#endregion
		}

		private double _windowWidth;
		private double _windowCenter;

		public BasicVoiLutLinear(double windowWidth, double windowCenter)
			: base()
		{
			this.WindowWidth = windowWidth;
			this.WindowCenter = windowCenter;
		}

		public BasicVoiLutLinear()
			: this(1, 0)
		{
		}

		protected override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		protected override double GetWindowCenter()
		{
			return this.WindowCenter;
		}

		/// <summary>
		/// Gets or sets the window width.
		/// </summary>
		public double WindowWidth
		{
			get { return _windowWidth; }
			set
			{
				if (value == _windowWidth)
					return;

				if (value < 1)
					value = 1;

				_windowWidth = value;
				base.OnLutChanged();
			}
		}

		/// <summary>
		/// Gets or sets the window center.
		/// </summary>
		public double WindowCenter
		{
			get { return _windowCenter; }
			set
			{
				if (value == _windowCenter)
					return;

				_windowCenter = value;
				base.OnLutChanged();
			}
		}

		public override string GetDescription()
		{
			return String.Format("W:{0} L:{1}", WindowWidth, WindowCenter);
		}

		public override IMemento CreateMemento()
		{
			return new WindowLevelMemento(this.WindowWidth, this.WindowCenter);
		}

		public override void SetMemento(IMemento memento)
		{
			WindowLevelMemento windowLevelMemento = memento as WindowLevelMemento;
			Platform.CheckForInvalidCast(windowLevelMemento, "memento", typeof(WindowLevelMemento).Name);

			this.WindowWidth = windowLevelMemento.WindowWidth;
			this.WindowCenter = windowLevelMemento.WindowCenter;
		}
	}
}
