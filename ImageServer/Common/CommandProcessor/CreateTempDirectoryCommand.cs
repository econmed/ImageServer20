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
using System.IO;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// Create a temporary directory.  Remove the directory and its contents when disposed.
	/// </summary>
	public class CreateTempDirectoryCommand : ServerCommand, IDisposable
	{
		#region Private Members
		private readonly string _directory;
		private bool _created = false;
		#endregion

		public string TempDirectory
		{
			get { return _directory; }
		}
		public CreateTempDirectoryCommand()
			: base("Create Temp Directory", true)
		{
			_directory = ServerPlatform.GetTempPath();
		}

		protected override void OnExecute()
		{
			if (Directory.Exists(_directory))
			{
				_created = false;
				return;
			}

			try
			{
			    Directory.CreateDirectory(_directory);
			}
            catch(UnauthorizedAccessException)
            {
                //alert the system admin
                ServerPlatform.Alert(AlertCategory.System, AlertLevel.Critical, "Filesystem", AlertTypeCodes.NoPermission,
                                     "Unauthorized access to {0} from {1}", _directory, ServiceTools.HostId);
                throw;
            }

			_created = true;
		}

		protected override void OnUndo()
		{
			if (_created)
			{
				DirectoryUtility.DeleteIfExists(_directory);
				_created = false;
			}
		}

		public void Dispose()
		{
			if (_created)
			{
				DirectoryUtility.DeleteIfExists(_directory);
				_created = false;
			}
		}
	}
}