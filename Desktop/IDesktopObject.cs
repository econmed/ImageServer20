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

using System;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the public interface to a <see cref="DesktopObject"/>.
    /// </summary>
    public interface IDesktopObject
    {
        /// <summary>
        /// Gets the runtime name of the object, or null if the object is not named.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the title that is presented to the user on the screen.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the current state of the object.
        /// </summary>
        DesktopObjectState State { get; }

        /// <summary>
        /// Activates the object.
        /// </summary>
        void Activate();

        /// <summary>
        /// Tries to close the object, interacting with the user if necessary.
        /// </summary>
        /// <returns>True if the object is closed, otherwise false.</returns>
        bool Close();

        /// <summary>
        /// Tries to close the object, interacting with the user only if specified.
        /// </summary>
        /// <param name="interactive">A value specifying whether user interaction is allowed.</param>
        /// <returns>True if the object is closed, otherwise false.</returns>
        bool Close(UserInteraction interactive);

        /// <summary>
        /// Checks if the object is in a closable state (would be able to close without user interaction).
        /// </summary>
        /// <returns>True if the object can be closed without user interaction.</returns>
        bool QueryCloseReady();

        /// <summary>
        /// Gets a value indicating whether this object is currently active.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Gets a value indicating whether this object is currently visible.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Occurs when the <see cref="Active"/> property changes.
        /// </summary>
        event EventHandler ActiveChanged;

        /// <summary>
        /// Occurs when the <see cref="Visible"/> property changes.
        /// </summary>
        event EventHandler VisibleChanged;

        /// <summary>
        /// Occurs when the <see cref="Title"/> property changes.
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Occurs when the object is about to close.
        /// </summary>
        event EventHandler<ClosingEventArgs> Closing;

        /// <summary>
        /// Occurs when the object has closed.
        /// </summary>
        event EventHandler<ClosedEventArgs> Closed;
    }
}
