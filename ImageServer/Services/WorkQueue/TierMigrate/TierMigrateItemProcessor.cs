using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    
    class TierMigrateItemProcessor : BaseItemProcessor
    {
        #region Private static members
        private static int _sessionStudiesMigrate = 0;
        private static TierMigrationAverageStatistics _averageStatisics = new TierMigrationAverageStatistics();
        private static readonly FilesystemMonitor _monitor = new FilesystemMonitor();
        #endregion

        #region Private Members
        private StatisticsSet _statistics = new StatisticsSet("TierMigration");
        #endregion

        static TierMigrateItemProcessor()
        {
            _monitor.Load();

        }

        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        /// <param name="failureDescription">The reason for the failure.</param>
        protected override void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            DBUpdateTime.Add(
                delegate
                {
                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                        WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
                        parms.ProcessorID = ServiceTools.ProcessorId;

                        parms.WorkQueueKey = item.GetKey();
                        parms.StudyStorageKey = item.StudyStorageKey;
                        parms.FailureCount = item.FailureCount + 1;
                        parms.FailureDescription = failureDescription;

                        Platform.Log(LogLevel.Error,
                                     "Failing {0} WorkQueue entry ({1}): {2}", 
                                     item.WorkQueueTypeEnum, 
                                     item.GetKey(), failureDescription);
                        parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                        parms.ScheduledTime = Platform.Time;
                        parms.ExpirationTime = Platform.Time.AddDays(1);

                        if (false == update.Execute(parms))
                        {
                            Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum,
                                         item.GetKey().ToString());
                        }
                        else
                            updateContext.Commit();
                    }
                }
                );


        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            
            try
            {
                _storageLocationList = LoadStorageLocation(item);
                DoMigrateStudies(_storageLocationList);
                PostProcessing(item, false, true);
            }
            catch(Exception e)
            {
                FailQueueItem(item, e.Message);
            }
        }

        private void DoMigrateStudies(IList<StudyStorageLocation> storages)
        {
            foreach(StudyStorageLocation storage in storages)
            {
                DoMigrateStudy(storage);
            }
            
            
        }


        private void DoMigrateStudy(StudyStorageLocation storage)
        {
            TierMigrationStatistics stat = new TierMigrationStatistics();
            stat.StudyInstanceUid = storage.StudyInstanceUid;
            stat.ProcessSpeed.Start();

            long size = (long) DirectoryUtility.CalculateFolderSize(storage.GetStudyPath());

            Platform.Log(LogLevel.Info, "Migrating study {0} from {1}", storage.StudyInstanceUid, storage.FilesystemTierEnum);
            ServerFilesystemInfo currFilesystem = _monitor.GetFilesystemInfo(storage.FilesystemKey);
            ServerFilesystemInfo newFilesystem = _monitor.GetLowerTierFilesystemForStorage(currFilesystem);

            if (newFilesystem == null)
            {
            	// this entry shouldn't have been scheduled in the first place.
                throw new ApplicationException("No writable filesystem found in lower tiers.");
            }

            ServerCommandProcessor _processor;
            TimeSpanStatistics dbUpdateTime;
            TimeSpanStatistics fileCopyTime;

            using (_processor = new ServerCommandProcessor("Migrate Study"))
            {
                TierMigrationContext context = new TierMigrationContext();
                context.OriginalStudyLocation = storage;
                context.Destination = newFilesystem;

                TierMigrateMoveStudyFolderCommand moveStudyFolderCommand = new TierMigrateMoveStudyFolderCommand(context);
                TierMigrateDatabaseUpdateCommand updateDBCommand = new TierMigrateDatabaseUpdateCommand(context);


                // TODO: Add some kind of progress indication.

                _processor.AddCommand(moveStudyFolderCommand);
                _processor.AddCommand(updateDBCommand);

                if (!_processor.Execute())
                {
                    throw new ApplicationException(_processor.FailureReason);
                }
                fileCopyTime = moveStudyFolderCommand.Statistics;
                dbUpdateTime = updateDBCommand.Statistics;
            }

            Platform.Log(LogLevel.Info, "Successfully migrated study {0} from {1} to {2}",
                            storage.StudyInstanceUid, storage.FilesystemTierEnum, newFilesystem.Filesystem.FilesystemTierEnum);

            stat.ProcessSpeed.SetData(size);
            stat.ProcessSpeed.End();
           
            _averageStatisics.AverageProcessSpeed.AddSample(stat.ProcessSpeed);
            _averageStatisics.AverageDBUpdateTime.AddSample(dbUpdateTime);
            _averageStatisics.AverageFileMoveTime.AddSample(fileCopyTime);
            _averageStatisics.AverageStudySize.AddSample(size);
            _statistics.AddSubStats(stat);

            _sessionStudiesMigrate++;

            if (_sessionStudiesMigrate%5==0)
            {
                StatisticsLogger.Log(LogLevel.Info, _averageStatisics);
                _averageStatisics = new TierMigrationAverageStatistics();
            }
            
                
        }

        
    }
}
