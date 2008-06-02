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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;


[assembly: WebResource("ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.js", "text/javascript")]

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    [ToolboxData("<{0}:ToolbarButton runat=server></{0}:ToolbarButton>")]
    [Themeable(true)]
    public class ToolbarButton : ImageButton, IScriptControl
    {
        #region Public Properties

        /// <summary>
        /// Sets or gets the url of the image to be used when the button is enabled.
        /// </summary>
        public string EnabledImageURL 
        {
            get
            {
                String s = (String)ViewState["EnabledImageURL"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["EnabledImageURL"] = value;
            }
        }

        /// <summary>
        /// Sets or gets the url of the image to be used when the button enabled and user hovers the mouse over the button.
        /// </summary>
        public string EnabledHoverImageURL
        {
            get
            {
                String s = (String)ViewState["EnabledHoverImageURL"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["EnabledHoverImageURL"] = value;
            }
        }

       

        /// <summary>
        /// Sets or gets the url of the image to be used when the button is disabled.
        /// </summary>
        public string DisabledImageURL
        {
            get
            {
                String s = (String)ViewState["DisabledImageURL"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["DisabledImageURL"] = value;
            }
        }

        #endregion Public Properties

        #region Protected Methods


        #endregion Protected Methods


        public override void  RenderControl(HtmlTextWriter writer)
        {
            if (Enabled)
                ImageUrl = EnabledImageURL;
            else
                ImageUrl = DisabledImageURL;

 	        base.RenderControl(writer);
        }


        #region IScriptControl Members
        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor desc = new ScriptControlDescriptor("ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton", ClientID);
            desc.AddProperty("EnabledImageUrl", Page.ResolveClientUrl(EnabledImageURL));
            desc.AddProperty("DisabledImageUrl", Page.ResolveClientUrl(DisabledImageURL));
            
            
            if (EnabledHoverImageURL!=null)
                desc.AddProperty("HoverImageUrl", Page.ResolveClientUrl(EnabledHoverImageURL));

            return new ScriptDescriptor[] { desc };
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference reference = new ScriptReference();

            reference.Path = Page.ClientScript.GetWebResourceUrl(typeof(ToolbarButton), "ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.js");
            return new ScriptReference[] { reference };
        }

        #endregion IScriptControl Members

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptControl(this);
            }

        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptDescriptors(this);
            }
            base.Render(writer);
        }

       
    }
}
