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
using System.Configuration;
using System.Data.SqlClient;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// SQL Server implementation of <see cref="IPersistentStore"/>.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreExtensionPoint))]
    public class PersistentStore : IPersistentStore
    {
        private String _connectionString;
        private ITransactionNotifier _transactionNotifier;

        #region IPersistentStore Members

        public void Initialize()
        {
            // Retrieve the partial connection string named databaseConnection
            // from the application's app.config or web.config file.
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings["ImageServerConnectString"];

            if (null != settings)
            {
                // Retrieve the partial connection string.
                _connectionString = settings.ConnectionString;
            }
        }

        public void SetTransactionNotifier(ITransactionNotifier transactionNotifier)
        {
            _transactionNotifier = transactionNotifier;
        }

        public IReadContext OpenReadContext()
        {
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);
                
                connection.Open();

                return new ReadContext(connection, _transactionNotifier);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e);
                throw new PersistenceException("Unexpected exception opening database connection", e);
            }
        }

        public IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode)
        {
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);

                connection.Open();

                return new UpdateContext(connection, _transactionNotifier, mode);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e);
                throw new PersistenceException("Unexpected exception opening database connection", e);
            }
        }

        #endregion
    }
}
