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
using System.IO;
using System.Collections.Generic;

using NHibernate.Cfg;
using ClearCanvas.Common;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Defines an extension point for contributing DDL script generators to the database definition.
    /// Note that extensions of this point must not have dependencies on the existence of other database objects,
    /// as the order in which each extension is processed is not deterministic.
    /// </summary>
    [ExtensionPoint]
    public class DdlScriptGeneratorExtensionPoint : ExtensionPoint<IDdlScriptGenerator>
    {
    }

    /// <summary>
    /// Utility class that generates a database creation and/or drop script and writes the script to a <see cref="StreamWriter"/>.
    /// </summary>
    public class ScriptWriter
    {
        private List<IDdlScriptGenerator> _generators;
        private PersistentStore _store;
        private Dialect _dialect;

        public ScriptWriter(PersistentStore store, Dialect dialect)
        {
            _store = store;
            _dialect = dialect;

            _generators = new List<IDdlScriptGenerator>();

            // the order of generator execution is important, so add the static generators first
            _generators.Add(new TableSchemaGenerator());
            _generators.Add(new EnumForeignKeyConstraintGenerator());
            _generators.Add(new EnumValueInsertGenerator());

            // subsequently we can add extension generators, with uncontrolled ordering
            foreach (IDdlScriptGenerator generator in (new DdlScriptGeneratorExtensionPoint().CreateExtensions()))
            {
                _generators.Add(generator);
            }
        }

        /// <summary>
        /// Writes a database creation script to the specified <see cref="TextWriter"/>
        /// </summary>
        /// <param name="sw"></param>
        public void WriteCreateScript(TextWriter sw)
        {
            foreach (IDdlScriptGenerator gen in _generators)
            {
                foreach (string script in gen.GenerateCreateScripts(_store, _dialect))
                {
                    sw.WriteLine(script);
                }
            }
        }

        /// <summary>
        /// Writes a database drop script to the specified <see cref="StreamWriter"/>
        /// </summary>
        /// <param name="sw"></param>
        public void WriteDropScript(StreamWriter sw)
        {
            foreach (IDdlScriptGenerator gen in _generators)
            {
                foreach (string script in gen.GenerateDropScripts(_store, _dialect))
                {
                    sw.WriteLine(script);
                }
            }
        }
    }
}
