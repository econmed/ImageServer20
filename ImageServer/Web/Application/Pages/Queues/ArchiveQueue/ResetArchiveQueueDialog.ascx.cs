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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue
{
	public partial class ResetArchiveQueueDialog : System.Web.UI.UserControl
	{
		#region Private Members
		private IList<Model.ArchiveQueue> _archiveQueueItemList;
		#endregion Private Members
		
		#region Public Properties
		/// <summary>
		/// Sets / Gets the <see cref="ServerEntityKey"/> of the <see cref="WorkQueue"/> item associated with this dialog
		/// </summary>
		/// 
		public IList<Model.ArchiveQueue> ArchiveQueueItemList
		{
			get { return _archiveQueueItemList; }
			set { _archiveQueueItemList = value; }
		}

		#endregion Public Properties

		#region Protected Methods

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			PreResetConfirmDialog.Confirmed += PreResetConfirmDialog_Confirmed;
		}

		#endregion Protected Methods

		#region Private Methods
		void PreResetConfirmDialog_Confirmed(object data)
		{
			List<Model.ArchiveQueue> key = data as List<Model.ArchiveQueue>;

			if (key != null)
			{
					ArchiveQueueController controller = new ArchiveQueueController();
					DateTime scheduledTime = key[0].ScheduledTime;
					if (scheduledTime < Platform.Time)
						scheduledTime = Platform.Time.AddSeconds(60);

					bool successful = false;
					try
					{
						successful = controller.ResetArchiveQueueItem(key, scheduledTime);
						if (successful)
						{
							Platform.Log(LogLevel.Info, "Archive Queue item(s) reset by user ");
						}
						else
						{
							Platform.Log(LogLevel.Error,
										 "PreResetConfirmDialog_Confirmed: Unable to reset archive queue item.");

							MessageBox.Message = App_GlobalResources.SR.ArchiveQueueResetFailed;
							MessageBox.MessageType =
								MessageBox.MessageTypeEnum.ERROR;
							MessageBox.Show();
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e,
										 "PreResetConfirmDialog_Confirmed: Unable to reset work queue item." );

						MessageBox.Message = App_GlobalResources.SR.ArchiveQueueResetFailed;
						MessageBox.MessageType =
							MessageBox.MessageTypeEnum.ERROR;
						MessageBox.Show();
					}
				
			}
		}

		#endregion Private Methods

		#region Public Methods

		public override void DataBind()
		{
			if (ArchiveQueueItemList != null)
			{
				ArchiveQueueAdaptor adaptor = new ArchiveQueueAdaptor();
			}

			base.DataBind();
		}

		/// <summary>
		/// Displays the dialog box for reseting the <see cref="WorkQueue"/> entry.
		/// </summary>
		/// <remarks>
		/// The <see cref="ArchiveQueueItemList"/> to be deleted must be set prior to calling <see cref="Show"/>.
		/// </remarks>

		public void Show()
		{
			DataBind();

			if (_archiveQueueItemList != null)
			{
				PreResetConfirmDialog.Data = ArchiveQueueItemList;
				PreResetConfirmDialog.MessageType =
					MessageBox.MessageTypeEnum.YESNO;
				if (_archiveQueueItemList.Count > 1)
					PreResetConfirmDialog.Message = App_GlobalResources.SR.MultipleArchiveQueueResetConfirm;
				else
					PreResetConfirmDialog.Message = App_GlobalResources.SR.ArchiveQueueResetConfirm;
				PreResetConfirmDialog.Show();
			}
		}

		/// <summary>
		/// Closes the dialog box
		/// </summary>
		public void Hide()
		{
			PreResetConfirmDialog.Close();
			MessageBox.Close();
		}

		#endregion Public Methods
	}
}