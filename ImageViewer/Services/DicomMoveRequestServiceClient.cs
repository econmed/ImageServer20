﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class DicomSendRequest : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string DestinationAETitleField;
        
        private string DestinationHostNameField;
        
        private int PortField;
        
        private string[] UidsField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string DestinationAETitle
        {
            get
            {
                return this.DestinationAETitleField;
            }
            set
            {
                this.DestinationAETitleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string DestinationHostName
        {
            get
            {
                return this.DestinationHostNameField;
            }
            set
            {
                this.DestinationHostNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Port
        {
            get
            {
                return this.PortField;
            }
            set
            {
                this.PortField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string[] Uids
        {
            get
            {
                return this.UidsField;
            }
            set
            {
                this.UidsField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class DicomRetrieveRequest : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int PortField;
        
        private ClearCanvas.ImageViewer.Shreds.DicomServer.RetrieveLevel RetrieveLevelField;
        
        private string SourceAETitleField;
        
        private string SourceHostNameField;
        
        private string[] UidsField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Port
        {
            get
            {
                return this.PortField;
            }
            set
            {
                this.PortField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public ClearCanvas.ImageViewer.Shreds.DicomServer.RetrieveLevel RetrieveLevel
        {
            get
            {
                return this.RetrieveLevelField;
            }
            set
            {
                this.RetrieveLevelField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string SourceAETitle
        {
            get
            {
                return this.SourceAETitleField;
            }
            set
            {
                this.SourceAETitleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string SourceHostName
        {
            get
            {
                return this.SourceHostNameField;
            }
            set
            {
                this.SourceHostNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string[] Uids
        {
            get
            {
                return this.UidsField;
            }
            set
            {
                this.UidsField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    public enum RetrieveLevel : int
    {
        
        Study = 0,
        
        Series = 1,
        
        Image = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class GetServerSettingResponse : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string AETitleField;
        
        private string HostNameField;
        
        private string InterimStorageDirectoryField;
        
        private int PortField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string AETitle
        {
            get
            {
                return this.AETitleField;
            }
            set
            {
                this.AETitleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string HostName
        {
            get
            {
                return this.HostNameField;
            }
            set
            {
                this.HostNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string InterimStorageDirectory
        {
            get
            {
                return this.InterimStorageDirectoryField;
            }
            set
            {
                this.InterimStorageDirectoryField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Port
        {
            get
            {
                return this.PortField;
            }
            set
            {
                this.PortField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class UpdateServerSettingRequest : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string AETitleField;
        
        private string HostNameField;
        
        private string InterimStorageDirectoryField;
        
        private int PortField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string AETitle
        {
            get
            {
                return this.AETitleField;
            }
            set
            {
                this.AETitleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string HostName
        {
            get
            {
                return this.HostNameField;
            }
            set
            {
                this.HostNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string InterimStorageDirectory
        {
            get
            {
                return this.InterimStorageDirectoryField;
            }
            set
            {
                this.InterimStorageDirectoryField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Port
        {
            get
            {
                return this.PortField;
            }
            set
            {
                this.PortField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IDicomMoveRequestService")]
public interface IDicomMoveRequestService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDicomMoveRequestService/Send", ReplyAction="http://tempuri.org/IDicomMoveRequestService/SendResponse")]
    void Send(ClearCanvas.ImageViewer.Shreds.DicomServer.DicomSendRequest request);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDicomMoveRequestService/Retrieve", ReplyAction="http://tempuri.org/IDicomMoveRequestService/RetrieveResponse")]
    void Retrieve(ClearCanvas.ImageViewer.Shreds.DicomServer.DicomRetrieveRequest request);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDicomMoveRequestService/GetServerSetting", ReplyAction="http://tempuri.org/IDicomMoveRequestService/GetServerSettingResponse")]
    ClearCanvas.ImageViewer.Shreds.DicomServer.GetServerSettingResponse GetServerSetting();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDicomMoveRequestService/UpdateServerSetting", ReplyAction="http://tempuri.org/IDicomMoveRequestService/UpdateServerSettingResponse")]
    void UpdateServerSetting(ClearCanvas.ImageViewer.Shreds.DicomServer.UpdateServerSettingRequest request);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface IDicomMoveRequestServiceChannel : IDicomMoveRequestService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class DicomMoveRequestServiceClient : System.ServiceModel.ClientBase<IDicomMoveRequestService>, IDicomMoveRequestService
{
    
    public DicomMoveRequestServiceClient()
    {
    }
    
    public DicomMoveRequestServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public DicomMoveRequestServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public DicomMoveRequestServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public DicomMoveRequestServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public void Send(ClearCanvas.ImageViewer.Shreds.DicomServer.DicomSendRequest request)
    {
        base.Channel.Send(request);
    }
    
    public void Retrieve(ClearCanvas.ImageViewer.Shreds.DicomServer.DicomRetrieveRequest request)
    {
        base.Channel.Retrieve(request);
    }
    
    public ClearCanvas.ImageViewer.Shreds.DicomServer.GetServerSettingResponse GetServerSetting()
    {
        return base.Channel.GetServerSetting();
    }
    
    public void UpdateServerSetting(ClearCanvas.ImageViewer.Shreds.DicomServer.UpdateServerSettingRequest request)
    {
        base.Channel.UpdateServerSetting(request);
    }
}
