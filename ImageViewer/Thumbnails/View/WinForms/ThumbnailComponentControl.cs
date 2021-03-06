﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using System;

namespace ClearCanvas.ImageViewer.Thumbnails.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ThumbnailComponent"/>.
    /// </summary>
    public partial class ThumbnailComponentControl : ApplicationComponentUserControl
    {
        private ThumbnailComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ThumbnailComponentControl(ThumbnailComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_galleryView.DataSource = _component.Thumbnails;

        	_imageSetTree.SelectionChanged += 
				delegate
            	{
            		_component.TreeSelection = _imageSetTree.Selection;
            	};

        	_imageSetTree.TreeBackColor = Color.FromKnownColor(KnownColor.Black);
			_imageSetTree.TreeForeColor = Color.FromKnownColor(KnownColor.ControlLight);
			_imageSetTree.TreeLineColor = Color.FromKnownColor(KnownColor.ControlLight);

			_component.PropertyChanged += OnPropertyChanged;

			_imageSetTree.Tree = _component.Tree;
        	_imageSetTree.VisibleChanged += OnTreeVisibleChanged;
		}

		private void OnTreeVisibleChanged(object sender, EventArgs e)
		{
			_imageSetTree.VisibleChanged -= OnTreeVisibleChanged;
			//the control isn't really visible until it's been drawn, so the selection can't be set until then.
			_imageSetTree.Selection = _component.TreeSelection;
		}

    	private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Tree")
			{
				_imageSetTree.Tree = _component.Tree;
			}
			else if (e.PropertyName == "TreeSelection")
			{
				_imageSetTree.Selection = _component.TreeSelection;
			}
		}
    }
}
