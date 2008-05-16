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

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// An implementation of <see cref="ISpecification"/> that tests if the current thread principal is in a given role.
    /// </summary>
    public class PrincipalPermissionSpecification : ISpecification
    {
        private readonly string _role;

        /// <summary>
        /// Constructs an instance of this class for the specified role.
        /// </summary>
        public PrincipalPermissionSpecification(string role)
        {
            _role = role;
        }

        #region ISpecification Members

        /// <summary>
        /// Tests the <see cref="Thread.CurrentPrincipal"/> for the permission represented by this object.
        /// </summary>
		/// <remarks>
		/// If the application is running in non-authenticated (stand-alone) mode, the test will always
		/// succeed.  If the application is running in authenticated (enterprise) mode, the test succeeds only
		/// if the thread current principal is in the role assigned to this instance.
		/// </remarks>
		/// <param name="obj">This parameter is ignored.</param>
        public TestResult Test(object obj)
        {
			// if the thread is running in a non-authenticated mode, then we have no choice but to allow.
			// this seems a little counter-intuitive, but basically we're counting on the fact that if
			// the desktop is running in an enterprise environment, then the thread *will* be authenticated,
			// and that this is enforced by some mechanism outside the scope of this class.  The only
			// scenario in which the thread would ever be unauthenticated is the stand-alone scenario.
			if(Thread.CurrentPrincipal == null || Thread.CurrentPrincipal.Identity.IsAuthenticated == false)
				return new TestResult(true);

			// if running in authenticated mode, test the role
            return new TestResult(Thread.CurrentPrincipal.IsInRole(_role));
        }

        #endregion
    }
}
