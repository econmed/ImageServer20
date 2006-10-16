using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public abstract class ApplicationComponentHost : IApplicationComponentHost
    {
        private IApplicationComponent _component;
        private IApplicationComponentView _componentView;
        private ApplicationComponentExitDelegate _exitCallback;

        public ApplicationComponentHost(IApplicationComponent component, ApplicationComponentExitDelegate exitCallback)
        {
            _component = component;
            _exitCallback = exitCallback;
            _component.SetHost(this);
        }

        public ApplicationComponentHost(IApplicationComponent component)
            :this(component, null)
        {
        }


        public virtual void StartComponent()
        {
            if (_component.IsStarted)
                throw new InvalidOperationException("Component has already been started");

            _component.Start();
        }

        public virtual void StopComponent()
        {
            if (!_component.IsStarted)
                throw new InvalidOperationException("Component was never started, or has already been stopped");

            _component.Stop();
            if (_exitCallback != null)
            {
                _exitCallback(_component);
            }
        }

        public bool IsStarted
        {
            get { return _component.IsStarted; }
        }

        public IApplicationComponent Component
        {
            get { return _component; }
        }

        public IApplicationComponentView ComponentView
        {
            get
            {
                if (_componentView == null)
                {
                    _componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_component.GetType());
                    _componentView.SetComponent(_component);
                }
                return _componentView;
            }
        }
	
	

        #region IApplicationComponentHost Members

        /// <summary>
        /// Not supported.  Override this method to add support.
        /// </summary>
        public virtual void Exit()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported. Override this method to add support.
        /// </summary>
        public virtual CommandHistory CommandHistory
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Shows a message box in the associated desktop window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
        {
            return Platform.ShowMessageBox(message, buttons);
        }

        /// <summary>
        /// Not supported. Override this method to add support.
        /// </summary>
        public virtual void SetTitle(string title)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the associated desktop window
        /// </summary>
        public abstract IDesktopWindow DesktopWindow { get; }

        #endregion
    }
}
