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
using System.Reflection;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Conveys information about plugins as they are loaded.
	/// </summary>
	/// <remarks>
	/// This class is used internally by the framework.
	/// </remarks>
	/// <see cref="PluginManager"/>
	public class PluginLoadedEventArgs : EventArgs
	{
		string _message;
		Assembly _pluginAssembly;

		internal PluginLoadedEventArgs(string message, Assembly pluginAssembly)
		{
			_message = message;
			_pluginAssembly = pluginAssembly;
		}

		/// <summary>
		/// Gets a user-friendly message describing the plugin that was loaded.
		/// </summary>
		/// <remarks>
		/// This is typically just the full name of the plugin assembly.
		/// </remarks>
		public string Message
		{
			get
			{
				return _message;
			}
		}

		/// <summary>
		/// Gets the plugin assembly that was loaded, if any.
		/// </summary>
		/// <remarks>
		/// Null if no actual assembly was loaded for this particular event.
		/// </remarks>
		public Assembly PluginAssembly
		{
			get
			{
				return _pluginAssembly;
			}
		}
	}
}
