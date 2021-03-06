#region License

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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public interface IArgumentHintResolver : IDisposable
	{
		string Resolve(string input);
		string Resolve(string input, bool resolveMultiValuedHints, string multiValueSeparator);
	}

	internal sealed class ArgumentHintResolver : IArgumentHintResolver
	{
		private static readonly Regex _pattern = new Regex(@"\$(\w*?)\$", RegexOptions.Compiled);
		private readonly Dictionary<string, ArgumentHintValue> _resolvedHints;
		private readonly IList<IArgumentHint> _hints;

		public ArgumentHintResolver() : this(null) {}

		public ArgumentHintResolver(IEnumerable<IArgumentHint> hints)
		{
			if (hints != null)
				this._hints = new List<IArgumentHint>(hints);
			else
				this._hints = new List<IArgumentHint>();

			this._resolvedHints = new Dictionary<string, ArgumentHintValue>();
			this._resolvedHints.Add("", new ArgumentHintValue("$"));
		}

		public void Dispose()
		{
			foreach (IArgumentHint hint in _hints)
				hint.Dispose();
			_hints.Clear();
		}

		public string Resolve(string input)
		{
			return Resolve(input, false, string.Empty);
		}

		public string Resolve(string input, bool resolveMultiValuedHints, string multiValueSeparator)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			int lastCopiedChar = -1;
			StringBuilder sb = new StringBuilder();

			foreach (Match m in _pattern.Matches(input))
			{
				string key = m.Groups[1].Value;
				if (!this._resolvedHints.ContainsKey(key))
				{
					// find a hint that provides this key
					foreach (IArgumentHint hint in this._hints)
					{
						ArgumentHintValue value = hint[key];
						if (value.IsNull)
							continue;
						this._resolvedHints.Add(key, value);
						break;
					}

					if (!this._resolvedHints.ContainsKey(key))
						return null; // unable to resolve arguments in this string
				}

				ArgumentHintValue hintValue = _resolvedHints[key];
				string resolvedHint;
				if (resolveMultiValuedHints)
					resolvedHint = hintValue.ToString(multiValueSeparator);
				else if (!hintValue.IsMultiValued)
					resolvedHint = hintValue.ToString();
				else
					return null; // unable to resolve argument with multivalue hints disabled

				++lastCopiedChar;
				sb.Append(input.Substring(lastCopiedChar, m.Index - lastCopiedChar));
				sb.Append(resolvedHint);
				lastCopiedChar = m.Index + m.Length - 1;
			}
			++lastCopiedChar;
			sb.Append(input.Substring(lastCopiedChar, input.Length - lastCopiedChar));

			return sb.ToString();
		}
	}
}