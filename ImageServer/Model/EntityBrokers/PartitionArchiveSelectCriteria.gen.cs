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

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    using System;
    using System.Xml;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;

    public partial class PartitionArchiveSelectCriteria : EntitySelectCriteria
    {
        public PartitionArchiveSelectCriteria()
        : base("PartitionArchive")
        {}
        public PartitionArchiveSelectCriteria(PartitionArchiveSelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new PartitionArchiveSelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ServerPartitionGUID")]
        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("ServerPartitionKey"))
              {
                 SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["ServerPartitionKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ArchiveTypeEnum")]
        public ISearchCondition<ArchiveTypeEnum> ArchiveTypeEnum
        {
            get
            {
              if (!SubCriteria.ContainsKey("ArchiveTypeEnum"))
              {
                 SubCriteria["ArchiveTypeEnum"] = new SearchCondition<ArchiveTypeEnum>("ArchiveTypeEnum");
              }
              return (ISearchCondition<ArchiveTypeEnum>)SubCriteria["ArchiveTypeEnum"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="Description")]
        public ISearchCondition<String> Description
        {
            get
            {
              if (!SubCriteria.ContainsKey("Description"))
              {
                 SubCriteria["Description"] = new SearchCondition<String>("Description");
              }
              return (ISearchCondition<String>)SubCriteria["Description"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="Enabled")]
        public ISearchCondition<Boolean> Enabled
        {
            get
            {
              if (!SubCriteria.ContainsKey("Enabled"))
              {
                 SubCriteria["Enabled"] = new SearchCondition<Boolean>("Enabled");
              }
              return (ISearchCondition<Boolean>)SubCriteria["Enabled"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ReadOnly")]
        public ISearchCondition<Boolean> ReadOnly
        {
            get
            {
              if (!SubCriteria.ContainsKey("ReadOnly"))
              {
                 SubCriteria["ReadOnly"] = new SearchCondition<Boolean>("ReadOnly");
              }
              return (ISearchCondition<Boolean>)SubCriteria["ReadOnly"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ArchiveDelayHours")]
        public ISearchCondition<Int32> ArchiveDelayHours
        {
            get
            {
              if (!SubCriteria.ContainsKey("ArchiveDelayHours"))
              {
                 SubCriteria["ArchiveDelayHours"] = new SearchCondition<Int32>("ArchiveDelayHours");
              }
              return (ISearchCondition<Int32>)SubCriteria["ArchiveDelayHours"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ConfigurationXml")]
        public ISearchCondition<XmlDocument> ConfigurationXml
        {
            get
            {
              if (!SubCriteria.ContainsKey("ConfigurationXml"))
              {
                 SubCriteria["ConfigurationXml"] = new SearchCondition<XmlDocument>("ConfigurationXml");
              }
              return (ISearchCondition<XmlDocument>)SubCriteria["ConfigurationXml"];
            } 
        }
    }
}
