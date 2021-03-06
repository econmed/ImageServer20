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
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;

namespace ClearCanvas.ImageServer.Rules.AutoRouteAction
{
    /// <summary>
    /// Class for implementing auto-route action as specified by <see cref="IActionItem{T}"/>
    /// </summary>
    public class AutoRouteActionItem : ServerActionItemBase
    {
        readonly private string _device;
    	private readonly DateTime? _startTime;
		private readonly DateTime? _endTime;
        #region Constructors

        public AutoRouteActionItem(string device)
            : base("AutoRoute Action")
        {
            _device = device;
        }

		public AutoRouteActionItem(string device, DateTime startTime, DateTime endTime)
			: base("AutoRoute Action")
		{
			_device = device;
			_startTime = startTime;
			_endTime = endTime;
		}

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool OnExecute(ServerActionContext context)
        {
            InsertAutoRouteCommand command;

			if (_startTime!=null && _endTime!=null)
			{
				DateTime now = Platform.Time;
				TimeSpan nowTimeOfDay = now.TimeOfDay;
				if (_startTime.Value > _endTime.Value)
				{
					if (nowTimeOfDay > _startTime.Value.TimeOfDay
						|| nowTimeOfDay < _endTime.Value.TimeOfDay)
					{
						command = new InsertAutoRouteCommand(context, _device);		
					}
					else
					{
						DateTime scheduledTime = now.Date.Add(_startTime.Value.TimeOfDay);
						command = new InsertAutoRouteCommand(context, _device, scheduledTime);
					}
				}
				else
				{
					if (nowTimeOfDay > _startTime.Value.TimeOfDay
						&& nowTimeOfDay < _endTime.Value.TimeOfDay )
					{
						command = new InsertAutoRouteCommand(context, _device);
					}
					else
					{
						if (nowTimeOfDay < _startTime.Value.TimeOfDay)
						{
							DateTime scheduledTime = now.Date.Add(_startTime.Value.TimeOfDay);
							command = new InsertAutoRouteCommand(context, _device, scheduledTime);
						}
						else
						{
							DateTime scheduledTime = now.Date.Date.AddDays(1d).Add(_startTime.Value.TimeOfDay);
							command = new InsertAutoRouteCommand(context, _device, scheduledTime);
						}
					}
				}				
			}
			else
				command = new InsertAutoRouteCommand(context, _device);

            if (context.CommandProcessor != null)
                context.CommandProcessor.AddCommand(command);
            else
            {
                try
                {
                    command.Execute(context.CommandProcessor);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when inserting auto-route request");

                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}