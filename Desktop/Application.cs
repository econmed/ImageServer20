using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Threading;
using System.Security.Principal;
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Desktop
{
    #region Extension Points

    /// <summary>
    /// Defines an extension point for providing an implementation of <see cref="IGuiToolkit"/>.
    /// The application requires one extension of this point.
    /// </summary>
    [ExtensionPoint]
    public class GuiToolkitExtensionPoint : ExtensionPoint<IGuiToolkit>
    {
    }

    /// <summary>
    /// Defines an extension point for providing an optional implementation of <see cref="ISessionManager"/>.
    /// </summary>
    /// <remarks>
    /// The framework will use one extension of this point if found, but no extension is required.</remarks>
    [ExtensionPoint()]
    public class SessionManagerExtensionPoint : ExtensionPoint<ISessionManager>
    {
    }

    /// <summary>
    /// Defines an extension point for a view onto the application.
    /// </summary>
    /// <remarks>
    /// One extension is required, or the application will not run.
    /// </remarks>
    [ExtensionPoint]
    public class ApplicationViewExtensionPoint : ExtensionPoint<IApplicationView>
    {
    }

    /// <summary>
    /// Tool context interface for tools that extend <see cref="ApplicationToolExtensionPoint"/>.
    /// </summary>
    public interface IApplicationToolContext : IToolContext
    {
    }

    /// <summary>
    /// Defines an extension point for application tools, which are global to the application.
    /// </summary>
    /// <remarks>
    /// Application tools are global to the application. An application tool is instantiated exactly once.
    /// Application tools cannot have actions because they are not associated with any UI entity.
    /// Extensions should expect to recieve a tool context of type <see cref="IApplicationToolContext"/>.
    /// </remarks>
    [ExtensionPoint]
    public class ApplicationToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    #endregion

    /// <summary>
    /// Singleton class that represents the desktop application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class extends <see cref="ApplicationRootExtensionPoint"/> and provides the implementation of
    /// <see cref="IApplicationRoot"/> for a desktop application.  This class may be subclassed if necessary.
    /// In order for the framework to use the subclass, it must be passed to <see cref="Platform.StartApp"/>.
    /// (Typically this is done by passing the class name as a command line argument to the executable).
    /// </para>
    /// <para>
    /// The class provides a number of static convenience methods that may be freely used by application code.
    /// These static members should not be considered thread-safe.
    /// </para>
    /// <para>
    /// The <see cref="Instance"/> property can be used to obtain the singleton instance of the class (or subclass).
    /// </para>
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    [AssociateView(typeof(ApplicationViewExtensionPoint))]
    public class Application : IApplicationRoot
    {
        #region Public Static Members

        private static Application _instance;

        /// <summary>
        /// Gets the singleton instance of the <see cref="Application"/> object
        /// </summary>
        public static Application Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the toolkit ID of the currently loaded GUI <see cref="IGuiToolkit"/>,
        /// or null if the toolkit has not been loaded yet.
        /// </summary>
        public static string GuiToolkitID
        {
            get { return _instance.GuiToolkit != null ? _instance.GuiToolkit.ToolkitID : null; }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public static string Name
        {
            get { return _instance.ApplicationName; }
        }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public static Version Version
        {
            get { return _instance.ApplicationVersion; }
        }

        /// <summary>
        /// Gets the collection of application windows.
        /// </summary>
        public static DesktopWindowCollection DesktopWindows
        {
            get { return _instance.Windows; }
        }

        /// <summary>
        /// Gets the currently active window.
        /// </summary>
        /// <value>The active window, or null if no windows have been created.</value>
        public static DesktopWindow ActiveDesktopWindow
        {
            get { return DesktopWindows.ActiveWindow; }
        }

        /// <summary>
        /// Shows a message box using the application name as the title.
        /// </summary>
        /// <remarks>
        /// It is preferable to use <see cref="ClearCanvas.Desktop.DesktkopWindow.ShowMessageBox"/> if a desktop window
        /// is available, since that method will ensure that the message box window is associated with
        /// the parent desktop window. This method is provided for situations where a message box needs to 
        /// be displayed prior to the creation of any desktop windows.
        /// </remarks>
        /// <param name="message">The message to display</param>
        /// <param name="actions">The actions that the user may take</param>
        /// <returns>The resulting action taken by the user</returns>
        public static DialogBoxAction ShowMessageBox(string message, MessageBoxActions actions)
        {
            return _instance.View.ShowMessageBox(message, actions);
        }

        /// <summary>
        /// Attempts to close all open desktop windows and terminate the application.
        /// </summary>
        /// <remarks>
        /// The request to quit is not guaranteed to succeed.  Specifically, it will fail if an
        /// open workspace demands user-interaction in order to close, in which case the user may
        /// cancel the operation.  The request may also be cancelled programmatically, by handlers
        /// of the <see cref="Quitting"/> event.
        /// </remarks>
        /// <returns>True if the application successfully quits, or false if it does not.</returns>
        public static bool Quit()
        {
            return _instance.DoQuit();
        }

        /// <summary>
        /// Occurs when a request has been made for the application to quit.
        /// </summary>
        public static event EventHandler<QuittingEventArgs> Quitting
        {
            add { _instance._quitting += value; }
            remove { _instance._quitting -= value; }
        }

        /// <summary>
        /// Occurs when the application has quit, just before the process terminates.
        /// </summary>
        public static event EventHandler Quitted
        {
            add { _instance._quitted += value; }
            remove { _instance._quitted -= value; }
        }


        #endregion

        #region ApplicationToolContext

        class ApplicationToolContext : ToolContext, IApplicationToolContext
        {
            internal ApplicationToolContext(Application application)
            {

            }
        }

        #endregion

        #region Default Session Manager Implementation

        class DefaultSessionManager : ISessionManager
        {
            #region ISessionManager Members

            public bool InitiateSession()
            {
                // do nothing
                return true;
            }

            public void TerminateSession()
            {
                // do nothing
            }

            #endregion
        }

        #endregion

        private string _appName;
        private Version _appVersion;
        private IGuiToolkit _guiToolkit;
        private IApplicationView _view;
        private DesktopWindowCollection _windows;
        private ToolSet _toolSet;
        private ISessionManager _sessionManager;

        private bool _initialized;  // flag to be set when initialization is complete

        private event EventHandler<QuittingEventArgs> _quitting;
        private event EventHandler _quitted;


        /// <summary>
        /// Default constructor, for internal framework use only.
        /// </summary>
        public Application()
        {
            _instance = this;
        }

        #region IApplicationRoot members

        /// <summary>
        /// Implementation of <see cref="IApplicationRoot.RunApplication"/>.  Runs the application.
        /// </summary>
        void IApplicationRoot.RunApplication(string[] args)
        {
            try
            {
                Run(args);
            }
            finally
            {
                CleanUp();
            }
        }

        #endregion

        #region Protected overridables

        /// <summary>
        /// Initializes the application. Override this method to perform custom initialization.
        /// </summary>
        /// <remarks>
        /// Initializes the application, including the session manager, application tools and root window.
        /// The GUI toolkit and application view have already been initialized prior to this method being
        /// called.
        /// </remarks>
        /// <param name="args">Arguments passed in from the command line.</param>
        /// <returns>True if initialization was successful, false if the application should terminate immediately.</returns>
        protected virtual bool Initialize(string[] args)
        {
            // initialize session
            if (!InitializeSessionManager())
                return false;

            // load tools
            _toolSet = new ToolSet(new ApplicationToolExtensionPoint(), new ApplicationToolContext(this));

            // create a root window
            _windows.AddNew("Root");

            return true;
        }

        /// <summary>
        /// Called after the GUI toolkit message loop terminates, to clean up the application.  Override
        /// this method to perform custom clean-up.  Be sure to call the base class method.
        /// </summary>
        protected virtual void CleanUp()
        {
            if (_view != null && _view is IDisposable)
            {
                (_view as IDisposable).Dispose();
                _view = null;
            }

            if (_toolSet != null)
            {
                _toolSet.Dispose();
                _toolSet = null;
            }

            if (_windows != null)
            {
                (_windows as IDisposable).Dispose();
            }

            if (_guiToolkit != null && _guiToolkit is IDisposable)
            {
                (_guiToolkit as IDisposable).Dispose();
            }
        }

        /// <summary>
        /// Raises the <see cref="Quitting"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnQuitting(QuittingEventArgs args)
        {
            EventsHelper.Fire(_quitting, this, args);
        }

        /// <summary>
        /// Raises the <see cref="Quitted"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnQuitted(EventArgs args)
        {
            EventsHelper.Fire(_quitted, this, args);
        }

        /// <summary>
        /// Gets the display name for the application. Override this method to provide a custom display name.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetName()
        {
            return SR.ApplicationName;
        }

        /// <summary>
        /// Gets the version of the application, which is by default the version of this assembly.
        /// Override this method to provide custom version information.
        /// </summary>
        /// <returns></returns>
        protected virtual Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
        
        #endregion

        #region Protected members

        /// <summary>
        /// Closes all desktop windows.
        /// </summary>
        /// <returns></returns>
        protected bool CloseAllWindows()
        {
            // make a copy of the windows collection for iteration
            List<DesktopWindow> windows = new List<DesktopWindow>(_windows);

            foreach (DesktopWindow window in windows)
            {
                // try to close the window
                bool closed = window.Close(UserInteraction.Allowed, CloseReason.ApplicationQuit);

                // if one fails, abort
                if (!closed)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the collection of desktop windows.
        /// </summary>
        protected DesktopWindowCollection Windows
        {
            get { return _windows; }
        }

        /// <summary>
        /// Gets the GUI toolkit.
        /// </summary>
        protected IGuiToolkit GuiToolkit
        {
            get { return _guiToolkit; }
        }

        /// <summary>
        /// Gets the application view.
        /// </summary>
        protected IApplicationView View
        {
            get { return _view; }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Implements the logic to start up the desktop by running the GUI toolkit and creating the application view.
        /// </summary>
        /// <param name="args"></param>
        private void Run(string[] args)
        {
            // load gui toolkit
            try
            {
                _guiToolkit = (IGuiToolkit)(new GuiToolkitExtensionPoint()).CreateExtension();
            }
            catch (Exception ex)
            {
                Platform.Log(ex, LogLevel.Fatal);
                return;
            }

            _guiToolkit.Started += delegate(object sender, EventArgs e)
            {
                // load application view
                try
                {
                    _view = (IApplicationView)ViewFactory.CreateAssociatedView(this.GetType());
                }
                catch (Exception ex)
                {
                    Platform.Log(ex, LogLevel.Fatal);
                    _guiToolkit.Terminate();
                    return;
                }

                // initialize
                if (!Initialize(args))
                {
                    _guiToolkit.Terminate();
                    return;
                }

                _initialized = true;
            };

            // init windows collection
            _windows = new DesktopWindowCollection(this);
            _windows.ItemClosed += delegate(object sender, ClosedItemEventArgs<DesktopWindow> e)
            {
                // terminate the app when the window count goes to 0 if the app isn't already quitting
                if (_windows.Count == 0 && e.Reason != CloseReason.ApplicationQuit)
                {
                    DoQuit();
                }
            };


            // start message pump - this will block until _guiToolkit.Terminate() is called
            _guiToolkit.Run();
        }
        
        /// <summary>
        /// Implements the logic to terminate the desktop, including closing all windows and terminating the session.
        /// </summary>
        /// <returns>True if the application is really going to terminate, false otherwise</returns>
        private bool DoQuit()
        {
            if (!_initialized)
                throw new InvalidOperationException("This method cannot be called until the Application is fully initialized");

            QuittingEventArgs args = new QuittingEventArgs(UserInteraction.Allowed, false);
            OnQuitting(args);
            if(args.Cancel)
                return false;

            if (!CloseAllWindows())
                return false;

            OnQuitted(EventArgs.Empty);

            try
            {
               _sessionManager.TerminateSession();
            }
            catch (Exception e)
            {
                Platform.Log(e);
            }

            // shut down the GUI message loop
            _guiToolkit.Terminate();

            return true;
        }

        /// <summary>
        /// Initializes the session manager, using an extension if one is provided.
        /// </summary>
        /// <returns></returns>
        private bool InitializeSessionManager()
        {
            try
            {
                _sessionManager = (ISessionManager)(new SessionManagerExtensionPoint()).CreateExtension();
                Platform.Log(string.Format("Using session manager extension: {0}", _sessionManager.GetType().FullName), LogLevel.Info);
            }
            catch (NotSupportedException)
            {
                _sessionManager = new DefaultSessionManager();
                Platform.Log("No session manager extension found", LogLevel.Info);
            }

            try
            {
                return _sessionManager.InitiateSession();
            }
            catch (Exception ex)
            {
                // log error as fatal
                Platform.Log(ex, LogLevel.Fatal);

                // any exception thrown here should be considered a "false" return value
                return false;
            }
        }

        /// <summary>
        /// Gets the cached application name.
        /// </summary>
        private string ApplicationName
        {
            get
            {
                if (_appName == null)
                    _appName = GetName();
                return _appName;
            }
        }

        /// <summary>
        /// Gets the cached application version.
        /// </summary>
        private Version ApplicationVersion
        {
            get
            {
                if (_appVersion == null)
                    _appVersion = GetVersion();
                return _appVersion;
            }
        }

        /// <summary>
        /// Creates a view for a desktop window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        internal IDesktopWindowView CreateDesktopWindowView(DesktopWindow window)
        {
            return _view.CreateDesktopWindowView(window);
        }

        #endregion
    }
}
