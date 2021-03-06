﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A VOI LUT Manager, which is responsible for managing installation and restoration
	/// of VOI LUTs via the Memento pattern.
	/// </summary>
	public sealed class VoiLutManager : IVoiLutManager
	{
		#region Private Fields
		
		private readonly IVoiLutInstaller _voiLutInstaller;
		private bool _enabled = true;
		private bool _allowDisable;
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public VoiLutManager(IVoiLutInstaller voiLutInstaller, bool allowDisable)
		{
			Platform.CheckForNullReference(voiLutInstaller, "voiLutInstaller");
			_voiLutInstaller = voiLutInstaller;
			_allowDisable = allowDisable;
		}

		#region IVoiLutManager Members

		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IComposableLut"/>.</returns>
		public IComposableLut GetLut()
		{
			return _voiLutInstaller.VoiLut;
		}

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="lut">The Lut to be installed.</param>
		public void InstallLut(IComposableLut lut)
		{
			InstallVoiLut(lut);
		}

		#endregion

		#region IVoiLutInstaller Members

		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IComposableLut"/>.</returns>
		public IComposableLut VoiLut
		{
			get { return _voiLutInstaller.VoiLut; }	
		}

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="lut">The Lut to be installed.</param>
		public void InstallVoiLut(IComposableLut lut)
		{
			IComposableLut existingLut = GetLut();
			if (existingLut is IGeneratedDataLut)
			{
				//Clear the data in the data lut so it's not hanging around using up memory.
				((IGeneratedDataLut)existingLut).Clear();
			}
			
			_voiLutInstaller.InstallVoiLut(lut);
		}

		/// <summary>
		/// Gets or sets whether the output of the VOI LUT should be inverted for display.
		/// </summary>
		public bool Invert
		{
			get { return _voiLutInstaller.Invert; }
			set { _voiLutInstaller.Invert = value; }
		}

		/// <summary>
		/// Toggles the state of the <see cref="IVoiLutInstaller.Invert"/> property.
		/// </summary>
		[Obsolete("Use the Invert property instead.")]
		public void ToggleInvert()
		{
			_voiLutInstaller.Invert = !_voiLutInstaller.Invert;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the LUT should be used in rendering the parent object.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if LUTs may not be disabled for rendering the parent object.</exception>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (!_allowDisable)
					throw new InvalidOperationException();

				_enabled = value;
			}
		}

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Captures enough information to restore the currently installed lut.
		/// </summary>
		public object CreateMemento()
		{
			return new VoiLutMemento(_voiLutInstaller.VoiLut, _voiLutInstaller.Invert);
		}

		/// <summary>
		/// Restores the previously installed lut and/or it's state.
		/// </summary>
		public void SetMemento(object memento)
		{
			VoiLutMemento lutMemento = (VoiLutMemento) memento;

			if (_voiLutInstaller.VoiLut != lutMemento.ComposableLutMemento.OriginatingLut)
				this.InstallLut(lutMemento.ComposableLutMemento.OriginatingLut);

			if (lutMemento.ComposableLutMemento.InnerMemento != null)
				_voiLutInstaller.VoiLut.SetMemento(lutMemento.ComposableLutMemento.InnerMemento);

			_voiLutInstaller.Invert = lutMemento.Invert;
		}

		#endregion
	}
}
