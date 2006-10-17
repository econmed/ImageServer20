using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("launch", "global-menus/Go/Home")]
    [ButtonAction("launch", "global-toolbars/Go/Home")]
    [Tooltip("launch", "Home")]
    [IconSet("launch", IconScheme.Colour, "Icons.HomeToolSmall.png", "Icons.HomeToolMedium.png", "Icons.HomeToolLarge.png")]
    [ClickHandler("launch", "Launch")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class HomeTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {

        private IWorkspace _workspace;

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomeTool()
        {
        }


        public void Launch()
        {
            if (_workspace == null)
            {
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    BuildComponent(),
                    "Home",
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }

        public IApplicationComponent BuildComponent()
        {
            WorklistComponent resultComponent = new WorklistComponent();
            FoldersComponent foldersComponent = new FoldersComponent(resultComponent);
            PatientProfilePreviewComponent previewComponent = new PatientProfilePreviewComponent();

            resultComponent.SelectedPatientChanged +=
                delegate(object sender, EventArgs e)
                {
                    previewComponent.Subject = resultComponent.SelectedPatient;
                };

            SplitComponentContainer container = new SplitComponentContainer(
                new SplitPane("Results", resultComponent, 0.45f),
                new SplitPane("Preview", previewComponent, 0.55f),
                SplitOrientation.Vertical);

            return new SplitComponentContainer(
                new SplitPane("Search", foldersComponent, 0.15f),
                new SplitPane("Results", container, 0.85f),
                SplitOrientation.Vertical);
        }
    }
}
