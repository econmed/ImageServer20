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
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using NHibernate.Cfg;
using NHibernate.Dialect;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Generates scripts to create the tables, foreign key constraints, and indexes.
    /// </summary>
    class RelationalSchemaGenerator : DdlScriptGenerator
    {
    	private readonly EnumOptions _enumOption;

		public RelationalSchemaGenerator(EnumOptions enumOption)
		{
			_enumOption = enumOption;
		}

        #region IDdlScriptGenerator Members

        public override string[] GenerateCreateScripts(Configuration config)
        {
			RelationalModelInfo currentModel = new RelationalModelInfo(config);
			RelationalModelInfo baselineModel = new RelationalModelInfo();		// baseline model is empty

            return GetScripts(config, baselineModel, currentModel);
		}

    	public override string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel)
    	{
    		RelationalModelInfo currentModel = new RelationalModelInfo(config);

    		return GetScripts(config, baselineModel, currentModel);
    	}

    	public override string[] GenerateDropScripts(Configuration config)
        {
            return new string[]{};
        }

        #endregion

		private string[] GetScripts(Configuration config, RelationalModelInfo baselineModel, RelationalModelInfo currentModel)
		{
			RelationalModelComparator comparator = new RelationalModelComparator(_enumOption);
			RelationalModelTransform transform = comparator.CompareModels(baselineModel, currentModel);

			IRenderer renderer = Renderer.GetRenderer(config);
			return CollectionUtils.Map<Statement, string>(transform.Render(renderer),
					delegate(Statement s) { return s.Sql; }).ToArray();
		}

	}
}
