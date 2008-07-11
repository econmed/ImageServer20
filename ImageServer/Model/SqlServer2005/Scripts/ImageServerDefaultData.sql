-- WorkQueueTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'StudyProcess','Process Study','Processing of a new incoming study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'AutoRoute','Auto Route','DICOM Auto-route request.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'DeleteStudy','Delete Study','Automatic deletion of a Study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'WebDeleteStudy','Web Delete Study','Manual study delete via the Web UI.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'WebMoveStudy','Web Move Study','Manual DICOM move of a study via the Web UI.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'WebEditStudy','Web Edit Study','Manual study edit via the Web UI.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),106,'CleanupStudy','Cleanup Study','Cleanup all unprocessed or failed instances within a study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),107,'CompressStudy','Compress Study','Compress a study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),108,'MigrateStudy','Study Tier Migration','Migrate studies between tiers.')
GO

-- WorkQueueStatusEnum inserts
INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'Idle','Idle','Waiting to expire or for more images')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'Pending','Pending','Pending')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),201,'In Progress','In Progress','In Progress')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),202,'Completed','Completed','The Queue entry is completed.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),203,'Failed','Failed','The Queue entry has failed.')
GO


-- FilesystemTierEnum
INSERT INTO [ImageServer].[dbo].[FilesystemTierEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'Tier1','Tier 1','Filesystem Tier 1')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemTierEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'Tier2','Tier 2','Filesystem Tier 2')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemTierEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'Tier3','Tier 3','Filesystem Tier 3')
GO


-- ServerRuleTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'AutoRoute','Auto Routing','A DICOM auto-routing rule')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'StudyDelete','Study Delete','A rule to specify when to delete a study')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'Tier1Retention','Tier1 Retention','A rule to specify how long a study will be retained on Tier1')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'OnlineRetention','Online Retention','A rule to specify how long a study will be retained online')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'StudyCompress','Study Compress','A rule to specify when a study should be compressed')
GO


--  WorkQueuePriorityEnum inserts
INSERT INTO [ImageServer].[dbo].WorkQueuePriorityEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'Low','Low','Low priority')
GO

INSERT INTO [ImageServer].[dbo].WorkQueuePriorityEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'Medium','Medium','Medium priority')
GO

INSERT INTO [ImageServer].[dbo].WorkQueuePriorityEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),300,'High','High','High priority')
GO


-- ServerRuleApplyTimeEnum inserts
INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'SopReceived','SOP Received','Apply rule when a SOP Instance has been received')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'SopProcessed','SOP Processed','Apply rule when a SOP Instance has been processed')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'SeriesProcessed','Series Processed','Apply rule when a Series is initially processed')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'StudyProcessed','Study Processed','Apply rule when a Study is initially processed')
GO


-- FilesystemQueueTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'DeleteStudy','Delete Study','Delete a Study')
GO
           
INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'PurgeStudy','Purge Study','Purge an Online Study')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'TierMigrate','Tier Migrate','Migrate a Study to a Lower Tier')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'LosslessCompress','Lossless Compress','Lossless Compress a Study')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'LossyCompress','Lossy Compress','Lossy Compress a Study')
GO


-- ServiceLockTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'FilesystemDelete','Filesystem Delete','Purge Data from a Filesystem')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'FilesystemReinventory','Filesystem Reinventory','Re-inventory Data within a Filesystem')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'FilesystemStudyProcess','Filesystem Reprocess Studies','Reapply Study Processing rules within a Filesystem')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'FilesystemLosslessCompress','Filesystem Lossless Compress','Lossless compress studies within a Filesystem')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'FilesystemLossyCompress','Filesystem Lossy Compress','Lossy compress studies within a Filesystem')
GO


