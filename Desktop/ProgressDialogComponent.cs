using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="ProgressDialogComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProgressDialogComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProgressDialogComponent class
    /// </summary>
    [AssociateView(typeof(ProgressDialogComponentViewExtensionPoint))]
    public class ProgressDialogComponent : ApplicationComponent
    {
        private BackgroundTask _task;
        private bool _closeDialog;

        private int _progressBar;
        private string _progressMessage;
        private bool _enableCancel;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressDialogComponent(BackgroundTask task, bool closeDialog)
        {
            _task = task;
            _closeDialog = closeDialog;

            _progressBar = 0;
            _progressMessage = "";

            if (_task != null)
                _enableCancel = _task.SupportsCancel;
            else
                _enableCancel = false;
        }

        public override void Start()
        {
            if (_task != null)
            {
                _task.ProgressUpdated += ProgressHandler;
                _task.Terminated += TerminatedHandler;
                _task.Run();
            }

            base.Start();
        }

        public override void Stop()
        {
            // wait for the background task to stop running
            if (_task != null && _task.IsRunning)
                return;

            _task.ProgressUpdated -= ProgressHandler;
            _task.Terminated -= TerminatedHandler;
            base.Stop();
        }

        #region Presentation Model

        public bool EnableCancel
        {
            get { return _enableCancel; }
        }

        public int ProgressBarMaximum
        {
            get { return 100; }
        }

        public int ProgressBar
        {
            get { return _progressBar; }
        }

        public string ProgressMessage
        {
            get { return _progressMessage; }
        }

        public string ButtonText
        {
            get { return (_task != null && _task.IsRunning ? "Cancel" : "Close"); }
        }

        #endregion

        #region Task EventHandler

        private void ProgressHandler(object sender, BackgroundTaskProgressEventArgs e)
        {
            if (_progressMessage != e.Progress.Message)
            {
                _progressMessage = e.Progress.Message;
                SignalMessageChanged();
            }

            if (_progressBar != e.Progress.Percent)
            {
                _progressBar = e.Progress.Percent;
                SignalProgressChanged();
            }
        }

        private void TerminatedHandler(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            // TODO: Insert handling code here, if needed
            //switch (e.Reason)
            //{
            //    case BackgroundTaskTerminatedReason.Exception:
            //    case BackgroundTaskTerminatedReason.Cancelled:
            //    case BackgroundTaskTerminatedReason.Completed:
            //    default:
            //        break;
            //}

            _enableCancel = true;


            if (_closeDialog)
            {
                this.ExitCode = ApplicationComponentExitCode.Cancelled;
                Host.Exit();
            }
            else
            {
                SignalProgressTerminate();
            }

        }

        #endregion

        #region Signal Model Changed

        private void SignalMessageChanged()
        {
            NotifyPropertyChanged("ProgressMessage");
        }

        private void SignalProgressChanged()
        {
            NotifyPropertyChanged("ProgressBar");
        }

        private void SignalProgressTerminate()
        {
            NotifyPropertyChanged("ProgressMessage");
            NotifyPropertyChanged("ProgressBar");
            NotifyPropertyChanged("EnableCancel");
            NotifyPropertyChanged("ButtonText");
        }

        #endregion

        public void Cancel()
        {
            if (_task != null && _task.IsRunning)
            {
                if (_task.SupportsCancel)
                    _task.RequestCancel();
                else
                    return; // should never get here
            }
            else
            {
                this.ExitCode = ApplicationComponentExitCode.Cancelled;
                Host.Exit();
            }
        }
    }
}
