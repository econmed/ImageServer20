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

using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using System;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// Class for saving a DicomFile instance in memory to disk.
	/// </summary>
	public class SaveDicomFileCommand : ServerCommand, IDisposable
	{
        private GetFilePathDelegateMethod GetFilePathDelegate;
        public delegate string GetFilePathDelegateMethod();

		#region Private Members
		private string _path;
        private string _backupPath;
		private readonly DicomFile _file;
		private readonly bool _failOnExists;
		private readonly bool _saveTemp;
		private bool _fileCreated = false;

	    #endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path">The path to save the file.</param>
		/// <param name="file">The file to save.</param>
		/// <param name="failOnExists">If the file already exists, the file will save.</param>
		/// <param name="saveTemp">Save the file to a temporary file first, then move to the final file.  This 
		/// reduces the likelyhood of having partial files if a crash in the service occurs when the file is being written.</param>
		public SaveDicomFileCommand(string path, DicomFile file, bool failOnExists, bool saveTemp)
			: base("Save DICOM Message", true)
		{
			Platform.CheckForNullReference(path, "File name");
			Platform.CheckForNullReference(file, "Dicom File object");

			_path = path;
			_file = file;
			_failOnExists = failOnExists;
			_saveTemp = saveTemp;
		}

        public SaveDicomFileCommand(GetFilePathDelegateMethod getFilePathDelegateMethod, DicomFile file, bool failOnExists, bool saveTemp)
			: base("Save DICOM Message", true)
		{
            Platform.CheckForNullReference(getFilePathDelegateMethod, "getFilePathDelegateMethod");
			Platform.CheckForNullReference(file, "Dicom File object");

            GetFilePathDelegate = getFilePathDelegateMethod;
			_file = file;
			_failOnExists = failOnExists;
			_saveTemp = saveTemp;
		}

        

	    private void Backup()
	    {
            if (String.IsNullOrEmpty(_path) && GetFilePathDelegate != null)
            {
                _path = GetFilePathDelegate();
            }

            if (File.Exists(_path))
            {
				if (_failOnExists)
					throw new ApplicationException(String.Format("DICOM File unexpectedly already exists: {0}",_path));
				try
				{
					_backupPath = String.Format("{0}.bak", _path);
					File.Copy(_path, _backupPath);
				}
				catch (IOException)
				{
					_backupPath = null;
					throw;
				}
            }
	    }

	    protected override void OnExecute()
		{
            if (RequiresRollback)
                Backup();

	    	string path;
			if (_saveTemp)
			{
				path = String.Format("{0}_tmp", _path);
				if (File.Exists(path))
					File.Delete(path);
			}
			else
				path = _path;

	    	using (FileStream stream = FileStreamOpener.OpenForSoleUpdate(path, FileMode.Create))
			{
				// Set _fileCreated here, because the file has been opened.
				if (!_saveTemp)
					_fileCreated = true;
				_file.Save(stream, DicomWriteOptions.Default);
				stream.Close();
			}

			if (_saveTemp)
			{
				if (File.Exists(_path))
					File.Delete(_path);
				File.Move(path, _path);
				_fileCreated = true;
			}
		}

		protected override void OnUndo()
		{
            if (File.Exists(_path) && _fileCreated) 
                File.Delete(_path); 
            
            if (false==String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
            {
                // restore original file
                File.Copy(_backupPath, _path, true);
            }            
		}

        #region IDisposable Members

        public void Dispose()
        {
            if (false == String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
            {
                File.Delete(_backupPath);
            }
        }

        #endregion
    }
}