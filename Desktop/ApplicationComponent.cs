using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Validation;

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
    public abstract class ApplicationComponent : IApplicationComponent, INotifyPropertyChanged, IDataErrorInfo
    {
        /// <summary>
        /// Executes the specified application component in a new workspace.  The exit callback will be invoked
        /// when the workspace is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from it's <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the workspace will run</param>
        /// <param name="component">The application component to launch</param>
        /// <param name="title">The title of the workspace</param>
        /// <param name="exitCallback">The callback to invoke when the workspace is closed</param>
        /// <returns>The workspace that is hosting the component</returns>
        public static Workspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ApplicationComponentExitDelegate exitCallback)
        {
            return LaunchAsWorkspace(desktopWindow, component, title, null, exitCallback);
        }

        public static Workspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            string name,
            ApplicationComponentExitDelegate exitCallback)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(component, "component");

            WorkspaceCreationArgs args = new WorkspaceCreationArgs(component, title, name);
            Workspace workspace = desktopWindow.Workspaces.AddNew(args);
            if (exitCallback != null)
            {
                workspace.Closed += delegate(object sender, ClosedEventArgs e)
                {
                    exitCallback(component);
                };
            }
            return workspace;
        }


        /// <summary>
        /// Executes the specified application component in a new shelf.  The exit callback will be invoked
        /// when the shelf is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from it's <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run</param>
        /// <param name="component">The application component to launch</param>
        /// <param name="title">The title of the shelf</param>
        /// <param name="displayHint">A hint as to how the shelf should initially be displayed</param>
        /// <param name="exitCallback">The callback to invoke when the shelf is closed</param>
        /// <returns>The shelf that is hosting the component</returns>
        public static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ShelfDisplayHint displayHint,
            ApplicationComponentExitDelegate exitCallback)
        {
            return LaunchAsShelf(desktopWindow, component, title, null, displayHint, exitCallback);
        }

        public static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            string name,
            ShelfDisplayHint displayHint,
            ApplicationComponentExitDelegate exitCallback)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(component, "component");

            ShelfCreationArgs args = new ShelfCreationArgs(component, title, name, displayHint);
            Shelf shelf = desktopWindow.Shelves.AddNew(args);
            if (exitCallback != null)
            {
                shelf.Closed += delegate(object sender, ClosedEventArgs e)
                {
                    exitCallback(component);
                };
            }
            return shelf;
        }

        /// <summary>
        /// Executes the specified application component in a modal dialog box.  This call will block until
        /// the dialog box is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from it's <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run</param>
        /// <param name="component">The application component to launch</param>
        /// <param name="title">The title of the shelf</param>
        /// <returns></returns>
        public static ApplicationComponentExitCode LaunchAsDialog(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title)
        {
            return LaunchAsDialog(desktopWindow, component, title, null);
        }

        public static ApplicationComponentExitCode LaunchAsDialog(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            string name)
        {
            DialogBoxCreationArgs args = new DialogBoxCreationArgs(component, title, name);
            desktopWindow.ShowDialogBox(args);
            return component.ExitCode;
        }

        private IApplicationComponentHost _host;
        private ApplicationComponentExitCode _exitCode;

        private bool _started;
        private bool _modified;
        private event EventHandler _modifiedChanged;

        private event EventHandler _allPropertiesChanged;
        private event PropertyChangedEventHandler _propertyChanged;

        private ValidationRuleSet _validation;
        private bool _showValidationErrors;
        private event EventHandler _showValidationErrorsChanged;


        /// <summary>
        /// Constructor
        /// </summary>
        protected ApplicationComponent()
        {
            _exitCode = ApplicationComponentExitCode.Normal;    // default exit code
            
            // default empty validation rule set
            _validation = new ValidationRuleSet();
        }

        /// <summary>
        /// Provides subclasses with access to the host
        /// </summary>
        protected IApplicationComponentHost Host
        {
            get
            {
                return _host;
            }
        }

        /// <summary>
        /// Convenience method for use by subclasses to set the exit code and ask the host to exit in a single call.
        /// </summary>
        /// <param name="exitCode"></param>
        protected void Exit(ApplicationComponentExitCode exitCode)
        {
            this.ExitCode = exitCode;
            this.Host.Exit();
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
        protected void NotifyModifiedChanged()
        {
            EventsHelper.Fire(_modifiedChanged, this, EventArgs.Empty);
        }

        /// <summary>
        /// Notifies that the specified property has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies that all properties may have changed.  A view should respond to this event
        /// by refreshing itself completely.
        /// </summary>
        protected void NotifyAllPropertiesChanged()
        {
            EventsHelper.Fire(_allPropertiesChanged, this, EventArgs.Empty);
        }

        public ValidationRuleSet Validation
        {
            get { return _validation; }
            set { _validation = value; }
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
        /// Default implementation of <see cref="IApplicationComponent.Start"/>. Overrides should
        /// be sure to call the base implementation.
        /// </summary>
        public virtual void Start()
        {
            AssertNotStarted();

            _started = true;
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.Stop"/>.  Overrides should
        /// be sure to call the base implementation.
        /// </summary>
        public virtual void Stop()
        {
            AssertStarted();

            _started = false;
        }

        public bool IsStarted
        {
            get { return _started; }
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.CanExit"/>.
        /// </summary>
        /// <remarks>
        /// Checks the <see cref="Modified"/> property, and if true, presents a standard
        /// confirmation dialog to the user asking whether or not changes should be
        /// retained.
        /// </remarks>
        public virtual bool CanExit(UserInteraction interactive)
        {
            AssertStarted();

            if (interactive == UserInteraction.NotAllowed)
                return !_modified;

            if (_modified)
            {
				DialogBoxAction result = this.Host.ShowMessageBox(SR.MessageConfirmSaveChangesBeforeClosing, MessageBoxActions.YesNoCancel);
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
                return true;
            }
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
                    NotifyModifiedChanged();
                }
            }
        }

        public event EventHandler AllPropertiesChanged
        {
            add { _allPropertiesChanged += value; }
            remove { _allPropertiesChanged -= value; }
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

        public virtual bool HasValidationErrors
        {
            get
            {
                AssertStarted();

                return this.Validation.GetResults(this).FindAll(delegate(ValidationResult r) { return !r.Success; }).Count > 0;
            }
        }

        public virtual void ShowValidation(bool show)
        {
            AssertStarted();

            _showValidationErrors = show;
            EventsHelper.Fire(_showValidationErrorsChanged, this, EventArgs.Empty);
        }

        public bool ValidationVisible
        {
            get { return _showValidationErrors; }
        }

        public event EventHandler ValidationVisibleChanged
        {
            add { _showValidationErrorsChanged += value; }
            remove { _showValidationErrorsChanged -= value; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (_showValidationErrors && _validation != null)
                {
                    ValidationResult result = _validation.GetResults(this, propertyName).Find(
                        delegate(ValidationResult r) { return !r.Success; });

                    return result == null ? null : result.GetMessageString("\n");
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Helper methods

        private void AssertStarted()
        {
            if (!_started)
                throw new InvalidOperationException(SR.ExceptionComponentNeverStarted);
        }

        private void AssertNotStarted()
        {
            if (_started)
                throw new InvalidOperationException(SR.ExceptionComponentAlreadyStarted);
        }

        #endregion
    }
}
