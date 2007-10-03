using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Edit
{
	[MenuAction("undo", "global-menus/MenuEdit/MenuUndo", "Undo", KeyStroke = XKeys.Control | XKeys.Z)]
	[ButtonAction("undo", "global-toolbars/ToolbarStandard/ToolbarUndo", "Undo")]
    [EnabledStateObserver("undo", "UndoEnabled", "UndoEnabledChanged")]
	[IconSet("undo", IconScheme.Colour, "Icons.UndoToolSmall.png", "Icons.UndoToolMedium.png", "Icons.UndoToolLarge.png")]
    [Tooltip("undo", "TooltipUndo")]
	[GroupHint("undo", "Application.Edit.Undo")]

	[MenuAction("redo", "global-menus/MenuEdit/MenuRedo", "Redo", KeyStroke = XKeys.Control | XKeys.Y)]
	[ButtonAction("redo", "global-toolbars/ToolbarStandard/ToolbarRedo", "Redo")]
    [EnabledStateObserver("redo", "RedoEnabled", "RedoEnabledChanged")]
	[IconSet("redo", IconScheme.Colour, "Icons.RedoToolSmall.png", "Icons.RedoToolMedium.png", "Icons.RedoToolLarge.png")]
    [Tooltip("redo", "TooltipRedo")]
	[GroupHint("redo", "Application.Edit.Redo")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class EditHistoryTool : Tool<IDesktopToolContext>
    {
        private bool _undoEnabled;
        private event EventHandler _undoEnabledChangedEvent;

        private bool _redoEnabled;
        private event EventHandler _redoEnabledChangedEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        public EditHistoryTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // need to monitor which workspace is active
            this.Context.DesktopWindow.Workspaces.ItemActivationChanged += WorkspaceActivatedEventHandler;
        }

        public bool UndoEnabled
        {
            get { return _undoEnabled; }
            private set
            {
                if (value != _undoEnabled)
                {
                    _undoEnabled = value;
                    EventsHelper.Fire(_undoEnabledChangedEvent, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler UndoEnabledChanged
        {
            add { _undoEnabledChangedEvent += value; }
            remove { _undoEnabledChangedEvent -= value; }
        }

        public bool RedoEnabled
        {
            get { return _redoEnabled; }
            private set
            {
                if (value != _redoEnabled)
                {
                    _redoEnabled = value;
                    EventsHelper.Fire(_redoEnabledChangedEvent, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler RedoEnabledChanged
        {
            add { _redoEnabledChangedEvent += value; }
            remove { _redoEnabledChangedEvent -= value; }
        }


        public void Undo()
        {
            this.ActiveWorkspace.CommandHistory.Undo();
        }

        public void Redo()
        {
            this.ActiveWorkspace.CommandHistory.Redo();
        }

        private Workspace ActiveWorkspace
        {
            get { return this.Context.DesktopWindow.ActiveWorkspace; }
        }

        private void WorkspaceActivatedEventHandler(object sender, ItemEventArgs<Workspace> e)
        {
            if (e.Item.Active)
            {
                // listen for changes to the command history of this workspace
                e.Item.CommandHistory.CurrentCommandChanged += CurrentCommandChangedEventHandler;
            }
            else
            {
                // stop listening to the previously active workspace
                e.Item.CommandHistory.CurrentCommandChanged -= CurrentCommandChangedEventHandler;
            }

            UpdateEnablement();
        }

        private void CurrentCommandChangedEventHandler(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void UpdateEnablement()
        {
            this.UndoEnabled = (this.ActiveWorkspace != null && this.ActiveWorkspace.CommandHistory.CurrentCommandIndex > -1);
            this.RedoEnabled = (this.ActiveWorkspace != null && this.ActiveWorkspace.CommandHistory.CurrentCommandIndex < this.ActiveWorkspace.CommandHistory.LastCommandIndex);
        }
    }
}
