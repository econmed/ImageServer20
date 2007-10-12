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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class TelephoneFormat
    {
        /// <summary>
        /// Formats the telephone number according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="hc"></param>
        /// <returns></returns>
        public static string Format(TelephoneDetail tn)
        {
            return Format(tn, FormatSettings.Default.TelephoneNumberDefaultFormat);
        }

        /// <summary>
        /// Formats the address according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %C - country code, preceded by +
        ///     %c - country code if different from default country code specified in <see cref="FormatSettings"/>
        ///     %A - area code
        ///     %N - phone number in form XXX-XXXX 
        ///     %X - extension, preceded by x
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(TelephoneDetail tn, string format)
        {
            string result = format;
            result = result.Replace("%C", tn.CountryCode == null ? "" : string.Format("+{0}", tn.CountryCode));

            result = result.Replace("%c",
                (tn.CountryCode == null || tn.CountryCode == FormatSettings.Default.TelephoneNumberSuppressCountryCode) ? ""
                : string.Format("+{0}", tn.CountryCode));

            result = result.Replace("%A", tn.AreaCode == null ? "" : tn.AreaCode);
            result = result.Replace("%N", tn.Number == null ? "" : string.Format("{0}-{1}", tn.Number.Substring(0, 3), tn.Number.Substring(3)));
            result = result.Replace("%X", string.IsNullOrEmpty(tn.Extension) ? "" : string.Format("x{0}", tn.Extension));

            return result.Trim();
        }

    }
}
