using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Controls.WinForms;

using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Forms;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class DesktopForm : DotNetMagicForm
    {
        private ActionModelNode _menuModel;
        private ActionModelNode _toolbarModel;

		private DockingManager _dockingManager;

        public DesktopForm()
        {
			if (SplashScreen.SplashForm != null)
				SplashScreen.SplashForm.Owner = this;
			
			InitializeComponent();

            _dockingManager = new DockingManager(_toolStripContainer.ContentPanel, VisualStyle.IDE2005);
            _dockingManager.ActiveColor = SystemColors.Control;
            _dockingManager.InnerControl = _tabbedGroups;
			_dockingManager.TabControlCreated += new DockingManager.TabControlCreatedHandler(OnDockingManagerTabControlCreated);

			_tabbedGroups.DisplayTabMode = DisplayTabModes.HideAll;
			_tabbedGroups.TabControlCreated += new TabbedGroups.TabControlCreatedHandler(OnTabbedGroupsTabControlCreated);

			if (_tabbedGroups.ActiveLeaf != null)
			{
				InitializeTabControl(_tabbedGroups.ActiveLeaf.TabControl);
			}
        }

        #region Public properties

        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
                BuildToolStrip(ToolStripBuilder.ToolStripKind.Menu, _mainMenu, _menuModel);
            }
        }

        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;
                BuildToolStrip(ToolStripBuilder.ToolStripKind.Toolbar, _toolbar, _toolbarModel);
            }
        }

        public TabbedGroups TabbedGroups
        {
            get { return _tabbedGroups; }
        }

        public DockingManager DockingManager
        {
            get { return _dockingManager; }
        }

        #endregion

        #region Form event handlers

        private void OnTabbedGroupsTabControlCreated(TabbedGroups tabbedGroups, Crownwood.DotNetMagic.Controls.TabControl tabControl)
        {
            InitializeTabControl(tabControl);
        }

        private void OnDockingManagerTabControlCreated(Crownwood.DotNetMagic.Controls.TabControl tabControl)
        {
            InitializeTabControl(tabControl);
        }


        #endregion

        #region Window Settings

        internal void LoadWindowSettings()
		{
			Rectangle screenRectangle = Screen.PrimaryScreen.Bounds;

			// If the bounds of the primary screen is different from what was saved
			// either because there was a screen resolution change or because the app
			// is being started for the first time, get Windows to properly position the window.
			if (screenRectangle != Settings.PrimaryScreenRectangle)
			{
				// Make the window size 75% of the primary screen
				float scale = 0.75f;
				this.Width = (int) (screenRectangle.Width * scale);
				this.Height = (int) (screenRectangle.Height * scale);

				// Center the window (for some reason, FormStartPosition.CenterScreen doesn't seem
				// to work.)
				//int x = (screenRectangle.Width - this.Width) / 2;
				//int y = (screenRectangle.Height - this.Height) / 2;
				//this.Location = new Point(x, y);
                this.StartPosition = FormStartPosition.CenterScreen;
			}
			else
			{
				this.Location = Settings.WindowRectangle.Location;
				this.Size = Settings.WindowRectangle.Size;
                this.StartPosition = FormStartPosition.Manual;
			}

			// If window was last closed when minimized, don't open it up minimized,
			// but rather just open it normally
			if (Settings.WindowState == FormWindowState.Minimized)
				this.WindowState = FormWindowState.Normal;
			else
				this.WindowState = Settings.WindowState;
		}

		internal void SaveWindowSettings()
		{
			// If the window state is normal, just save its location and size
			if (this.WindowState == FormWindowState.Normal)
				Settings.WindowRectangle = new Rectangle(this.Location, this.Size);
			// But, if it's minimized or maximized, save the restore bounds instead
			else
				Settings.WindowRectangle = this.RestoreBounds;

			Settings.WindowState = this.WindowState;
			Settings.PrimaryScreenRectangle = Screen.PrimaryScreen.Bounds;
			Settings.Save();
        }

        #endregion

        #region Helper methods

        internal DesktopViewSettings Settings
        {
            get
            {
                return DesktopViewSettings.Default;
            }
        }

        private void InitializeTabControl(Crownwood.DotNetMagic.Controls.TabControl tabControl)
		{
			if (tabControl == null)
				return;

			tabControl.TextTips = true;
			tabControl.ToolTips = false;
			tabControl.MaximumHeaderWidth = 256;
        }

        private void BuildToolStrip(ToolStripBuilder.ToolStripKind kind, ToolStrip toolStrip, ActionModelNode actionModel)
        {
            // avoid flicker
            toolStrip.SuspendLayout();
            // very important to clean up the existing ones first
            ToolStripBuilder.Clear(toolStrip.Items);

            if (actionModel != null)
            {
                ToolStripBuilder.BuildToolStrip(kind, toolStrip.Items, actionModel.ChildNodes);
            }

            toolStrip.ResumeLayout();
        }

        #endregion
    }
}
