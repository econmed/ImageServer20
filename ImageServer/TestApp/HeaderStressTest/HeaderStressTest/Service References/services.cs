﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HeaderStressTest.services
{
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="HeaderRetrievalParameters", Namespace="http://schemas.datacontract.org/2004/07/ClearCanvas.ImageServer.Services.Streamin" +
        "g.HeaderRetrieval")]
    [System.SerializableAttribute()]
    public partial class HeaderRetrievalParameters : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string ReferenceIDField;
        
        private string ServerAETitleField;
        
        private string StudyInstanceUIDField;
        
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
        public string ReferenceID
        {
            get
            {
                return this.ReferenceIDField;
            }
            set
            {
                this.ReferenceIDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string ServerAETitle
        {
            get
            {
                return this.ServerAETitleField;
            }
            set
            {
                this.ServerAETitleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string StudyInstanceUID
        {
            get
            {
                return this.StudyInstanceUIDField;
            }
            set
            {
                this.StudyInstanceUIDField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="HeaderStressTest.services.IHeaderRetrievalService")]
    public interface IHeaderRetrievalService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHeaderRetrievalService/GetStudyHeader", ReplyAction="http://tempuri.org/IHeaderRetrievalService/GetStudyHeaderResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://tempuri.org/IHeaderRetrievalService/GetStudyHeaderStringFault", Name="string", Namespace="http://schemas.microsoft.com/2003/10/Serialization/")]
        System.IO.Stream GetStudyHeader(string callingAETitle, HeaderStressTest.services.HeaderRetrievalParameters parameters);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IHeaderRetrievalServiceChannel : HeaderStressTest.services.IHeaderRetrievalService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class HeaderRetrievalServiceClient : System.ServiceModel.ClientBase<HeaderStressTest.services.IHeaderRetrievalService>, HeaderStressTest.services.IHeaderRetrievalService
    {
        
        public HeaderRetrievalServiceClient()
        {
        }
        
        public HeaderRetrievalServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public HeaderRetrievalServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public HeaderRetrievalServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public HeaderRetrievalServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.IO.Stream GetStudyHeader(string callingAETitle, HeaderStressTest.services.HeaderRetrievalParameters parameters)
        {
            return base.Channel.GetStudyHeader(callingAETitle, parameters);
        }
    }
}
