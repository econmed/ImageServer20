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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyStorageLocation : ServerEntity, ICloneable, IEquatable<StudyStorageLocation>
    {
        private const string STUDY_XML_EXTENSION = "xml";
        private const string STUDY_XML_GZIP_EXTENSION = "xml.gz";
        
        #region Constructors
        public StudyStorageLocation()
            : base("StudyStorageLocation")
        {
        }

        /// <summary>
        /// Create a copy of the specified <see cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="original"></param>
        public StudyStorageLocation(StudyStorageLocation original)
            : base("StudyStorageLocation")
        {
            this.SetKey(original.Key);
            this.Enabled = original.Enabled;
            this.FilesystemKey = original.FilesystemKey;
            this.FilesystemPath = original.FilesystemPath;
            this.FilesystemStudyStorageKey = original.FilesystemStudyStorageKey;
            this.FilesystemTierEnum = original.FilesystemTierEnum;
            this.InsertTime = original.InsertTime;
            this.IsReconcileRequired = original.IsReconcileRequired;
            this.LastAccessedTime = original.LastAccessedTime;
            this.Lock = original.Lock;
            this.PartitionFolder = original.PartitionFolder;
            this.QueueStudyStateEnum = original.QueueStudyStateEnum;
            this.ReadOnly = original.ReadOnly;
            this.ServerPartitionKey = original.ServerPartitionKey;
            this.ServerTransferSyntaxKey = original.ServerTransferSyntaxKey;
            this.StudyFolder = original.StudyFolder;
            this.StudyInstanceUid = original.StudyInstanceUid;
            this.StudyStatusEnum = original.StudyStatusEnum;
            this.StudyUidFolder = original.StudyUidFolder;
            this.TransferSyntaxUid = original.TransferSyntaxUid;
            this.WriteOnly = original.WriteOnly;
        }
        #endregion

        #region Private Members
        static private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private ServerEntityKey _serverPartitionKey;
        private ServerEntityKey _filesystemKey;
    	private ServerEntityKey _serverTransferSyntaxKey;
        private string _studyInstanceUid;
        private DateTime _lastAccessed;
        private DateTime _insertTime;
        private StudyStatusEnum _statusEnum;
        private String _filesystemPath;
        private String _serverPartitionFolder;
        private String _storageFilesystemFolder;
        private bool _lock; 
        private bool _enabled;
        private bool _readOnly;
        private bool _writeOnly;
        private FilesystemTierEnum _filesystemTierEnum;
    	private String _transferSyntaxUid;
    	private ServerEntityKey _filesystemStudyStorageKey;
    	private QueueStudyStateEnum _queueStudyState;
        private bool _reconcileRequired;
        private IList<StudyIntegrityQueue> _integrityQueueItems;

        private ServerPartition _partition;
        private Study _study;
        private StudyXml _studyXml;
        private string _studyUidFolder;
        private string _studyFolderRelativePath;
        private StudyStorage _studyStorage;
        private Patient _patient;
        private IList<ArchiveStudyStorage> _archives;

        #endregion

        #region Public Properties
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "ServerPartitionGUID")]
		public ServerEntityKey ServerPartitionKey
		{
			get { return _serverPartitionKey; }
			set { _serverPartitionKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "FilesystemGUID")]
		public ServerEntityKey FilesystemKey
		{
			get { return _filesystemKey; }
			set { _filesystemKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "ServerTransferSyntaxGUID")]
		public ServerEntityKey ServerTransferSyntaxKey
		{
			get { return _serverTransferSyntaxKey; }
			set { _serverTransferSyntaxKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "StudyInstanceUid")]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "LastAccessedTime")]
		public DateTime LastAccessedTime
		{
			get { return _lastAccessed; }
			set { _lastAccessed = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "InsertTime")]
		public DateTime InsertTime
		{
			get { return _insertTime; }
			set { _insertTime = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "Lock")]
		public bool Lock
		{
			get { return _lock; }
			set { _lock = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "StudyStatusEnum")]
		public StudyStatusEnum StudyStatusEnum
		{
			get { return _statusEnum; }
			set { _statusEnum = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "FilesystemPath")]
		public string FilesystemPath
		{
			get { return _filesystemPath; }
			set { _filesystemPath = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "PartitionFolder")]
		public string PartitionFolder
		{
			get { return _serverPartitionFolder; }
			set { _serverPartitionFolder = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "StudyFolder")]
		public string StudyFolder
		{
			get { return _storageFilesystemFolder; }
			set { _storageFilesystemFolder = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "Enabled")]
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "ReadOnly")]
		public bool ReadOnly
		{
			get { return _readOnly; }
			set { _readOnly = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "WriteOnly")]
		public bool WriteOnly
		{
			get { return _writeOnly; }
			set { _writeOnly = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "FilesystemTierEnum")]
		public FilesystemTierEnum FilesystemTierEnum
		{
			get { return _filesystemTierEnum; }
			set { _filesystemTierEnum = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "TransferSyntaxUid")]
		public string TransferSyntaxUid
		{
			get { return _transferSyntaxUid; }
			set { _transferSyntaxUid = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "FilesystemStudyStorageGUID")]
		public ServerEntityKey FilesystemStudyStorageKey
		{
			get { return _filesystemStudyStorageKey; }
			set { _filesystemStudyStorageKey = value; }
		}

		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "QueueStudyStateEnum")]
		public QueueStudyStateEnum QueueStudyStateEnum
		{
			get { return _queueStudyState; }
			set { _queueStudyState = value; }
		}

		[EntityFieldDatabaseMappingAttribute(TableName = "StudyStorageLocation", ColumnName = "IsReconcileRequired")]
		public bool IsReconcileRequired
		{
			get { return _reconcileRequired; }
			set { _reconcileRequired = value; }
		}
    	#endregion

        #region Public Properties

        public string StudyUidFolder
        {
            get
            {
                if (String.IsNullOrEmpty(_studyUidFolder))
                    return StudyInstanceUid;
                else
                    return _studyUidFolder;
            }
            set { _studyUidFolder = value; }
        }

        /// <summary>
        /// Gets the <see cref="ServerPartition"/> of the study.
        /// </summary>
        public ServerPartition ServerPartition
        {
            get
            {
                lock (SyncRoot)
                {
                    if (_partition == null)
                    {
                        _partition = ServerPartition.Load(ServerPartitionKey);
                    }
                }
                return _partition;
            }
        }

        /// <summary>
        /// Gets the related <see cref="ArchiveStudyStorage"/>.
        /// </summary>
        public IList<ArchiveStudyStorage> ArchiveLocations
        {
            get
            {
                if (_archives == null)
                {
                    lock (SyncRoot)
                    {
                        _archives = GetArchiveLocations(Key);
                    }
                }
                return _archives;
            }
        }

        /// <summary>
        /// Gets the related <see cref="Study"/> entity.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public Study Study
        {
            get
            {
                if (_study==null)
                {
                    using(IPersistenceContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
                        _study = LoadStudy(context);
                    }
                }
                return _study;
            }
        }

        #endregion

        #region Public Methods
        
        public Study LoadStudy(IPersistenceContext context)
        {
            lock (SyncRoot)
            {
                _study = Study.Find(context, StudyInstanceUid, ServerPartition);
            }
            return _study;
            
        }

        

        public string GetStudyPath()
        {
            string path = Path.Combine(FilesystemPath, StudyFolderRelativePath);
            return path;
        }

        public string StudyFolderRelativePath
        {
            get { 
                if (_studyFolderRelativePath!=null)
                    return _studyFolderRelativePath; 
                else
                {
                    string path = Path.Combine(PartitionFolder, StudyFolder);
                    path = Path.Combine(path, StudyUidFolder);
                    return path;
                }
            }
            set { _studyFolderRelativePath = value; }
        }

        /// <summary>
        /// Gets the related <seealso cref="StudyStorage"/> entity.
        /// </summary>
        public StudyStorage StudyStorage
        {
            get
            {
                if (_studyStorage==null)
                {
                    lock(SyncRoot)
                    {
                        // TODO: Use ExecutionContext to re-use db connection if possible
                        // This however requires breaking the Common --> Model dependency.
                        using(IReadContext readContext= PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                        {
                            _studyStorage = StudyStorage.Load(readContext, Key);
                        }
                    }
                }
                return _studyStorage;
            }
        }

        public Patient Patient
        {
            get {

                if (_patient==null)
                {
                    lock (SyncRoot)
                    {
                        // TODO: Use ExecutionContext to re-use db connection if possible
                        // This however requires breaking the Common --> Model dependency.
                        using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                        {
                            _patient = Patient.Load(readContext, StudyStorage.Study.PatientKey);
                        }
                    }
                }
                return _patient;
            }
        }

        public bool IsNearline
        {
            get { return StudyStatusEnum.Equals(Model.StudyStatusEnum.Nearline); }
        }

        /// <summary>
        /// Returns a boolean indicating whether the study has been archived and the latest 
        /// copy in the archive is lossless.
        /// </summary>
        public bool IsLatestArchiveLossless
        {
            get
            {
                if (ArchiveLocations == null || ArchiveLocations.Count == 0)
                    return false;
                else
                    return ArchiveLocations[0].ServerTransferSyntax.Lossless;
            }
        }


        /// <summary>
        /// Acquires a lock on the study for processing
        /// </summary>
        /// <returns>
        /// <b>true</b> if the study is successfully locked.
        /// <b>false</b> if the study cannot be locked or is being locked by another process.
        /// </returns>
        /// <remarks>
        /// This method is non-blocking. Caller must check the return value to ensure the study has been
        /// successfully locked.
        /// </remarks>
        public bool AcquireLock()
        {
            IUpdateContext context =
                PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
            using (context)
            {
                ILockStudy lockStudyBroker = context.GetBroker<ILockStudy>();
                LockStudyParameters parms = new LockStudyParameters();
                parms.StudyStorageKey = GetKey();
                parms.Lock = true;
                if (!lockStudyBroker.Execute(parms))
                    return false;


                context.Commit();
                return parms.Successful;
            }
        }

        /// <summary>
        /// Releases a lock acquired via <see cref="AcquireLock"/>
        /// </summary>
        /// <returns>
        /// <b>true</b> if the study is successfully unlocked.
        /// </returns>
        /// <remarks>
        /// This method is non-blocking. Caller must check the return value to ensure the study has been
        /// successfully unlocked.
        /// </remarks>
        public bool ReleaseLock()
        {
            IUpdateContext context =
                PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
            using (context)
            {
                ILockStudy lockStudyBroker = context.GetBroker<ILockStudy>();
                LockStudyParameters parms = new LockStudyParameters();
                parms.StudyStorageKey = GetKey();
                parms.Lock = false;
                if (!lockStudyBroker.Execute(parms))
                    return false;


                context.Commit();
                return parms.Successful;
            }
        }


        /// <summary>
        /// Return snapshot of all related items in the Study Integrity Queue.
        /// </summary>
        /// <returns></returns>
        public IList<StudyIntegrityQueue> GetRelatedStudyIntegrityQueueItems()
        {
            lock (SyncRoot) // make this thread-safe
            {
                if (_integrityQueueItems == null)
                {
                    using (IReadContext ctx = _store.OpenReadContext())
                    {
                        IStudyIntegrityQueueEntityBroker integrityQueueBroker =
                            ctx.GetBroker<IStudyIntegrityQueueEntityBroker>();
                        StudyIntegrityQueueSelectCriteria parms = new StudyIntegrityQueueSelectCriteria();

                        parms.StudyStorageKey.EqualTo(GetKey());

                        _integrityQueueItems = integrityQueueBroker.Find(parms);
                    }
                }
            }
            return _integrityQueueItems;
        }

        /// <summary>
        /// Returns a value indicating whether this study can be updated.
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        /// <remarks>A study can be updated does not necessarily mean a work queue entry
        /// can be inserted. Other conditions may require.</remarks>
        public bool CanUpdate(out string reason)
        {
            reason = null;
            if (StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy) && IsLatestArchiveLossless)
                reason = String.Format("Study {0} cannot be updated because it has been archived as lossless and is currently lossy compressed.", StudyInstanceUid);

            return string.IsNullOrEmpty(reason);
        }
        #endregion

        /// <summary>
        /// Find all <see cref="StudyStorageLocation"/> associcated with the specified <see cref="StudyStorage"/>
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        static public IList<StudyStorageLocation> FindStorageLocations(StudyStorage storage)
        {
            using(IReadContext readContext = _store.OpenReadContext())
            {
                return FindStorageLocations(readContext, storage, null);
            }
        }

        static public IList<StudyStorageLocation> FindStorageLocations(
                    IPersistenceContext context, StudyStorage storage)
        {
            return FindStorageLocations(context, storage, null);
        }

        static public IList<StudyStorageLocation> FindStorageLocations(
                   IPersistenceContext context, StudyStorage storage,
                   Predicate<StudyStorageLocation> filter)
        {
            IQueryStudyStorageLocation locQuery = context.GetBroker<IQueryStudyStorageLocation>();
            StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters();
            locParms.StudyInstanceUid = storage.StudyInstanceUid;
            locParms.ServerPartitionKey = storage.ServerPartitionKey;
            IList<StudyStorageLocation> list = locQuery.Find(locParms);
            if (filter != null)
                CollectionUtils.Remove(list, filter);
            return list;
        }

        /// <summary>
		/// Returns the path of the folder for the specified series.
		/// </summary>
		/// <param name="seriesInstanceUid"></param>
        public String GetSeriesPath(string seriesInstanceUid)
        {
            return Path.Combine(GetStudyPath(), seriesInstanceUid);
        }

        /// <summary>
		/// Returns the path of the sop instance with the specified series and sop instance uid.
		/// </summary>
		/// <param name="seriesInstanceUid"></param>
        /// <param name="sopInstanceUid"></param>
        public String GetSopInstancePath(string seriesInstanceUid, string sopInstanceUid)
        {
            String path = Path.Combine(GetSeriesPath(seriesInstanceUid), sopInstanceUid + ".dcm");
            return path ;
        }


        /// <summary>
		/// Query for the all archival records for a study, sorted by archive time descendingly (latest first).
		/// </summary>
		/// <param name="studyStorageKey">The primary key of the StudyStorgae table.</param>
		/// <returns>null if not found, else the value.</returns>
        static public IList<ArchiveStudyStorage> GetArchiveLocations(ServerEntityKey studyStorageKey)
        {
            using (IReadContext readContext = _store.OpenReadContext())
            {
                ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
                archiveStudyStorageCriteria.StudyStorageKey.EqualTo(studyStorageKey);
                archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

                IArchiveStudyStorageEntityBroker broker = readContext.GetBroker<IArchiveStudyStorageEntityBroker>();

                return broker.Find(archiveStudyStorageCriteria);
            }
        }


        /// <summary>
        /// Gets the path of the study xml.
        /// </summary>
        /// <returns></returns>
        public String GetStudyXmlPath()
        {
            String path = Path.Combine(GetStudyPath(), StudyInstanceUid);
            path += "." + STUDY_XML_EXTENSION;

            return path;
        }

        /// <summary>
        /// Gets the path of the compressed study xml.
        /// </summary>
        /// <returns></returns>
        public String GetCompressedStudyXmlPath()
        {
            String path = Path.Combine(GetStudyPath(), StudyInstanceUid);
            path += "." + STUDY_XML_GZIP_EXTENSION;

            return path;
        }

        /// <summary>
        /// Gets the <see cref="StudyXml"/> if it exists in this storage location.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public StudyXml LoadStudyXml()
        {
            // TODO: Use FileStreamOpener instead
            // Can't do it until we break the dependency of ImageServer.Common on Model
            if (_studyXml==null)
            {
                lock(SyncRoot)
                {
                    try
                    {
                        Stream xmlStream = Open(GetStudyXmlPath());
                        if (xmlStream != null)
                        {
                            XmlDocument xml = new XmlDocument();
                            using (xmlStream)
                            {
                                StudyXmlIo.Read(xml, xmlStream);
                                xmlStream.Close();
                            }

                            _studyXml = new StudyXml();
                            _studyXml.SetMemento(xml);

                        }
                    }
                    catch (Exception)
                    { }
                }
                
            }
            

            return _studyXml;
        }

        public StudyXml LoadStudyXml(bool refresh)
        {
            lock(SyncRoot)
            {
                Stream xmlStream = Open(GetStudyXmlPath());
                if (xmlStream != null)
                {
                    XmlDocument xml = new XmlDocument();
                    using (xmlStream)
                    {
                        StudyXmlIo.Read(xml, xmlStream);
                        xmlStream.Close();
                    }

                    _studyXml = new StudyXml();
                    _studyXml.SetMemento(xml);

                }

            }
            
            return _studyXml;
        }
        
        // TODO: Replace this method with FileStreamOpener
        private static Stream Open(string path)
        {
            FileStream stream = null;

            for (int i = 0; i<50; i++)
            {
                Exception lastException=null;
                try
                {
                    stream = new FileStream(path, FileMode.Open, 
                                            FileAccess.Read, 
                                            FileShare.None /* deny sharing */);
                    break;
                }
                catch (FileNotFoundException e)
                {
                    // Maybe it is being swapped?
                    lastException = e;
                }
                catch (DirectoryNotFoundException)
                {
                    // The path is invalid
                    throw;
                }
                catch (PathTooLongException)
                {
                    // The path is too long
                    throw;
                }
                catch (IOException e)
                {
                    // other IO exceptions should be treated as retry
                    lastException = e;
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(50, 100));
                }
            }
            return stream;
        }

		public void LogFilesystemQueue( )
		{
			FilesystemQueueSelectCriteria criteria = new FilesystemQueueSelectCriteria();
			criteria.StudyStorageKey.EqualTo(Key);

			using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				IFilesystemQueueEntityBroker broker = read.GetBroker<IFilesystemQueueEntityBroker>();

				IList<FilesystemQueue> list = broker.Find(criteria);
				foreach (FilesystemQueue queueItem in list)
				{
					if (queueItem.FilesystemQueueTypeEnum.Equals(FilesystemQueueTypeEnum.LosslessCompress) ||
						queueItem.FilesystemQueueTypeEnum.Equals(FilesystemQueueTypeEnum.LossyCompress))
					{
						XmlElement element = queueItem.QueueXml.DocumentElement;

						string syntax = element.Attributes["syntax"].Value;

						TransferSyntax compressSyntax = TransferSyntax.GetTransferSyntax(syntax);
						if (compressSyntax != null)
						{
							Platform.Log(LogLevel.Info, "{0}: Study {1} on partition {2} scheduled for {3} compression at {4}",
										 queueItem.FilesystemQueueTypeEnum.Description, StudyInstanceUid, ServerPartition.AeTitle,
										 compressSyntax.Name, queueItem.ScheduledTime);
						}
						else
							Platform.Log(LogLevel.Info, "{0}: Study {1} on partition {2} scheduled for {3} compression at {4}",
										 queueItem.FilesystemQueueTypeEnum.Description, StudyInstanceUid, ServerPartition.AeTitle,
										 syntax, queueItem.ScheduledTime);
					}
					else
					{
						Platform.Log(LogLevel.Info, "{0}: Study {1} on partition {2} scheduled for {3}",
									 queueItem.FilesystemQueueTypeEnum.Description, StudyInstanceUid, ServerPartition.AeTitle,
									 queueItem.ScheduledTime);
					}
				}
			}
		}

        #region ICloneable Members

        public object Clone()
        {
            return new StudyStorageLocation(this);
        }

        #endregion



        #region IEquatable<StudyStorageLocation> Members

        public bool Equals(StudyStorageLocation other)
        {
            // TODO: We may need more sophisticated method to determine if 2 locations are the same, especially if mapped drives are used.
            return this.GetStudyPath().Equals(other.GetStudyPath(), StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

    }
}
