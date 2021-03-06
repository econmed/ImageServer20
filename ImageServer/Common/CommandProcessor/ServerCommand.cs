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

using System;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// Abstract class representing a command within the Command pattern.
	/// </summary>
	/// <remarks>
	/// <para>The Command pattern is used throughout the ImageServer when doing
	/// file and database operations to allow undoing of the operations.  This
	/// abstract class is used as the interface for the command.</para>
	/// </remarks>
	public abstract class ServerCommand: IServerCommand
	{
		#region Private Members

		private string _description;
		private bool _requiresRollback;
		private readonly ServerCommandStatistics _stats;
        private ExecutionContext _executionContext;
	    private bool _rollBackRequested;

	    private EventHandler _executingEventHandlers;
        #endregion

		#region Public property

		/// <summary>
		/// Gets the <see cref="ServerCommandStatistics"/> of the command.
		/// </summary>
		public ServerCommandStatistics Statistics
		{
			get { return _stats; }
		}

		#endregion

		#region Constructor
		/// <summary>
		/// Constructor for a ServerCommand.
		/// </summary>
		/// <param name="description">A description of the command</param>
		/// <param name="requiresRollback">bool telling if the command requires a rollback of the operation if it fails</param>
		public ServerCommand(string description, bool requiresRollback)
		{
			_description = description;
			_requiresRollback = requiresRollback;
			_stats = new ServerCommandStatistics(this);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets and sets a value describing what the command is doing.
		/// </summary>
	    [XmlIgnore]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets a value describing if the ServerCommand requires a rollback of the operation its included in if it fails during execution.
		/// </summary>
        [XmlIgnore]
		public bool RequiresRollback
		{
			get { return _requiresRollback; }
			set { _requiresRollback = value; }
		}

        /// <summary>
        /// Gets or sets the execution context for the command.
        /// </summary>
        /// <remarks>
        /// The execution context is managed by the owner.
        /// </remarks>
        [XmlIgnore]
        public ExecutionContext ExecutionContext
        {
            get { return _executionContext; }
            set { _executionContext = value; }
        }

		/// <summary>
		/// Simple flag that tells if a rollback / Undo has been requested for the command.
		/// </summary>
		[XmlIgnore]
		protected bool RollBackRequested
	    {
	        get { return _rollBackRequested; }
	    }

	    #endregion

		#region Events
        /// <summary>
        /// Occurs when <see cref="Execute"/> begins.
        /// </summary>
        public event EventHandler Executing
        {
            add { _executingEventHandlers += value; }
            remove { _executingEventHandlers -= value; }
        }
		#endregion

		#region Public Methods
		/// <summary>
		/// Execute the ServerCommand.
		/// </summary>
		/// <param name="theProcessor">The <see cref="ServerCommandProcessor"/> executing the command.</param>
		public void Execute(ServerCommandProcessor theProcessor)
		{
			try
			{
				_stats.Start();
                EventsHelper.Fire(_executingEventHandlers, this, new EventArgs());
				OnExecute(theProcessor);    
			}
			finally
			{
				_stats.End();
			}
            
		}

        
		/// <summary>
		/// Undo the operation done by <see cref="Execute"/>.
		/// </summary>
		public void Undo()
		{
		    _rollBackRequested = true;
			OnUndo();
			_stats.End();
		}
		#endregion

		#region Virtual methods

		protected abstract void OnExecute(ServerCommandProcessor theProcessor);
		protected abstract void OnUndo();
		#endregion
        
    }

    /// <summary>
    /// Abstract class representing commands that tie to a specific context
    /// </summary>
    /// <remarks>
    /// <para> 
    /// </para>
    /// </remarks>
    public abstract class ServerCommand<TContext> : ServerCommand
        where TContext : class
    {
        #region Private Fields
        private readonly TContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ServerCommand"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="requiresRollback"></param>
        /// <param name="context"></param>
        public ServerCommand(string description, bool requiresRollback, TContext context)
            : base(description, requiresRollback)
        {
            _context = context;
        }
        #endregion

		#region Protected Properties
		/// <summary>
        /// Gets or sets the context of this command.
        /// </summary>
        protected TContext Context
        {
            get { return _context; }
		}
		#endregion
	}

    /// <summary>
    /// Abstract class representing commands that tie to a specific context
    /// and require specific type of input.
    /// </summary>
    /// <remarks>
    /// <para> 
    /// </para>
    /// </remarks>
    public abstract class ServerCommand<TContext, TParameters> : ServerCommand<TContext>
        where TContext : class
        where TParameters : class
    {
        #region Private Fields
        private readonly TParameters _parms;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ServerCommand"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="requiresRollback"></param>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        public ServerCommand(string description, bool requiresRollback, TContext context, TParameters parameters)
            :base(description, requiresRollback, context)
        {
            _parms = parameters;
        }
        #endregion

		#region Protected Properties
		/// <summary>
        /// Gets or sets the paramater object used for constructing this command.
        /// </summary>
        protected TParameters Parameters
        {
            get { return _parms; }
		}
		#endregion
	}
}