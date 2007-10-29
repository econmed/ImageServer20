#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.DicomServices;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Class that handles DICOM C-Store Requests for Non-Image objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This routine is a plugin implemeting the <see cref="IDicomScp{DicomScpParameters}"/> interface for handling 
    /// Non-Image DICOM C-STORE-RQ messages.
    /// </para>
    /// <para>
    /// The method queries the PartitionSopClass table in the database to determine the services
    /// it should support.  It also only implements the default DICOM transfer syntaxes.
    /// </para>
    /// </remarks>
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class NonImageStorageScpExtension : StorageScp
    {
        #region Private Members
        private IList<SupportedSop> _list;
        #endregion

        #region Contructors
        public NonImageStorageScpExtension()
        {

        }
        #endregion

        #region IDicomScp Members

        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            // Use the command processor for rollback capabilities.
            ServerCommandProcessor processor = new ServerCommandProcessor("Processing NonImage C-STORE-RQ");
            DicomStatus returnStatus;
            try
            {
                String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0,"");
                String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, "");

                StudyStorageLocation studyLocation = GetStudyStorageLocation(message);
                if (studyLocation == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to process image, no writeable storage location: {0}", sopInstanceUid);
                    throw new ApplicationException("No writeable storage location.");
                }

                String path = studyLocation.FilesystemPath;
                processor.ExecuteCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, studyLocation.PartitionFolder);
                processor.ExecuteCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, studyLocation.StudyFolder);
                processor.ExecuteCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, studyLocation.StudyInstanceUid);
                processor.ExecuteCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, seriesInstanceUid);
                processor.ExecuteCommand(new CreateDirectoryCommand(path));

                path = Path.Combine(path, sopInstanceUid + ".dcm");
                if (File.Exists(path))
                {
                    Platform.Log(LogLevel.Info, "Duplicate SOP Instance received, rejecting {0}", sopInstanceUid);
                    throw new ApplicationException("Duplicate SOP Instance received.");
                }

                DicomFile file = ConvertToDicomFile(message, path, association);
                processor.ExecuteCommand(new SaveDicomFileCommand(path, file));

                UpdateWorkQueue(message, studyLocation);

                Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2}", sopInstanceUid, association.CallingAE, association.CalledAE);
                
                returnStatus = DicomStatuses.Success;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", processor.Description);
                processor.Rollback();

                returnStatus =  DicomStatuses.ProcessingFailure;
            }

            // Send the response message
            server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid, returnStatus);

            return true;
        }

        /// <summary>
        /// Returns a list of the DICOM services supported by this plugin.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            if (_list == null)
            {
                // Load from the database the non-image sops that are current configured for this server partition.
                _list = new List<SupportedSop>();

                // Input parameters
                PartitionSopClassQueryParameters inputParms = new PartitionSopClassQueryParameters();
                inputParms.ServerPartitionKey = Partition.GetKey();

                // Do the query
                IReadContext read = _store.OpenReadContext();
                IQueryServerPartitionSopClasses broker = read.GetBroker<IQueryServerPartitionSopClasses>();
                IList<PartitionSopClass> sopClasses = broker.Execute(inputParms);
                read.Dispose();

                // Now process the SOP Class list
                foreach (PartitionSopClass partitionSopClass in sopClasses)
                {
                    if (partitionSopClass.Enabled
                        && partitionSopClass.NonImage)
                    {
                        SupportedSop sop = new SupportedSop();

                        sop.SopClass = SopClass.GetSopClass(partitionSopClass.SopClassUid);
                        sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
                        sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);

                        _list.Add(sop);
                    }
                }
            }

            return _list;
        }

        #endregion
    }
}