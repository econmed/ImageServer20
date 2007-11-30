#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to declare "click" actions.
    /// </summary>
    public abstract class ClickActionAttribute : ActionInitiatorAttribute
    {
        private string _path;
        private string _clickHandler;
        private ClickActionFlags _flags;
		private XKeys _keyStroke;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="clickHandler">The name of the method that will be invoked when the action is clicked.</param>
        public ClickActionAttribute(string actionID, string path, string clickHandler)
            :base(actionID)
        {
            _path = path;
            _clickHandler = clickHandler;
            _flags = ClickActionFlags.None; // default value, will override if named parameter is specified
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        public ClickActionAttribute(string actionID, string path)
            : this(actionID, path, null)
        {
        }

        /// <summary>
        /// Gets or sets the flags that customize the behaviour of the action.
        /// </summary>
        public ClickActionFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        /// <summary>
        /// Gets or sets the key-stroke that should invoke the action from the keyboard.
        /// </summary>
		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}

		/// <summary>
		/// The suggested location of the action in the action model.
		/// </summary>
		public string Path { get { return _path; } }
		
		/// <summary>
		/// Applies this attribute to an <see cref="IAction"/> instance, via the specified <see cref="IActionBuildingContext"/>.
    	/// </summary>
    	/// <remarks>
    	/// Because this action is an <see cref="ActionInitiatorAttribute"/>, this method actually
    	/// creates the associated <see cref="ClickAction"/>.  <see cref="ActionDecoratorAttribute"/>s
    	/// merely modify the properties of the action.
    	/// </remarks>
    	public override void Apply(IActionBuildingContext builder)
        {
            // assert _action == null
            ActionPath path = new ActionPath(this.Path, builder.ResourceResolver);
            builder.Action = CreateAction(builder.ActionID, path, this.Flags, builder.ResourceResolver);
            builder.Action.Persistent = true;
            ((ClickAction)builder.Action).KeyStroke = this.KeyStroke;
            builder.Action.Label = path.LastSegment.LocalizedText;

            if (!string.IsNullOrEmpty(_clickHandler))
            {
                // check that the method exists, etc
                ValidateClickHandler(builder.ActionTarget, _clickHandler);

                ClickHandlerDelegate clickHandler =
                    (ClickHandlerDelegate)Delegate.CreateDelegate(typeof(ClickHandlerDelegate), builder.ActionTarget, _clickHandler);
                ((ClickAction)builder.Action).SetClickHandler(clickHandler);
            }
        }

		/// <summary>
		/// Creates the <see cref="ClickAction"/> represented by this attribute.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="flags">Flags that specify the click behaviour of the action.</param>
		/// <param name="resolver">The object used to resolve the action path and icons.</param>
        protected abstract ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resolver);

        private static void ValidateClickHandler(object target, string methodName)
        {
            MethodInfo info = target.GetType().GetMethod(
                methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);

            if (info == null)
            {
                throw new ActionBuilderException(
                    string.Format(SR.ExceptionActionBuilderMethodDoesNotExist, methodName, target.GetType().FullName));
            }
        }
    }
}
