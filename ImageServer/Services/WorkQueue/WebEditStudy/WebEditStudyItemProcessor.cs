using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    public class WebEditStudyItemProcessor : BaseItemProcessor
    {
        /// <summary>
        /// Represents an event generated by the <see cref="WebEditStudyItemProcessor"/> when the study is about to be edited.
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
        /// Represents an event generated by the <see cref="WebEditStudyItemProcessor"/> when the study has been edited.
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

        #region Private Fields
        private ServerFilesystemInfo _filesystem;
        private EventHandler<StudyEditingEventArgs> _edittingHandlers;
        private EventHandler<StudyEditedEventArgs> _editedHandlers;
        private IList<IWebEditStudyProcessorExtension> _plugins;
        private Study _study;
        private Patient _patient;
        #endregion

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

        #region Private Methods


        private void OnStudyUpdating(WebEditStudyContext context)
        {
            EventsHelper.Fire(_edittingHandlers, this, new StudyEditingEventArgs(context));
        }
        private void OnStudyUpdated(WebEditStudyContext context)
        {
            EventsHelper.Fire(_editedHandlers, this, new StudyEditedEventArgs(context));
        }

        private void LoadAdditionalEntities()
        {
            Debug.Assert(ServerPartition != null);
            Debug.Assert(StorageLocation != null);

            _filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);
            _study = Study.Find(StorageLocation.StudyInstanceUid, ServerPartition);
            _patient = Patient.Load(_study.PatientKey);

        }


        private bool FilesystemIsAccessable()
        {
            _filesystem =
                FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);

            return _filesystem != null && _filesystem.Readable && _filesystem.Writeable;
        }

        #endregion

        #region Overriden Protected Methods
        protected override void ProcessItem(Model.WorkQueue item)
        {
            if (!LoadStorageLocation(item))
            {
                WorkQueueSettings settings = WorkQueueSettings.Instance;

                DateTime newScheduledTime = Platform.Time.AddMilliseconds(settings.WorkQueueQueryDelay);
                DateTime expire = newScheduledTime.AddSeconds(settings.WorkQueueExpireDelaySeconds);
                Platform.Log(LogLevel.Info, "Storage is not ready. Postponing {0} work queue entry (GUID={1}) to {2}",item.WorkQueueTypeEnum, item.GetKey(), newScheduledTime);
                PostponeItem(item, newScheduledTime, expire);
            }
            else
            {
                
                if (!FilesystemIsAccessable())
                {
                    String reason = String.Format("Filesystem {0} is not readable and writable.", _filesystem.Filesystem.Description);
                    FailQueueItem(item, reason);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Study Edit started. GUID={0}. Storage={1}", item.GetKey(), item.StudyStorageKey);

                    LoadAdditionalEntities();
                    
                    LoadExtensions();

                    WebEditStudyCommandCompiler compiler = new WebEditStudyCommandCompiler();

                    StatisticsSet statistics = null;
                    using (ServerCommandProcessor processor = new ServerCommandProcessor("Web Edit Study"))
                    {
                        XmlElement actionXml = item.Data.DocumentElement;

                        List<BaseImageLevelUpdateCommand> updateCommands = compiler.Compile(actionXml);
                        UpdateStudyCommand updateStudyCommand = new UpdateStudyCommand(ServerPartition, StorageLocation, updateCommands);
                        processor.AddCommand(updateStudyCommand);
                        
                        WebEditStudyContext context = new WebEditStudyContext();
                        context.WorkQueueProcessor = this;
                        context.CommandProcessor = processor;
                        context.EditType = EditType.WebEdit;
                        context.OriginalStudyStorageLocation = StorageLocation;
                        context.EditCommands = updateCommands;
                        context.OriginalStudy = _study;
                        context.OrginalPatient = _patient;
                        
                        OnStudyUpdating(context);
                        
                        if (processor.Execute())
                        {
                            context.NewStudystorageLocation = context.OriginalStudyStorageLocation;//won't change

                            Complete();

                            OnStudyUpdated(context);
                            
                            statistics = updateStudyCommand.Statistics;
                        }
                        else
                        {
                            FailQueueItem(WorkQueueItem, processor.FailureReason);
                            Platform.Log(LogLevel.Info, "Study Edit failed. GUID={0}. Reason={1}", WorkQueueItem.GetKey(), processor.FailureReason);
                        }
                    }

                    if (statistics != null)
                    {
                        StatisticsLogger.Log(LogLevel.Info, statistics);
                    }
                }
            }
        }
        #endregion

        #region Protected Methods
        protected void LoadExtensions()
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
                    plugin.Initialize(this);
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
        protected void Complete()
        {
            PostProcessing(WorkQueueItem, true, true, true);
            Platform.Log(LogLevel.Info, "Study Edit completed. GUID={0}", WorkQueueItem.GetKey());
        }

        protected override bool CanStart()
        {
            Model.WorkQueue item = WorkQueueItem;

            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(item.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.In(
                new WorkQueueTypeEnum[]
                {
                    WorkQueueTypeEnum.StudyProcess,
                    WorkQueueTypeEnum.ReconcileStudy
                });
            workQueueCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending });

            List<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(item, workQueueCriteria);
            return (relatedItems == null || relatedItems.Count == 0);
        }
        #endregion

        #region Public Methods


        public override void Dispose()
        {
            
            if (_plugins != null)
            {
                foreach (IWebEditStudyProcessorExtension plugin in _plugins)
                {
                    plugin.Dispose();
                }
            }

            base.Dispose();
        }

        #endregion


    }
}
