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

using System.Collections.Generic;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for application components that act as containers for other application components.
    /// </summary>
    public abstract class ApplicationComponentContainer : ApplicationComponent, IApplicationComponentContainer
    {
        private IApplicationComponentContainerValidationStrategy _validationStrategy;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ApplicationComponentContainer()
        {
            _validationStrategy = new NoNodesContainerValidationStrategy();
        }

        /// <summary>
        /// Gets or sets the validation strategy that determines how this container responds
        /// to validation requests.
        /// </summary>
        public IApplicationComponentContainerValidationStrategy ValidationStrategy
        {
            get { return _validationStrategy; }
            set { _validationStrategy = value; }
        }

        /// <summary>
        /// Gets a value indicating whether there are any data validation errors.
        /// </summary>
        /// <remarks>
        /// The default implementation of this property delegates to the <see cref="ValidationStrategy"/> object.
        /// Invoking this property may cause any unstarted components in the container to be started,
        /// which means that it may throw exceptions.
        /// </remarks>
        public override bool HasValidationErrors
        {
            get
            {
                return _validationStrategy.HasValidationErrors(this);
            }
        }

        /// <summary>
		/// Sets the <see cref="ApplicationComponent.ValidationVisible"/> property and raises the 
		/// <see cref="ApplicationComponent.ValidationVisibleChanged"/> event.
        /// </summary>
        /// <remarks>
        /// The default implementation of this property delegates to the <see cref="ValidationStrategy"/> object.
        /// Invoking this property may cause any unstarted components in the container to be started,
        /// which means that it may throw exceptions.
        /// </remarks>
        /// <param name="show"></param>
        public override void ShowValidation(bool show)
        {
            _validationStrategy.ShowValidation(this, show);
        }

        #region IApplicationComponentContainer Members

        /// <summary>
        /// Gets an enumeration of the contained components.
        /// </summary>
        public abstract IEnumerable<IApplicationComponent> ContainedComponents { get; }

        /// <summary>
        /// Gets an enumeration of the components that are currently visible.
        /// </summary>
        public abstract IEnumerable<IApplicationComponent> VisibleComponents { get; }

        /// <summary>
        /// Ensures that the specified component is visible.
        /// </summary>
        public abstract void EnsureVisible(IApplicationComponent component);

        /// <summary>
        /// Ensures that the specified component has been started.
        /// </summary>
        public abstract void EnsureStarted(IApplicationComponent component);

        #endregion
    }
}
