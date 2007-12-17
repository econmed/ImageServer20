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

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProtocollingComponent"/>
    /// </summary>
    public partial class ProtocollingComponentControl : ApplicationComponentUserControl
    {
        private readonly ProtocollingComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocollingComponentControl(ProtocollingComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            Control orderSummary = (Control)_component.OrderSummaryComponentHost.ComponentView.GuiElement;
            orderSummary.Dock = DockStyle.Fill;
            _orderSummaryPanel.Controls.Add(orderSummary);

            Control protocolEditor = (Control)_component.ProtocolEditorComponentHost.ComponentView.GuiElement;
            protocolEditor.Dock = DockStyle.Fill;
            _protocolEditorPanel.Controls.Add(protocolEditor);

            Control orderDetails = (Control)_component.OrderDetailsHost.ComponentView.GuiElement;
            orderDetails.Dock = DockStyle.Fill;
            _orderDetailsPanel.Controls.Add(orderDetails);

        }
    }
}
