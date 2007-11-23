#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ImageServerWebApplication.Common
{
    /// <summary>
    /// Generate a disk usage bar.
    /// </summary>
    /// <remarks>
    /// The usage and watermarks are specified in the Query string.
    /// To embed the generated image within another page, use <img src="BarChart.aspx?pct=xxxx&high=xxxx&low=xxxx"/>
    /// 
    /// </remarks>
    public partial class BarChart : System.Web.UI.Page
    {
        #region Private members
        private float _percentage;
        private float _high;
        private float _low;
        private int _width;
        private int _height;
        #endregion

        #region protected methods
        protected void Page_Load(object sender, EventArgs e)
        {
            // Read the input from the query string
            _percentage = float.Parse(Request.QueryString["pct"]);
            _high = float.Parse(Request.QueryString["high"]);
            _low = float.Parse(Request.QueryString["low"]);

            
            // set the ContentType appropriately, we are creating PNG image
            Response.ContentType = "image/png";

            // Load the background image
            System.Drawing.Image bmp = System.Drawing.Image.FromFile(Server.MapPath("~/images/usage.png"));
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            _width = bmp.Width;
            _height = bmp.Height;

            DrawUsageOverlay(ref graphics);

            // Save the image to memory stream (needed to do this if we are creating PNG image)
            MemoryStream MemStream = new MemoryStream();
            bmp.Save(MemStream, ImageFormat.Png);
            MemStream.WriteTo(Response.OutputStream);

            graphics.Dispose();
            bmp.Dispose();
        }

        /// <summary>
        /// Draw the disk usage bar.
        /// </summary>
        /// <param name="graphics"></param>
        protected void DrawUsageOverlay(ref Graphics graphics)
        {
            SolidBrush brush = new SolidBrush(Color.LightGray);

            int leftoffset = 1;

            if (_percentage < _low)
                brush.Color = Color.FromArgb(150, 0x27, 0xFF, 0x0F);
            else if (_percentage < _high)
                brush.Color = Color.FromArgb(150, 0xFF, 0xD8, 0x00);
            else
                brush.Color = Color.FromArgb(150, 0xFF, 0x3A, 0x00);

            // overlay the "usage" bar on top
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.FillRectangle(brush, new Rectangle(leftoffset, 6, (int)(_width * _percentage / 100f), 8));

            // add watermark icons
            graphics.CompositingMode = CompositingMode.SourceOver;
            System.Drawing.Image watermark = System.Drawing.Image.FromFile(Server.MapPath("~/images/Watermark.gif"));


            graphics.DrawImageUnscaled(watermark, (int)(_width * _high / 100f) - watermark.Width / 2 + leftoffset, 12);
            graphics.DrawImageUnscaled(watermark, (int)(_width * _low / 100f) - watermark.Width / 2 + leftoffset, 12);

            watermark.Dispose();

        }    
        #endregion protected methods

       
    }
}
