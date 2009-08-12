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
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Class for editing a study. 
	/// </summary>
	public class StudyEditor : IDisposable
	{
		/// <summary>
		/// Represents an event generated by the <see cref="StudyEditor"/> when the study is about to be edited.
		/// </summary>
		public class StudyEditingEventArgs : EventArgs
		{
			private readonly WebEditStudyContext _context;

			public StudyEditingEventArgs(WebEditStudyContext _context)
			{
				this._context = _context;
			}

			public WebEditStudyContext Context
			{
				get { return _context; }
			}
		}

		/// <summary>
		/// Represents an event generated by the <see cref="StudyEditor"/> when the study has been edited.
		/// </summary>
		public class StudyEditedEventArgs : EventArgs
		{
			private readonly WebEditStudyContext _context;

			public StudyEditedEventArgs(WebEditStudyContext _context)
			{
				this._context = _context;
			}

			public WebEditStudyContext Context
			{
				get { return _context; }
			}

		}


		#region Events
		public event EventHandler<StudyEditingEventArgs> StudyEditing
		{
			add { _edittingHandlers += value; }
			remove { _edittingHandlers -= value; }
		}
		public event EventHandler<StudyEditedEventArgs> StudyEdited
		{
			add { _editedHandlers += value; }
			remove { _editedHandlers -= value; }
		}
		#endregion

		#region Private Fields
		private EventHandler<StudyEditingEventArgs> _edittingHandlers;
		private EventHandler<StudyEditedEventArgs> _editedHandlers;
		private readonly Patient _patient;
		private readonly Study _study;
		private readonly ServerPartition _serverPartition;
		private readonly StudyStorageLocation _storageLocation;

		private IList<IWebEditStudyProcessorExtension> _plugins;
		public string _failureReason = string.Empty;
		#endregion

		#region Properties
		public Patient Patient
		{
			get { return _patient; }
		}
		public Study Study
		{
			get { return _study; }
		}
		public ServerPartition ServerPartition
		{
			get { return _serverPartition; }
		}
		public StudyStorageLocation StorageLocation
		{
			get { return _storageLocation; }
		}
		public string FailureReason
		{
			get { return _failureReason; }
			set { _failureReason = value; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="thePartition"></param>
		/// <param name="location"></param>
		/// <param name="thePatient"></param>
		/// <param name="theStudy"></param>
		public StudyEditor(ServerPartition thePartition, StudyStorageLocation location, Patient thePatient, Study theStudy)
		{
			Platform.CheckForNullReference(thePartition, "thePartition");
			Platform.CheckForNullReference(location, "location");
			Platform.CheckForNullReference(thePatient, "thePatient");
			Platform.CheckForNullReference(theStudy, "theStudy");

			_serverPartition = thePartition;
			_storageLocation = location;
			_patient = thePatient;
			_study = theStudy;
		}
		#endregion

		#region Private Methods
		private void OnStudyUpdating(WebEditStudyContext context)
		{
			EventsHelper.Fire(_edittingHandlers, this, new StudyEditingEventArgs(context));
		}

		private void OnStudyUpdated(WebEditStudyContext context)
		{
			EventsHelper.Fire(_editedHandlers, this, new StudyEditedEventArgs(context));
		}

		private void LoadExtensions()
		{
			Platform.Log(LogLevel.Debug, "Loading extensions..");
			WebEditStudyProcessorExtensionPoint ex = new WebEditStudyProcessorExtensionPoint();
			_plugins = CollectionUtils.Select<IWebEditStudyProcessorExtension>(
								ex.CreateExtensions(),
								delegate(IWebEditStudyProcessorExtension plugin)
								{
									return plugin.Enabled;
								});

			if (_plugins != null && _plugins.Count > 0)
			{

				Platform.Log(LogLevel.Debug, "{0} extension(s) found:", _plugins.Count);
				foreach (IWebEditStudyProcessorExtension plugin in _plugins)
				{
					plugin.Initialize();
				}

				StudyEditing += delegate(object sender, StudyEditingEventArgs ev)
								{
									foreach (IWebEditStudyProcessorExtension plugin in _plugins)
									{
										plugin.OnStudyEditing(ev.Context);
									}
								};

				StudyEdited += delegate(object sender, StudyEditedEventArgs ev)
								   {
									   foreach (IWebEditStudyProcessorExtension plugin in _plugins)
									   {
										   plugin.OnStudyEdited(ev.Context);
									   }
								   };

			}
		}
		#endregion

		/// <summary>
		/// Perform the edit.
		/// </summary>
		/// <param name="actionXml">A serialized XML representation of <see cref="SetTagCommand"/> objects</param>
		/// <returns></returns>
		public bool Edit(XmlElement actionXml)
		{

			Platform.Log(LogLevel.Info,
						 "Starting Edit of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
						 Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
						 Study.AccessionNumber, ServerPartition.Description);

			LoadExtensions();

            EditStudyWorkQueueDataParser parser = new EditStudyWorkQueueDataParser();
		    EditStudyWorkQueueData data = parser.Parse(actionXml);

		    using (ServerCommandProcessor processor = new ServerCommandProcessor("Web Edit Study"))
			{
				// Load the engine for editing rules.
				ServerRulesEngine engine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopEdited, ServerPartition.Key);
				engine.Load();

				// Convert UpdateItem in the request into BaseImageLevelUpdateCommand
                List<BaseImageLevelUpdateCommand> updateCommands = null;
                if (data!=null)
                {
                	updateCommands= CollectionUtils.Map<UpdateItem, BaseImageLevelUpdateCommand>(
					        data.EditRequest.UpdateEntries,
					        delegate(UpdateItem item)
					            {
					                // Note: For edit, we assume each UpdateItem is equivalent to SetTagCommand
					                return new SetTagCommand(item.DicomTag.TagValue, item.OriginalValue, item.Value);
					            }
					        );
                }               
                

				UpdateStudyCommand updateStudyCommand =
					new UpdateStudyCommand(ServerPartition, StorageLocation, updateCommands, engine);
				processor.AddCommand(updateStudyCommand);

				WebEditStudyContext context = new WebEditStudyContext();
				context.CommandProcessor = processor;
				context.EditType = EditType.WebEdit;
				context.OriginalStudyStorageLocation = StorageLocation;
				context.EditCommands = updateCommands;
				context.OriginalStudy = _study;
				context.OrginalPatient = _patient;
			    context.UserId = data.EditRequest.UserId;

				OnStudyUpdating(context);

				if (processor.Execute())
				{
					context.NewStudystorageLocation = context.OriginalStudyStorageLocation; //won't change

					OnStudyUpdated(context);

					if (updateStudyCommand.Statistics != null)
						StatisticsLogger.Log(LogLevel.Info, updateStudyCommand.Statistics);

					return true;
				}
				else
				{
					FailureReason = processor.FailureReason;

					return false;
				}
			}

		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{

			if (_plugins != null)
			{
				foreach (IWebEditStudyProcessorExtension plugin in _plugins)
				{
					plugin.Dispose();
				}
				_plugins = null;
			}
		}
	}
}
