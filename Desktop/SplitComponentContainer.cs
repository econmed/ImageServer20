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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
	/// Defines an extension point for views onto the <see cref="SplitComponentContainer"/>.
    /// </summary>
	public sealed class SplitComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// Specifies the orientation of the <see cref="SplitComponentContainer"/>.
	/// </summary>
	public enum SplitOrientation
	{
		/// <summary>
		/// The <see cref="SplitComponentContainer"/> should be split horizontally.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// The <see cref="SplitComponentContainer"/> should be split vertically.
		/// </summary>
		Vertical = 1
	}

	/// <summary>
	/// A component container for hosting two <see cref="IApplicationComponent"/>s
	/// separated by a splitter.
	/// </summary>
    [AssociateView(typeof(SplitComponentContainerViewExtensionPoint))]
    public class SplitComponentContainer : ApplicationComponentContainer
    {
		/// <summary>
		/// A host for a <see cref="SplitPane"/>.
		/// </summary>
        private class SplitPaneHost : ApplicationComponentHost
        {
            private SplitComponentContainer _owner;

            internal SplitPaneHost(
				SplitComponentContainer owner,
				SplitPane pane)
                :base(pane.Component)
            {
				Platform.CheckForNullReference(owner, "owner");

                _owner = owner;
            }

            #region ApplicationComponentHost overrides

			/// <summary>
			/// Gets the associated desktop window.
			/// </summary>
			public override DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

			/// <summary>
			/// Gets the title displayed in the user-interface.
			/// </summary>
			/// <remarks>
			/// The title cannot be set.
			/// </remarks>
			/// <exception cref="NotSupportedException">The host does not support titles.</exception>
			public override string Title
            {
                get { return _owner.Host.Title; }
                // individual components cannot set the title for the container
                set { throw new NotSupportedException(); }
            }
            
            #endregion
        }


		private SplitPane _pane1;
		private SplitPane _pane2;
		private SplitOrientation _splitOrientation;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SplitComponentContainer(
			SplitPane pane1, 
			SplitPane pane2, 
			SplitOrientation splitOrientation)
        {
			this.Pane1 = pane1;
			this.Pane2 = pane2;

			_splitOrientation = splitOrientation;
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        public SplitComponentContainer(SplitOrientation splitOrientation)
        {
            _splitOrientation = splitOrientation;
        }

		/// <summary>
		/// Gets or sets the first <see cref="SplitPane"/>.
		/// </summary>
		public SplitPane Pane1
		{
			get { return _pane1; }
            set
            {
                if(_pane1 != null && _pane1.ComponentHost != null && _pane1.ComponentHost.IsStarted)
					throw new InvalidOperationException(SR.ExceptionCannotSetPaneAfterContainerStarted);

                _pane1 = value;
                _pane1.ComponentHost = new SplitPaneHost(this, _pane1);
            }
		}

		/// <summary>
		/// Gets or sets the second <see cref="SplitPane"/>.
		/// </summary>
		public SplitPane Pane2
		{
			get { return _pane2; }
            set
            {
                if (_pane2 != null && _pane2.ComponentHost != null && _pane2.ComponentHost.IsStarted)
					throw new InvalidOperationException(SR.ExceptionCannotSetPaneAfterContainerStarted);

                _pane2 = value;
                _pane2.ComponentHost = new SplitPaneHost(this, _pane2);
            }
        }

		/// <summary>
		/// Gets the <see cref="SplitOrientation"/> of the container.
		/// </summary>
		public SplitOrientation SplitOrientation
		{
			get { return _splitOrientation; }
        }

        #region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		///  <remarks>
		/// <para>
		/// Calls <see cref="ApplicationComponent.Start"/> on both of the <see cref="SplitPane"/>s.
		/// </para>
		/// <para>
		/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
		/// </para>
		/// </remarks>
		public override void Start()
        {
			base.Start();

			_pane1.ComponentHost.StartComponent();
            _pane2.ComponentHost.StartComponent();
        }

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Calls <see cref="ApplicationComponent.Stop"/> on both of the <see cref="SplitPane"/>s.
		/// </para>
		/// <para>
		/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
		/// </para>
		/// </remarks>
		public override void Stop()
        {
            _pane1.ComponentHost.StopComponent();
            _pane2.ComponentHost.StopComponent();

            base.Stop();
        }

		/// <summary>
		/// Returns the set of actions that the component wishes to export to the desktop.
		/// </summary>
		/// <remarks>
		/// The <see cref="IActionSet"/> returned by this method is the union of the 
		/// exported actions from the two <see cref="SplitPane"/>s.
		/// </remarks>
		public override IActionSet ExportedActions
        {
            get
            {
                // export the actions from both subcomponents
                return _pane1.Component.ExportedActions.Union(_pane2.Component.ExportedActions);
            }
        }

        #endregion

        #region ApplicationComponentContainer overrides

		/// <summary>
		/// Gets an enumeration of the contained components.
		/// </summary>
		/// <remarks>
		/// Simply returns both <see cref="SplitPane"/>s.
		/// </remarks>
		public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get { return new IApplicationComponent[] { _pane1.Component, _pane2.Component }; }
        }

		/// <summary>
		/// Gets an enumeration of the components that are currently visible.
		/// </summary>
		/// <remarks>
		/// Simply returns both <see cref="SplitPane"/>s, since they are always visible.
		/// </remarks>
		public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get { return this.ContainedComponents; }
        }

		/// <summary>
		/// Ensures that the specified component is visible.
		/// </summary>
		/// <remarks>
		/// Does nothing because both <see cref="SplitPane"/>s are already visible.
		/// </remarks>
		public override void EnsureVisible(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted components are started by default
        }

		/// <summary>
		/// Ensures that the specified component has been started.
		/// </summary>
		/// <remarks>
		/// Does nothing because both <see cref="SplitPane"/>s are already started.
		/// </remarks>
		public override void EnsureStarted(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted components are visible by default
        }

        #endregion
    }
}
