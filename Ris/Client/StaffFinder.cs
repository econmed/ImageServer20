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
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Provides utilities for staff name resolution.
    /// </summary>
    public class StaffFinder
    {
        /// <summary>
        /// Attempts to resolve the specified query to a single staff.  The query may consist of part of the surname,
        /// optionally followed by a comma and then part of the given name (e.g. "sm, b" for smith, bill).  The method
        /// returns true if the query resolves to a unique staff with the system, or false otherwise.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static bool ResolveName(string query, out StaffSummary staff)
        {
            if (!string.IsNullOrEmpty(query))
            {
                ListStaffRequest request = new ListStaffRequest();
                request.PageRequest = new PageRequestDetail(0, 2);  // need at most 2 rows to know if resolution was successful

                string[] names = query.Split(',');
                if (names.Length > 0)
                    request.LastName = names[0].Trim();
                if (names.Length > 1)
                    request.FirstName = names[1].Trim();

                ListStaffResponse response = null;
                Platform.GetService<IStaffAdminService>(
                    delegate(IStaffAdminService service)
                    {
                        response = service.ListStaff(request);
                    });
                if (response.Staffs.Count == 1)
                {
                    staff = response.Staffs[0];
                    return true;
                }
            }

            // can't resolve
            staff = null;
            return false;
        }
          
        /// <summary>
        /// Attempts to resolve the specified query to a single staff.  The query may consist of part of the surname,
        /// optionally followed by a comma and then part of the given name (e.g. "sm, b" for smith, bill).  If the 
        /// query matches more than a single name, a dialog is shown allowing the user to select a staff person.
        /// The method returns true if the name is successfully resolved, or false otherwise.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="desktopWindow"></param>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static bool ResolveNameInteractive(string query, IDesktopWindow desktopWindow, out StaffSummary staff)
        {
            bool resolved = ResolveName(query, out staff);
            if (!resolved)
            {
                StaffSummaryComponent staffComponent = new StaffSummaryComponent(true);

                if (!string.IsNullOrEmpty(query))
                {
                    string[] names = query.Split(',');
                    if (names.Length > 0)
                        staffComponent.LastName = names[0].Trim();
                    if (names.Length > 1)
                        staffComponent.FirstName = names[1].Trim();
                }

                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    desktopWindow, staffComponent, SR.TitleStaff);

                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    staff = (StaffSummary)staffComponent.SelectedStaff.Item;
                    if (staff != null)
                    {
                        resolved = true;
                    }
                }
            }
            return resolved;
        }

    }
}
