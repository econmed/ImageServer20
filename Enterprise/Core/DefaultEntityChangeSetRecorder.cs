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
using System.Text;
using System.IO;
using System.Xml;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Default implementation of <see cref="IEntityChangeSetRecorder"/>.
    /// </summary>
    public class DefaultEntityChangeSetRecorder : IEntityChangeSetRecorder
    {
        private const string LargeValue = "{large-value}";
        private const string NullValue = "{null}";
        private const string MultiValued = "{multiple-values}";


        private string _operationName;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DefaultEntityChangeSetRecorder()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="operationName"></param>
        public DefaultEntityChangeSetRecorder(string operationName)
        {
            _operationName = operationName;
        }

        /// <summary>
        /// Gets or sets a logical operation name for the operation that produced the change set.
        /// </summary>
        public string OperationName
        {
            get { return _operationName; }
            set { _operationName = value; }
        }


        #region ITransactionLogger Members

        /// <summary>
        /// Creates a <see cref="AuditLogEntry"/> for the specified change set.
        /// </summary>
        /// <param name="changeSet"></param>
        /// <returns></returns>
        public AuditLogEntry CreateLogEntry(IEnumerable<EntityChange> changeSet)
        {
            return new AuditLogEntry("ChangeSet", _operationName, WriteXml(changeSet));
        }

        #endregion

        private string WriteXml(IEnumerable<EntityChange> changeSet)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                WriteChangeSet(writer, changeSet);
                return sw.ToString();
            }
        }

        private void WriteChangeSet(XmlWriter writer, IEnumerable<EntityChange> changeSet)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("change-set");
            writer.WriteAttributeString("operation", StringUtilities.EmptyIfNull(_operationName));
            foreach (EntityChange entityChange in changeSet)
            {
                writer.WriteStartElement("action");
                writer.WriteAttributeString("type", entityChange.ChangeType.ToString());
                writer.WriteAttributeString("class", EntityRefUtils.GetClass(entityChange.EntityRef).FullName);
                writer.WriteAttributeString("oid", EntityRefUtils.GetOID(entityChange.EntityRef).ToString());
                writer.WriteAttributeString("version", EntityRefUtils.GetVersion(entityChange.EntityRef).ToString());

                // for creates and updates, write property changes
                if(entityChange.ChangeType != EntityChangeType.Delete)
                {
                    WriteProperties(writer, entityChange.ChangeType, entityChange.PropertyChanges);
                }

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private void WriteProperties(XmlWriter writer, EntityChangeType changeType, IEnumerable<PropertyChange> propertyChanges)
        {
            foreach (PropertyChange prop in propertyChanges)
            {
                writer.WriteStartElement("property");
                writer.WriteAttributeString("name", prop.PropertyName);
                if (changeType == EntityChangeType.Update)
                {
                    WritePropertyValue(writer, "old-value", prop.OldValue);
                }
                WritePropertyValue(writer, "new-value", prop.NewValue);
                writer.WriteEndElement();
            }
        }

        private void WritePropertyValue(XmlWriter writer, string tagName, object propertyValue)
        {
            writer.WriteElementString(tagName, GetSerializedValue(propertyValue));   
        }

        private string GetSerializedValue(object value)
        {
            if(value == null)
                return NullValue;

            if(value is string)
            {
                string s = value as string;
                return s.Length > 255 ? LargeValue : s;
            }
            // use ISO format for date times, because it is easy to parse
            if(value is DateTime)
                return DateTimeUtils.FormatISO((DateTime)value);

            // for entities, write the class name and object id
            if(value is Entity)
            {
                Entity entity = value as Entity;
                return string.Format("{0}:{1}", entity.GetClass().FullName, entity.OID);
            }

            // for enum values, write the code
            if(value is EnumValue)
                return (value as EnumValue).Code;

            // don't support dealing with collection properties at the present time
            // just write out that it is multi-valued
            if(value is ICollection || value is IList)
                return MultiValued;

            // for all other values, including components (ValueObject subclasses)
            // just call ToString() and hope for the best ;)
            return value.ToString();
        }

    }
}
