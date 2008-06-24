#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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


namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A base implementation for Color Map factories.
	/// </summary>
	/// <typeparam name="T">Must be derived from <see cref="ColorMap"/> and have a parameterless default constructor.</typeparam>
	/// <seealso cref="IColorMapFactory"/>
	/// <seealso cref="ColorMap"/>
	public abstract class ColorMapFactoryBase<T> : IColorMapFactory
		where T : ColorMap, new()
	{
		#region Protected Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ColorMapFactoryBase()
		{
		}

		#endregion

		#region IColorMapFactory Members

		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IColorMapFactory"/>s.
		/// </summary>
		/// <remarks>
		/// This name should not be a resource string, as it should be language-independent.
		/// </remarks>
		public abstract string Name { get; }

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// Creates and returns a <see cref="ColorMap"/>.
		/// </summary>
		public IDataLut Create()
		{
			return new T();
		}

		#endregion
	}
}
