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

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Common.Specifications
{
	[ExtensionOf(typeof(ExpressionFactoryExtensionPoint))]
	[LanguageSupport("jscript")]
	public class JScriptExpressionFactory : IExpressionFactory
	{
		#region IExpressionFactory Members

		public Expression CreateExpression(string text)
		{
			return new JScriptExpression(text);
		}

		#endregion
	}

	internal class JScriptExpression : Expression
	{
		private const string AutomaticVariableToken = "$";

		[ThreadStatic]
		private static IScriptEngine _scriptEngine;
		private IExecutableScript _script;

		internal JScriptExpression(string text)
			: base(text)
		{
		}

		public override object Evaluate(object arg)
		{
			if (string.IsNullOrEmpty(this.Text))
				return null;

			if (this.Text == AutomaticVariableToken)
				return arg;

			try
			{
				// create the script if not yet created
				if (_script == null)
					_script = CreateScript(this.Text);

				// evaluate the test expression
				var context = new Dictionary<string, object> {{AutomaticVariableToken, arg}};
				return _script.Run(context);
			}
			catch (Exception e)
			{
				throw new SpecificationException(string.Format(SR.ExceptionJScriptEvaluation, this.Text), e);
			}
		}

		private static IExecutableScript CreateScript(string expression)
		{
			return ScriptEngine.CreateScript("return " + expression, new [] { AutomaticVariableToken });
		}

		private static IScriptEngine ScriptEngine
		{
			get
			{
				if (_scriptEngine == null)
				{
					_scriptEngine = ScriptEngineFactory.GetEngine("jscript");
				}
				return _scriptEngine;
			}
		}
	}
}
