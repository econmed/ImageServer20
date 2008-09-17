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
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Server Command for editting a study
    /// </summary>
    public class EditStudyCommand : ServerCommand, IDisposable
    {
        #region Private Members
        private EditStudyContext _context = null;
        private readonly XmlElement _actionNode = null;
        private readonly ServerCommandProcessor _processor;
        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Create an instance of <see cref="EditStudyCommand"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="context"></param>
        /// <param name="actionNode">An <see cref="XmlElement"/> specifying the actions to be performed</param>
        public EditStudyCommand(string description, EditStudyContext context, XmlElement actionNode)
            : base(description, true)
        {
            _actionNode = actionNode;
            _context = context;
            _processor = new ServerCommandProcessor("EditCommandProcessor");
        }
        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Sets/Gets the context associated with the operation
        /// </summary>
        public EditStudyContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        #endregion Public Properties

        #region Private Methods

        #endregion Private Methods

        #region Overridden Protected Methods

        protected override void OnUndo()
        {
            if (_processor != null)
                _processor.Rollback();

        }


        /// <summary>
        /// Updates the study information 
        /// </summary>
        /// 
        protected override void OnExecute()
        {
            if (Context!=null)
            {

                StudyFolderUpdateCommand updatefiles = new StudyFolderUpdateCommand("Study Files Update Command", Context, _actionNode);
                DatabaseUpdateCommand dbUpdateCommand = new DatabaseUpdateCommand("Database Update Command", Context);
                dbUpdateCommand.Context = Context;

                _processor.AddCommand(updatefiles);
                _processor.AddCommand(dbUpdateCommand);

                if (!_processor.Execute())
                    throw new ApplicationException(_processor.FailureReason); // this will cause the caller to call Undo()
                
            }
        }


        #endregion

        #region Public Methods
        public void Dispose()
        {
            if (_processor != null)
                _processor.Dispose();

            if (Context != null)
                DirectoryUtility.DeleteIfExists(Context.TempOutRootFolder);
        }

        #endregion
    }
}
