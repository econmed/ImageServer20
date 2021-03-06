#region License

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

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Collections.Generic;
    using ClearCanvas.ImageServer.Model.EntityBrokers;
    using ClearCanvas.ImageServer.Enterprise;
    using System.Reflection;

[Serializable]
public partial class DeviceTypeEnum : ServerEnum
{
      #region Private Static Members
      private static readonly DeviceTypeEnum _Workstation = GetEnum("Workstation");
      private static readonly DeviceTypeEnum _Modality = GetEnum("Modality");
      private static readonly DeviceTypeEnum _Server = GetEnum("Server");
      private static readonly DeviceTypeEnum _Broker = GetEnum("Broker");
      #endregion

      #region Public Static Properties
      /// <summary>
      /// Workstation
      /// </summary>
      public static DeviceTypeEnum Workstation
      {
          get { return _Workstation; }
      }
      /// <summary>
      /// Modality
      /// </summary>
      public static DeviceTypeEnum Modality
      {
          get { return _Modality; }
      }
      /// <summary>
      /// Server
      /// </summary>
      public static DeviceTypeEnum Server
      {
          get { return _Server; }
      }
      /// <summary>
      /// Broker
      /// </summary>
      public static DeviceTypeEnum Broker
      {
          get { return _Broker; }
      }

      #endregion

      #region Constructors
      public DeviceTypeEnum():base("DeviceTypeEnum")
      {}
      #endregion
      #region Public Members
      public override void SetEnum(short val)
      {
          ServerEnumHelper<DeviceTypeEnum, IDeviceTypeEnumBroker>.SetEnum(this, val);
      }
      static public List<DeviceTypeEnum> GetAll()
      {
          return ServerEnumHelper<DeviceTypeEnum, IDeviceTypeEnumBroker>.GetAll();
      }
      static public DeviceTypeEnum GetEnum(string lookup)
      {
          return ServerEnumHelper<DeviceTypeEnum, IDeviceTypeEnumBroker>.GetEnum(lookup);
      }
      #endregion
}
}
