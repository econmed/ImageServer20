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

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// <see cref="ServerCommand"/> derived class for implementing commands that interact with the database.
	/// </summary>
	public abstract class ServerDatabaseCommand : ServerCommand
	{
		#region Private Members
		private IUpdateContext _updateContext;
		#endregion

		#region Properties
		public IUpdateContext UpdateContext
		{
			get { return _updateContext; }
			set { _updateContext = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor for a ServerDatabaseCommand.
		/// </summary>
		/// <param name="description">A description of the command</param>
		/// <param name="requiresRollback">bool telling if the command requires a rollback of the operation if it fails</param>
		public ServerDatabaseCommand(string description, bool requiresRollback)
			: base(description, requiresRollback)
		{
		}
		#endregion

		/// <summary>
		/// Execute the ServerDatabaseCommand with the specified <see cref="IUpdateContext"/>.
		/// </summary>
		protected abstract void OnExecute(IUpdateContext updateContext);

		/// <summary>
		/// Execute the <see cref="ServerCommand"/> 
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void OnExecute()
		{
			if (UpdateContext != null)
			{
				OnExecute(UpdateContext);
				UpdateContext = null;
			}
			else
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					OnExecute(updateContext);
					updateContext.Commit();
				}
			}
		}

		/// <summary>
		/// Undo of database command, note that this is not called because the transaction is rolled back instead.
		/// </summary>
		protected override void OnUndo()
		{

		}
	}
}