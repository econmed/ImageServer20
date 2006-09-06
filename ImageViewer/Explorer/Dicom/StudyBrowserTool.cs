using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class StudyBrowserTool : Tool<IStudyBrowserToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChangedEvent;

		public StudyBrowserTool()
		{

		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
		}

		void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			if (this.Context.SelectedStudy != null)
				this.Enabled = true;
			else
				this.Enabled = false;
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChangedEvent += value; }
			remove { _enabledChangedEvent -= value; }
		}
	}
}
