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
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Decoded information of the ChangeDescription field of a <see cref="StudyHistory"/> 
	/// record whose type is "WebEdited"
	/// </summary>
	public class WebEditStudyHistoryChangeDescription
	{
		#region Public Properties

		/// <summary>
		/// Type of the edit operation occured on the study.
		/// </summary>
		[XmlElement("EditType")]
		public EditType EditType { get; set; }

		/// <summary>
		/// Reason that the study is being editted
		/// </summary>
		[XmlElement("Reason")]
		public string Reason { get; set; }

		/// <summary>
		/// List of <see cref="BaseImageLevelUpdateCommand"/> that were executed on the study.
		/// </summary>
		[XmlArrayItem("Command", Type = typeof (AbstractProperty<BaseImageLevelUpdateCommand>))]
		public List<BaseImageLevelUpdateCommand> UpdateCommands { get; set; }

		[XmlElement("UserId")]
		public string UserId { get; set; }

		[XmlElement("TimeStamp")]
		public DateTime? TimeStamp { get; set; }

		#endregion
	}
}