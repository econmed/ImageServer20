using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Defines an application entry point.
    /// </summary>
    /// <seealso cref="ApplicationRootExtensionPoint"/>
    public interface IApplicationRoot
    {
        /// <summary>
        /// Called by the platform to run the application.
        /// </summary>
        /// <remarks>
        /// It is expected that this method may block for the duration of the application's execution, if
        /// for instance, a GUI event message pump is started.
        /// </remarks>
        void RunApplication();
    }
}
