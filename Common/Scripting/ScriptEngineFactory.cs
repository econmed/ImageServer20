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
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Common.Scripting
{
	/// <summary>
	/// Extension point for <see cref="IScriptEngine"/>s.
	/// </summary>
	[ExtensionPoint]
	public sealed class ScriptEngineExtensionPoint : ExtensionPoint<IScriptEngine>
	{
	}

	/// <summary>
	/// Factory for creating instances of <see cref="IScriptEngine"/>s that support a given language.
	/// </summary>
	public static class ScriptEngineFactory
	{
		private static readonly Dictionary<string, IScriptEngine> _singletonEngineInstances = new Dictionary<string, IScriptEngine>();
		private static readonly object _syncLock = new object();

		/// <summary>
		/// Attempts to instantiate a script engine for the specified language. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// Internally, this class looks for an extension of <see cref="ScriptEngineExtensionPoint"/> 
		/// that is capable of running scripts in the specified language.
		/// In order to be considered a match, extensions must be decorated with a 
		/// <see cref="LanguageSupportAttribute"/> matching the <paramref name="language"/> parameter.
		/// </para>
		/// <para>
		/// If the engine is marked as a singleton, and a cached instance already exists, that instance will
		/// be returned.
		/// </para>
		/// <para>
		/// This method can safely be called by multiple threads.
		/// </para>
		/// </remarks>
		/// <param name="language">The case-insensitive script language, so jscript is equivalent to JScript.</param>
		public static IScriptEngine GetEngine(string language)
		{
			lock (_syncLock)
			{
				// check for a cached singleton engine instance for this language
				IScriptEngine engine;
				if (_singletonEngineInstances.TryGetValue(language, out engine))
					return engine;

				// create a new engine instance
				engine = CreateEngine(language);

				var optionsAttr = AttributeUtils.GetAttribute<ScriptEngineOptionsAttribute>(engine.GetType());
				if (optionsAttr != null)
				{
					// if the engine requires synchronization, wrap it
					if (optionsAttr.SynchronizeAccess)
					{
						engine = new SynchronizedScriptEngineWrapper(engine);
					}

					// if the engine is a singleton, cache this instance
					if (optionsAttr.Singleton)
					{
						_singletonEngineInstances.Add(language, engine);
					}
				}
				return engine;
			}
		}

		private static IScriptEngine CreateEngine(string language)
		{
			try
			{
				var xp = new ScriptEngineExtensionPoint();
				return (IScriptEngine)xp.CreateExtension(new AttributeExtensionFilter(new LanguageSupportAttribute(language)));
			}
			catch (NotSupportedException e)
			{
				throw new NotSupportedException(string.Format(SR.ExceptionScriptEngineLanguage, language), e);
			}
		}
	}
}
