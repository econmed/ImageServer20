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
using NHibernate.Metadata;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;
using System.Collections;
using NHibernate.Type;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Adds foreign-key constraints on hard-enum columns to the Hibernate relational model, since Hibernate does not know
    /// to add these constraints automatically.
    /// </summary>
    class EnumForeignKeyProcessor : IDdlPreProcessor
    {
        public void Process(PersistentStore store)
        {
            foreach (PersistentClass pc in store.Configuration.ClassMappings)
            {
                CreateConstraints(store, pc.PropertyCollection);
            }
        }

        private void CreateConstraints(PersistentStore store, ICollection properties)
        {
            foreach (Property prop in properties)
            {
                if (prop.Value is Component)
                {
                    // recur on component properties
                    Component comp = prop.Value as Component;
                    CreateConstraints(store, comp.PropertyCollection);
                }
                else if (prop.Value is Collection)
                {
                    // recur on collections-of-values (composite-element)
                    Collection coll = prop.Value as Collection;
                    if (coll.Element is Component)
                    {
                        Component comp = coll.Element as Component;
                        CreateConstraints(store, comp.PropertyCollection);
                    }
                }
                else
                {
                    // is this property mapped with an EnumHbm class???
                    if (prop.Type is EnumStringType)
                    {
                        Type enumClass = GetEnumValueClassForEnumType(prop.Type.ReturnedClass);

                        // build a constraint for this column
                        Table constrainedTable = prop.Value.Table;
                        Column constrainedColumn = CollectionUtils.FirstElement<Column>(prop.ColumnCollection);
                        Table referencedTable = GetTableForEnumClass(enumClass, store);

                        // don't really know if this will be unique or not...
                        int unique = constrainedTable.Name.GetHashCode() ^ constrainedColumn.Name.GetHashCode() ^ referencedTable.Name.GetHashCode();
                        string fkName = string.Format("FK{0}", unique.ToString("X"));

                        constrainedTable.CreateForeignKey(fkName, new Column[] { constrainedColumn }, enumClass);
                    }
                }
            }
        }

        private Type GetEnumValueClassForEnumType(Type enumType)
        {
            EnumValueClassAttribute attr = CollectionUtils.FirstElement<EnumValueClassAttribute>(
                enumType.GetCustomAttributes(typeof(EnumValueClassAttribute), false));

            if (attr == null)
                throw new Exception(string.Format("{0} is not marked with the EnumValueClassAttribute", enumType.FullName));

            return attr.EnumValueClass;
        }

        private Table GetTableForEnumClass(Type enumClass, PersistentStore store)
        {
            PersistentClass pclass = CollectionUtils.SelectFirst<PersistentClass>(store.Configuration.ClassMappings,
                delegate(PersistentClass c) { return c.MappedClass == enumClass; });

            if (pclass == null)
                throw new Exception(string.Format("{0} is not a persistent class", enumClass.FullName));

            return pclass.Table;
        }

        #region unused

        private void Write(string text, int depth)
        {
            string tabs = "";
            for (int i = 0; i < depth; i++) tabs += "\t";
            Console.WriteLine(tabs + text);
        }

        private void WriteProperties(IEnumerable properties, int depth)
        {
            foreach (Property prop in properties)
            {
                if (prop.Value is Component)
                {
                    Write(prop.Name, depth);
                    Component comp = prop.Value as Component;
                    WriteProperties(comp.PropertyCollection, depth + 1);
                }
                else if (prop.Value is Collection)
                {
                    Write(prop.Name, depth);
                    Collection coll = prop.Value as Collection;
                    if (coll.Element is Component)
                    {
                        Component comp = coll.Element as Component;
                        WriteProperties(comp.PropertyCollection, depth + 1);
                    }
                }
                else
                {
                    if (prop.Type is EnumStringType)
                    {
                        Write(prop.Name, depth);
                        foreach (Column col in prop.ColumnCollection)
                        {
                            Write(prop.Value.Table.Name + "." + col.Name + ": " + prop.Type.ReturnedClass.FullName, depth + 1);
                        }
                    }
                    else
                    {
                        Write(prop.Name, depth);
                    }
                }
            }
        }
        #endregion
    }
}