-- ServerSopClass inserts
INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.1', '12-lead ECG Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.3', 'Ambulatory ECG Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.11', 'Basic Text SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.4.1', 'Basic Voice Audio Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.4', 'Blending Softcopy Presentation State Storage SOP Class', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.3.1', 'Cardiac Electrophysiology Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.65', 'Chest CAD SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.2', 'Color Softcopy Presentation State Storage SOP Class', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.33', 'Comprehensive SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1', 'Computed Radiography Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.2', 'CT Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.3', 'Deformable Spatial Registration Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.3', 'Digital Intra-oral X-Ray Image Storage � For Presentation', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.3.1', 'Digital Intra-oral X-Ray Image Storage � For Processing', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.2', 'Digital Mammography X-Ray Image Storage � For Presentation', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.2.1', 'Digital Mammography X-Ray Image Storage � For Processing', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.1', 'Digital X-Ray Image Storage � For Presentation', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.1.1', 'Digital X-Ray Image Storage � For Processing', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.104.1', 'Encapsulated PDF Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.2.1', 'Enhanced CT Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4.1', 'Enhanced MR Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.22', 'Enhanced SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.1.1', 'Enhanced XA Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.2.1', 'Enhanced XRF Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.2', 'General ECG Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.1', 'Grayscale Softcopy Presentation State Storage SOP Class', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.29', 'Hardcopy  Grayscale Image Storage SOP Class (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.30', 'Hardcopy Color Image Storage SOP Class (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.2.1', 'Hemodynamic Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.59', 'Key Object Selection Document', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.50', 'Mammography CAD SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4', 'MR Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4.2', 'MR Spectroscopy Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.2', 'Multi-frame Grayscale Byte Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.3', 'Multi-frame Grayscale Word Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.1', 'Multi-frame Single Bit Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.4', 'Multi-frame 1 Color Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.5', 'Nuclear Medicine Image  Storage (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.20', 'Nuclear Medicine Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.2', 'Ophthalmic Photography 16 Bit Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.1', 'Ophthalmic Photography 8 Bit Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.128', 'Positron Emission Tomography Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.40', 'Procedure Log Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.3', 'Pseudo-Color Softcopy Presentation State Storage SOP Class', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66', 'Raw Data Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.67', 'Real World Value Mapping Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.4', 'RT Beams Treatment Record Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.6', 'RT Brachy Treatment Record Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.2', 'RT Dose Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.1', 'RT Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.9', 'RT Ion Beams Treatment Record Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.8', 'RT Ion Plan Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.5', 'RT Plan Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.3', 'RT Structure Set Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.7', 'RT Treatment Summary Record Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7', 'Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.4', 'Segmentation Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.2', 'Spatial Fiducials Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.1', 'Spatial Registration Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9', 'Standalone Curve Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.10', 'Standalone Modality LUT Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.8', 'Standalone Overlay Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.129', 'Standalone PET Curve Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11', 'Standalone VOI LUT Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.3', 'Stereometric Relationship Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.27', 'Stored Print Storage SOP Class (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.6.1', 'Ultrasound Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.6', 'Ultrasound Image Storage (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.3.1', 'Ultrasound Multi-frame Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.3', 'Ultrasound Multi-frame Image Storage (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.1.1', 'Video Endoscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.2.1', 'Video Microscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.4.1', 'Video Photographic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.1', 'VL Endoscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.2', 'VL Microscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.4', 'VL Photographic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.3', 'VL Slide-Coordinates Microscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.3', 'X-Ray Angiographic Bi-Plane Image Storage (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.1', 'X-Ray Angiographic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.67', 'X-Ray Radiation Dose SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.2', 'X-Ray Radiofluoroscopic Image Storage', 0)
GO
     
INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.13.1.1', 'X-Ray 3D Angiographic Image Storage', 0);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.13.1.2', 'X-Ray 3D Craniofacial Image Storage', 0);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.4', 'Ophthalmic Tomography Image Storage', 0);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.104.2', 'Encapsulated CDA Storage', 1);
GO


-- ServerTransferSyntax inserts
INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.2', 'Explicit VR Big Endian', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.1', 'Explicit VR Little Endian', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2', 'Implicit VR Little Endian: Default Transfer Syntax for DICOM', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.91', 'JPEG 2000 Image Compression', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.90', 'JPEG 2000 Image Compression (Lossless Only)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.50', 'JPEG Baseline (Process 1)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.51', 'JPEG Extended (Process 2 & 4)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.70', 'JPEG Lossless, non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1])', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.5', 'RLE Lossless', 1)
GO


-- [StudyStatusEnum] inserts
INSERT INTO [ImageServer].[dbo].[StudyStatusEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),100,'Online','Online','Study is online')
GO

INSERT INTO [ImageServer].[dbo].[StudyStatusEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),101,'OnlineLossless','Online (Lossless)','Study is online and lossless compressed')
GO

INSERT INTO [ImageServer].[dbo].[StudyStatusEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),102,'OnlineLossy','Online (Lossy)','Study is online and lossy compressed')
GO

INSERT INTO [ImageServer].[dbo].[StudyStatusEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),200,'Pending','Pending','Pending')
GO


-- DuplicateSopPolicyEnum inserts
INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),100,'SendSuccess','Send Success','Send a DICOM C-STORE-RSP success status when receiving a duplicate, but ignore the file.')
GO

INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),101,'RejectDuplicates','Reject Duplicates','Send a DICOM C-STORE-RSP reject status when receiving a duplicate.')
GO

INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),102,'AcceptLatest','Accept Latest','Keep the latest object received.')
GO

INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),103,'CompareDuplicates','Compare Duplicates','Process duplicate objects received and compare them to originals flagging any differences as a failure.')
GO

-- ArchiveQueueStatusEnum inserts
INSERT INTO [ImageServer].[dbo].[ArchiveQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),100,'Pending','Pending','Pending')
GO

INSERT INTO [ImageServer].[dbo].[ArchiveQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),101,'In Progress','In Progress','In Progress')
GO

INSERT INTO [ImageServer].[dbo].[ArchiveQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),102,'Completed','Completed','The Queue entry is completed.')
GO

INSERT INTO [ImageServer].[dbo].[ArchiveQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),103,'Failed','Failed','The Queue entry has failed.')
GO


-- RestoreQueueStatusEnum inserts
INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),100,'Pending','Pending','Pending')
GO

INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),101,'In Progress','In Progress','In Progress')
GO

INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),102,'Completed','Completed','The Queue entry is completed.')
GO

INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),103,'Failed','Failed','The Queue entry has failed.')
GO


-- ArchiveTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),100,'HsmArchive','HSM Archive','Hierarchical storage management archive such as StorageTek QFS')
GO
