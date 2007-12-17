#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardStack/Activate", "Select", KeyStroke = XKeys.S)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.StackToolSmall.png", "Icons.StackToolMedium.png", "Icons.StackToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Standard")]

	[MouseWheelHandler(ModifierFlags.None)]
	[MouseToolButton(XMouseButtons.Left, true)]

	[KeyboardAction("stackup", "imageviewer-keyboard/ToolsStandardStack/StackUp", "StackUp", KeyStroke = XKeys.PageUp)]
	[KeyboardAction("stackdown", "imageviewer-keyboard/ToolsStandardStack/StackDown", "StackDown", KeyStroke = XKeys.PageDown)]
	[KeyboardAction("jumptobeginning", "imageviewer-keyboard/ToolsStandardStack/JumpToBeginning", "JumpToBeginning", KeyStroke = XKeys.Home)]
	[KeyboardAction("jumptoend", "imageviewer-keyboard/ToolsStandardStack/JumpToEnd", "JumpToEnd", KeyStroke = XKeys.End)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class StackTool : MouseImageViewerTool
	{
		private UndoableCommand _command;
		private int _initialPresentationImageIndex;
		private IImageBox _currentImageBox;

		public StackTool()
			: base(SR.TooltipStack)
		{
			this.CursorToken = new CursorToken("Icons.StackToolSmall.png", this.GetType().Assembly);
		}

		public override string Tooltip
		{
			get { return base.Tooltip; }
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		private void CaptureBeginState(IImageBox imageBox)
		{
			_command = new UndoableCommand(imageBox);
			_command.Name = SR.CommandStack;

			// Capture state before stack
			_command.BeginState = imageBox.CreateMemento();
			_currentImageBox = imageBox;

			_initialPresentationImageIndex = imageBox.SelectedTile.PresentationImageIndex;
		}

		private void CaptureEndState()
		{
			if (_command == null || _currentImageBox == null)
			{
				_currentImageBox = null;
				return;
			}

			// If nothing's changed then just return
			if (_initialPresentationImageIndex == _currentImageBox.SelectedTile.PresentationImageIndex)
			{
				_command = null;
				_currentImageBox = null;
				return;
			}

			// Capture state after stack
			_command.EndState = _currentImageBox.CreateMemento();
			if (!_command.EndState.Equals(_command.BeginState)) 
				this.Context.Viewer.CommandHistory.AddCommand(_command);

			_command = null;
			_currentImageBox = null;
		}

		private void JumpToBeginning()
		{
			if (this.Context.Viewer.SelectedTile == null)
				return;

			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = 0;
			imageBox.Draw();
			CaptureEndState();
		}

		private void JumpToEnd()
		{
			if (this.Context.Viewer.SelectedTile == null)
				return;

			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;

			if (imageBox.DisplaySet == null)
				return;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count - 1;
			imageBox.Draw();
			CaptureEndState();
		}

		private void StackUp()
		{
			if (this.Context.Viewer.SelectedTile == null)
				return;

			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(-1, imageBox);
			CaptureEndState();
		}

		private void StackDown()
		{
			if (this.Context.Viewer.SelectedTile == null)
				return;

			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(+1, imageBox);
			CaptureEndState();
		}

		private void AdvanceImage(int increment, IImageBox selectedImageBox)
		{
			selectedImageBox.TopLeftPresentationImageIndex += increment;
			selectedImageBox.Draw();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			CaptureBeginState(mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			if (base.DeltaY == 0)
				return true;

			int increment;

			if (base.DeltaY > 0)
				increment = 1;
			else
				increment = -1;

			AdvanceImage(increment, mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			this.CaptureEndState();
		}

		protected override void StartWheel()
		{
			IImageBox imageBox = this.Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
		}

		protected override void WheelDown()
		{
			AdvanceImage(1, this.Context.Viewer.SelectedTile.ParentImageBox);
		}

		protected override void WheelUp()
		{
			AdvanceImage(-1, this.Context.Viewer.SelectedTile.ParentImageBox);
		}

		protected override void StopWheel()
		{
			CaptureEndState();
		}
	}
}
