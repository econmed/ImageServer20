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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Defines the interface of a file system configuration controller.
    /// </summary>
    public interface IFileSystemsConfigurationController
    {
        bool AddFileSystem(Filesystem filesystem);
        bool UpdateFileSystem(Filesystem filesystem);
        IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria);
        IList<Filesystem> GetAllFileSystems();

        IList<FilesystemTierEnum> GetFileSystemTiers();
    }

    public class FileSystemsConfigurationController
    {
        #region Private members
        /// <summary>
        /// The adapter class to retrieve/set filesystems from Filesystem table
        /// </summary>
        private FileSystemDataAdapter _adapter = new FileSystemDataAdapter();
        
        #endregion

        #region public methods
       
        public bool AddFileSystem(Filesystem filesystem)
        {
            Platform.Log(LogLevel.Info, "Adding new filesystem : description = {0}, path={1}", filesystem.Description, filesystem.FilesystemPath);

            bool ok = _adapter.AddFileSystem(filesystem);

            Platform.Log(LogLevel.Info, "New filesystem added : description = {0}, path={1}", filesystem.Description, filesystem.FilesystemPath);

            return ok;
        }

        public bool UpdateFileSystem(Filesystem filesystem)
        {
            Platform.Log(LogLevel.Info, "Updating filesystem : description = {0}, path={1}", filesystem.Description, filesystem.FilesystemPath);

            bool ok = _adapter.Update(filesystem);

            if (ok)
                Platform.Log(LogLevel.Info, "Filesystem updated: description = {0}, path={1}", filesystem.Description, filesystem.FilesystemPath);
            else
                Platform.Log(LogLevel.Info, "Unable to update Filesystem: description = {0}, path={1}", filesystem.Description, filesystem.FilesystemPath);

            return ok;
        }
        public IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria)
        {
            return _adapter.GetFileSystems(criteria);
        }
        public IList<Filesystem> GetAllFileSystems()
        {
            return _adapter.GetAllFileSystems();
        }

        public IList<FilesystemTierEnum> GetFileSystemTiers()
        {
            return _adapter.GetFileSystemTiers();
        }
        #endregion public methods
    }
}
