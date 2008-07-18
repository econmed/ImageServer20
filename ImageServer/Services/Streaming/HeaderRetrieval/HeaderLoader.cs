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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderRetrieval
{
    /// <summary>
    /// Loads the compressed study header stream.
    /// </summary>
    internal class HeaderLoader
    {
        private static readonly FilesystemMonitor _monitor = new FilesystemMonitor("Header Retrieval");
        private readonly HeaderRetrievalContext _context;
        private readonly HeaderLoaderStatistics _statistics = new HeaderLoaderStatistics();
        private Stream _compressedHeaderStream = null;
        private string _partitionAE;
        private string _studyInstanceUid;
        private StudyStorageLocation _studyLocation;

        #region Constructor

        static HeaderLoader()
        {
            _monitor.Load();
        }


        public HeaderLoader(HeaderRetrievalContext context)
        {
            _context = context;

            _studyInstanceUid = context.Parameters.StudyInstanceUID;
            _partitionAE = context.Parameters.ServerAETitle;
            StudyLocation = GetStudyStorageLocation(StudyInstanceUid, PartitionAE);
        }

        #endregion

        #region Protected Properties

        protected string PartitionAE
        {
            get { return _partitionAE; }
            set { _partitionAE = value; }
        }

        protected string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        #endregion Protected Properties

        #region Private methods

        public static StudyStorageLocation GetStudyStorageLocation(String studyInstanceUid, String partitionAE)
        {
            StudyStorageLocation location = null;
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                IServerPartitionEntityBroker partitionBroker = ctx.GetBroker<IServerPartitionEntityBroker>();
                ServerPartitionSelectCriteria partitionCriteria = new ServerPartitionSelectCriteria();
                partitionCriteria.AeTitle.EqualTo(partitionAE);
                IList<ServerPartition> partitions = partitionBroker.Find(partitionCriteria);

                if (partitions != null && partitions.Count > 0)
                {
                    ServerPartition partition = partitions[0];

                    IQueryStudyStorageLocation locQuery = ctx.GetBroker<IQueryStudyStorageLocation>();
                    StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters();
                    locParms.StudyInstanceUid = studyInstanceUid;
                    locParms.ServerPartitionKey = partition.GetKey();
                    IList<StudyStorageLocation> studyLocationList = locQuery.Execute(locParms);

                    if (studyLocationList != null && studyLocationList.Count > 0)
                    {
                        location = studyLocationList[0];
                    }
                }
            }

            return location;
        }

       
        #endregion

        #region Public Properties

        public HeaderLoaderStatistics Statistics
        {
            get { return _statistics; }
        }

        public Stream CompressedHeaderStream
        {
            get { return _compressedHeaderStream; }
        }

        public bool StudyExists
        {
            get { return StudyLocation != null; }
        }

        public StudyStorageLocation StudyLocation
        {
            get { return _studyLocation; }
            set { _studyLocation = value; }
        }

        #endregion

        #region Private Methods

        private void OpenCompressedHeader()
        {
            Platform.CheckForNullReference(StudyLocation, "StudyLocation");

            if (!IsFileSystemReadable(StudyLocation.FilesystemKey))
            {
                Platform.Log(LogLevel.Warn, "Study {0} on partition {1} resided on a non-readable filesystem",
                             StudyInstanceUid, PartitionAE);
            }

            String studyPath = StudyLocation.GetStudyPath();
            if (!Directory.Exists(studyPath))
            {
                // the study exist in the database but not on the filesystem.

                // TODO: If the study is migrated to another tier and the study folder is removed, 
                // we may want to do something here instead of throwing exception.
                throw new ApplicationException(String.Format("Study Folder {0} doesn't exist", studyPath));
            }

            String compressedHeaderFile = Path.Combine(studyPath, StudyInstanceUid + ".xml.gz");

            Platform.Log(LogLevel.Debug, "Study Header Path={0}", compressedHeaderFile);
            _compressedHeaderStream = FileStreamOpener.OpenForRead(compressedHeaderFile, FileMode.Open, 30000 /* try for 30 seconds */);

            //Thread.Sleep(100000);
        }

        #endregion

        #region Private Static Methods
        private static bool IsFileSystemReadable(ServerEntityKey fskey)
        {
            ServerFilesystemInfo fsInfo = _monitor.GetFilesystemInfo(fskey);
            return fsInfo.Readable;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the compressed header stream for the study with the specified study instance uid
        /// </summary>
        /// <returns>
        /// The compressed study header stream or null if the study doesn't exist.
        /// </returns>
        /// <remarks>
        /// </remarks>
        public Stream Load()
        {
            PartitionAE = _context.Parameters.ServerAETitle;
            StudyInstanceUid = _context.Parameters.StudyInstanceUID;

            if (StudyExists)
            {
                OpenCompressedHeader();
                return CompressedHeaderStream;
            }
            else
            {
                return null;
            }
        }

        #endregion Public Methods
    }
}