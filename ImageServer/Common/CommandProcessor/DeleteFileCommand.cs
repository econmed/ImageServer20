#region License

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

using System;
using System.IO;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    /// <summary>
    /// Command to delete a file.
    /// </summary>
    public class DeleteFileCommand : ServerCommand, IDisposable
    {
        #region Private Memebers
        private readonly string _originalFile;
        private string _backupFile;
        #endregion

        #region Constructors
        public DeleteFileCommand(String file)
            :base(String.Format("Delete {0}", file), true)
        {
            _originalFile = file;
        }
        #endregion

        #region Overriden Protected Methods
		protected override void OnExecute(ServerCommandProcessor theProcessor)
        {
            if (RequiresRollback)
            {
                Backup();
            }

            FileInfo fileInfo = new FileInfo(_originalFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        
        protected override void OnUndo()
        {
            if (!File.Exists(_originalFile))
            {
                File.Copy(_backupFile, _originalFile, true);
            }
        }
        #endregion

        #region Private Methods

        private void Backup()
        {
            if (File.Exists(_originalFile))
            {
                _backupFile = FileUtils.Backup(_originalFile);
            }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            if (RollBackRequested)
            {
                if (File.Exists(_originalFile))
                {
                    // we can now safely delete the backup file
                    if (File.Exists(_backupFile))
                        FileUtils.Delete(_backupFile);
                }
            }
            else
            {
                if (File.Exists(_backupFile))
					FileUtils.Delete(_backupFile);
            }
        }

        #endregion
    }
}
