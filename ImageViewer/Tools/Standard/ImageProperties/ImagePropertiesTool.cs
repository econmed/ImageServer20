﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarImageProperties", "Show", KeyStroke = XKeys.Control | XKeys.P)]
	[MenuAction("show", "global-menus/MenuView/MenuImageProperties", "Show", KeyStroke = XKeys.Control | XKeys.P)]
	[MenuAction("show", "imageviewer-contextmenu/MenuImageProperties", "Show")]
	[Tooltip("show", "TooltipImageProperties")]
	[IconSet("show", IconScheme.Colour, "ImagePropertiesToolSmall.png", "ImagePropertiesToolMedium.png", "ImagePropertiesToolLarge.png")]
	[GroupHint("show", "Application.View.ImageProperties")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ImagePropertiesTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();
		
		public ImagePropertiesTool()
		{
		}

		private IShelf ComponentShelf
		{
			get
			{
				if (_shelves.ContainsKey(Context.DesktopWindow))
					return _shelves[Context.DesktopWindow];

				return null;
			}
		}

		public void Show()
		{
			if (ComponentShelf == null)
			{
				try
				{
					IDesktopWindow desktopWindow = Context.DesktopWindow;
					
					ImagePropertiesApplicationComponent component =
						new ImagePropertiesApplicationComponent(Context.DesktopWindow);

					IShelf shelf = ApplicationComponent.LaunchAsShelf(Context.DesktopWindow, component,
						SR.TitleImageProperties, "ImageProperties", ShelfDisplayHint.DockLeft);

					_shelves.Add(Context.DesktopWindow, shelf);
					shelf.Closed += delegate { _shelves.Remove(desktopWindow); };
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, Context.DesktopWindow);
				}
			}
			else
			{
				ComponentShelf.Show();
			}
		}
	}
}