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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.Upgrade
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class ModelUpgradeApplication : IApplicationRoot
	{
		public void RunApplication(string[] args)
		{
			ModelUpgradeCommandLine cmdLine = new ModelUpgradeCommandLine();
			try
			{
				cmdLine.Parse(args);

				DatabaseVersion version = LoadVersion();
				
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
			}
		}

		DatabaseVersion LoadVersion()
		{
			using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				IDatabaseVersionEntityBroker broker = read.GetBroker<IDatabaseVersionEntityBroker>();
				DatabaseVersionSelectCriteria criteria = new DatabaseVersionSelectCriteria();
				IList<DatabaseVersion> versions = broker.Find(criteria);
				if (versions.Count == 0)
					return null;

				return CollectionUtils.FirstElement(versions);
			}
		}

		public void ExecuteSql(SqlConnection connection, IUpgradeScript script)
		{
			string sql = script.GetScript();

			Regex regex = new Regex("^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			string[] lines = regex.Split(sql);

			SqlTransaction transaction = connection.BeginTransaction();
			using (SqlCommand cmd = connection.CreateCommand())
			{
				cmd.Connection = connection;
				cmd.Transaction = transaction;

				foreach (string line in lines)
				{
					if (line.Length > 0)
					{
						cmd.CommandText = line;
						cmd.CommandType = CommandType.Text;

						try
						{
							cmd.ExecuteNonQuery();
						}
						catch (SqlException)
						{
							transaction.Rollback();
							throw;
						}
					}
				}
			}

			transaction.Commit();
		}
	}
}
