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

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Xml;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class CannedText: ServerEntity
    {
        #region Constructors
        public CannedText():base("CannedText")
        {}
        public CannedText(
             String _label_
            ,String _category_
            ,String _text_
            ):base("CannedText")
        {
            Label = _label_;
            Category = _category_;
            Text = _text_;
        }
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="CannedText", ColumnName="Label")]
        public String Label
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="CannedText", ColumnName="Category")]
        public String Category
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="CannedText", ColumnName="Text")]
        public String Text
        { get; set; }
        #endregion

        #region Static Methods
        static public CannedText Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public CannedText Load(IPersistenceContext read, ServerEntityKey key)
        {
            ICannedTextEntityBroker broker = read.GetBroker<ICannedTextEntityBroker>();
            CannedText theObject = broker.Load(key);
            return theObject;
        }
        static public CannedText Insert(CannedText entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                CannedText newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public CannedText Insert(IUpdateContext update, CannedText entity)
        {
            ICannedTextEntityBroker broker = update.GetBroker<ICannedTextEntityBroker>();
            CannedTextUpdateColumns updateColumns = new CannedTextUpdateColumns();
            updateColumns.Label = entity.Label;
            updateColumns.Category = entity.Category;
            updateColumns.Text = entity.Text;
            CannedText newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
