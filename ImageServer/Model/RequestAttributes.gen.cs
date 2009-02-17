#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using ClearCanvas.Dicom;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class RequestAttributes: ServerEntity
    {
        #region Constructors
        public RequestAttributes():base("RequestAttributes")
        {}
        public RequestAttributes(
             System.String _requestedProcedureId_
            ,System.String _scheduledProcedureStepId_
            ,ClearCanvas.ImageServer.Enterprise.ServerEntityKey _seriesKey_
            ):base("RequestAttributes")
        {
            _requestedProcedureId = _requestedProcedureId_;
            _scheduledProcedureStepId = _scheduledProcedureStepId_;
            _seriesKey = _seriesKey_;
        }
        #endregion

        #region Private Members
        private System.String _requestedProcedureId;
        private System.String _scheduledProcedureStepId;
        private ClearCanvas.ImageServer.Enterprise.ServerEntityKey _seriesKey;
        #endregion

        #region Public Properties
        [DicomField(DicomTags.RequestedProcedureId, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="RequestAttributes", ColumnName="RequestedProcedureId")]
        public System.String RequestedProcedureId
        {
        get { return _requestedProcedureId; }
        set { _requestedProcedureId = value; }
        }
        [DicomField(DicomTags.ScheduledProcedureStepId, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="RequestAttributes", ColumnName="ScheduledProcedureStepId")]
        public System.String ScheduledProcedureStepId
        {
        get { return _scheduledProcedureStepId; }
        set { _scheduledProcedureStepId = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="RequestAttributes", ColumnName="SeriesGUID")]
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey SeriesKey
        {
        get { return _seriesKey; }
        set { _seriesKey = value; }
        }
        #endregion

        #region Static Methods
        static public RequestAttributes Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public RequestAttributes Load(IReadContext read, ServerEntityKey key)
        {
            IRequestAttributesEntityBroker broker = read.GetBroker<IRequestAttributesEntityBroker>();
            RequestAttributes theObject = broker.Load(key);
            return theObject;
        }
        static public RequestAttributes Insert(RequestAttributes table)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                return Insert(update, table);
            }
        }
        static public RequestAttributes Insert(IUpdateContext update, RequestAttributes table)
        {
            IRequestAttributesEntityBroker broker = update.GetBroker<IRequestAttributesEntityBroker>();
            RequestAttributesUpdateColumns updateColumns = new RequestAttributesUpdateColumns();
            updateColumns.RequestedProcedureId = table.RequestedProcedureId;
            updateColumns.ScheduledProcedureStepId = table.ScheduledProcedureStepId;
            updateColumns.SeriesKey = table.SeriesKey;
            RequestAttributes theObject = broker.Insert(updateColumns);
            update.Commit();
            return theObject;
        }
        #endregion
    }
}
