using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides a callback when an application component exits
    /// </summary>
    /// <param name="component">The component that exited</param>
    public delegate void ApplicationComponentExitDelegate(IApplicationComponent component);    
    
    /// <summary>
    /// Abstract base class for all application components.  Components should extend this class
    /// rather than implement <see cref="IApplicationComponent"/> directly, as it provides a default
    /// implementation suitable for most situations.
    /// </summary>
    public abstract class ApplicationComponent : IApplicationComponent
    {
        /// <summary>
        /// Executes the specified application component in a new workspace.  The exit callback will be invoked
        /// when the workspace is closed.
        /// </summary>
        /// <param name="desktopWindow">The desktop window in which the workspace will run</param>
        /// <param name="component">The application component to launch</param>
        /// <param name="title">The title of the workspace</param>
        /// <param name="exitCallback">The callback to invoke when the workspace is closed</param>
        /// <returns>The workspace that is hosting the component</returns>
        public static IWorkspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ApplicationComponentExitDelegate exitCallback)
        {
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
			Platform.CheckForNullReference(component, "component");

            IWorkspace workspace = new ApplicationComponentHostWorkspace(component, title, exitCallback);
            desktopWindow.WorkspaceManager.Workspaces.Add(workspace);
            return workspace;
        }

        /// <summary>
        /// Executes the specified application component in a new shelf.  The exit callback will be invoked
        /// when the shelf is closed.
        /// </summary>
        /// <param name="desktopWindow">The desktop window in which the shelf will run</param>
        /// <param name="component">The application component to launch</param>
        /// <param name="title">The title of the shelf</param>
        /// <param name="displayHint">A hint as to how the shelf should initially be displayed</param>
        /// <param name="exitCallback">The callback to invoke when the shelf is closed</param>
        /// <returns>The shelf that is hosting the component</returns>
        public static IShelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ShelfDisplayHint displayHint,
            ApplicationComponentExitDelegate exitCallback)
        {
            IShelf shelf = new ApplicationComponentHostShelf(title, component, displayHint, exitCallback);
            desktopWindow.ShelfManager.Shelves.Add(shelf);
            return shelf;
        }

        /// <summary>
        /// Executes the specified application component in a modal dialog box.  This call will block until
        /// the dialog box is closed.
        /// </summary>
        /// <param name="desktopWindow">The desktop window in which the shelf will run</param>
        /// <param name="component">The application component to launch</param>
        /// <param name="title">The title of the shelf</param>
        /// <returns></returns>
        public static ApplicationComponentExitCode LaunchAsDialog(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title)
        {
            ApplicationComponentHostDialog hostDialog = new ApplicationComponentHostDialog(title, component);
            return hostDialog.RunModal(desktopWindow);
        }

        private IApplicationComponentHost _host;
        private ApplicationComponentExitCode _exitCode;
        private bool _modified;
        private event EventHandler _modifiedChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationComponent()
        {
            _exitCode = ApplicationComponentExitCode.Normal;    // default exit code
        }

        /// <summary>
        /// Provides subclasses with access to the host
        /// </summary>
        protected IApplicationComponentHost Host
        {
            get { return _host; }
        }

        /// <summary>
        /// Convenience method to fire the <see cref="ModifiedChanged"/> event.
        /// Note that it is not necessary to explicitly call this method if the 
        /// default implementation of the <see cref="Modified"/> property is used,
        /// since the event is fired automatically.
        /// 
        /// This method is provided for situations where the subclass has chosen
        /// to override the <see cref="Modified"/> property.
        /// </summary>
        protected void FireModifiedChanged()
        {
            EventsHelper.Fire(_modifiedChanged, this, new EventArgs());
        }

        #region IApplicationComponent Members

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.SetHost"/>
        /// Called by the framework to initialize this component with access to its host
        /// </summary>
        /// <param name="host">The host in which the component is running</param>
        public void SetHost(IApplicationComponentHost host)
        {
			Platform.CheckForNullReference(host, "host");
            _host = host;
        }

        /// <summary>
        /// Returns an empty set of actions.  Subclasses can override this to export
        /// a desired set of actions.
        /// </summary>
        public virtual IActionSet ExportedActions
        {
            get
            {
                return new ActionSet();
            }
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.Start"/>
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.Stop"/>
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.CanExit"/>.
        /// Checks the <see cref="Modified"/> property, and if true, presents a standard
        /// confirmation dialog to the user asking whether or not changes should be
        /// retained.
        /// </summary>
        public virtual bool CanExit()
        {
            if (this.Modified)
            {
                DialogBoxAction result = this.Host.ShowMessageBox("Save changes before closing?", MessageBoxActions.YesNoCancel);
                switch (result)
                {
                    case DialogBoxAction.Yes:
                        this.ExitCode = ApplicationComponentExitCode.Normal;
                        return true;
                    case DialogBoxAction.No:
                        this.ExitCode = ApplicationComponentExitCode.Cancelled;
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                // this is equivalent to cancelling
                this.ExitCode = ApplicationComponentExitCode.Cancelled;
            }
            return true;
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.Modified"/>
        /// Set this property from within the subclass.
        /// </summary>
        public virtual bool Modified
        {
            get { return _modified; }
            protected set
            {
                if (value != _modified)
                {
                    _modified = value;
                    FireModifiedChanged();
                }
            }
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.ModifiedChanged"/>
        /// </summary>
        public event EventHandler ModifiedChanged
        {
            add { _modifiedChanged += value; }
            remove { _modifiedChanged -= value; }
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.ExitCode"/>
        /// Set this property from within the subclass.
        /// </summary>
        public virtual ApplicationComponentExitCode ExitCode
        {
            get { return _exitCode; }
            protected set { _exitCode = value; }
        }

        #endregion
    }
}
