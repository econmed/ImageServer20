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

using System.Collections.Generic;
using ClearCanvas.Common.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
	public class ChangeSetRecorder : IEntityChangeSetRecorder
	{
		private string _operationName;
		public string OperationName
		{
			get { return _operationName; }
			set { _operationName = value; }
		}

		public void WriteLogEntry(IEnumerable<EntityChange> changeSet, AuditLog auditLog)
		{
			AuditLogEntry entry = CreateLogEntry(changeSet);
			auditLog.WriteEntry(entry.Category, entry.Details);
		}

		public AuditLogEntry CreateLogEntry(IEnumerable<EntityChange> changeSet)
		{
			string details = string.Empty;
			string type = string.Empty;
			foreach (EntityChange change in changeSet)
			{
				if (change.ChangeType == EntityChangeType.Create)
				{
					type = "Create";
				}
				else if (change.ChangeType == EntityChangeType.Delete)
				{
					type = "Delete";
				}
				else if (change.ChangeType == EntityChangeType.Update)
				{
					type = "Update";
				}
			}
			return new AuditLogEntry("ImageServer", type, details);			
		}
	}
}
