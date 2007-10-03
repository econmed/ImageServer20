using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarCreate", "Create")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuCreate", "Create")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipCreate")]
	[IconSet("activate", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png")]
    [ExtensionOf(typeof(DicomEditorToolExtensionPoint))]
    class CreateTool : Tool<DicomEditorComponent.DicomEditorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public CreateTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = true;
            this.Context.SelectedTagChanged += new EventHandler(OnSelectedTagChanged);
        }

        public bool Enabled
        {
            get { return _enabled; }
            protected set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Notifies that the Enabled state of this tool has changed.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        private void Create()
        {
            DicomEditorCreateToolComponent creator = new DicomEditorCreateToolComponent();
            ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, creator, SR.TitleCreateTag);
            if (result == ApplicationComponentExitCode.Normal)
            {
                try
                {
                    this.Context.DumpManagement.EditTag(creator.TagId, creator.Value, false);
                }
                catch (DicomException)
                {
                    this.Context.DesktopWindow.ShowMessageBox(SR.MessageTagCannotBeCreated, MessageBoxActions.Ok);
                    return;
                }

                this.Context.UpdateDisplay();
            }
        }

        private void OnSelectedTagChanged(object sender, EventArgs e)
        {
            if (this.Context.SelectedTags != null && this.Context.SelectedTags.Count > 1)
                this.Enabled = false;
            else
                this.Enabled = true;
        }   
    }
}
