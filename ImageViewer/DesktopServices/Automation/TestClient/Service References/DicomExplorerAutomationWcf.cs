﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation
{
    using System.Runtime.Serialization;
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchRemoteStudiesRequest))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchLocalStudiesRequest))]
    public partial class SearchStudiesRequest : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.DicomExplorerSearchCriteria SearchCriteriaField;
        
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
        public ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.DicomExplorerSearchCriteria SearchCriteria
        {
            get
            {
                return this.SearchCriteriaField;
            }
            set
            {
                this.SearchCriteriaField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    public partial class DicomExplorerSearchCriteria : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string AccessionNumberField;
        
        private System.ComponentModel.BindingList<string> ModalitiesField;
        
        private string PatientIdField;
        
        private string PatientsNameField;
        
        private System.Nullable<System.DateTime> StudyDateFromField;
        
        private System.Nullable<System.DateTime> StudyDateToField;
        
        private string StudyDescriptionField;
        
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
        public string AccessionNumber
        {
            get
            {
                return this.AccessionNumberField;
            }
            set
            {
                this.AccessionNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.ComponentModel.BindingList<string> Modalities
        {
            get
            {
                return this.ModalitiesField;
            }
            set
            {
                this.ModalitiesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string PatientId
        {
            get
            {
                return this.PatientIdField;
            }
            set
            {
                this.PatientIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string PatientsName
        {
            get
            {
                return this.PatientsNameField;
            }
            set
            {
                this.PatientsNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Nullable<System.DateTime> StudyDateFrom
        {
            get
            {
                return this.StudyDateFromField;
            }
            set
            {
                this.StudyDateFromField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Nullable<System.DateTime> StudyDateTo
        {
            get
            {
                return this.StudyDateToField;
            }
            set
            {
                this.StudyDateToField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string StudyDescription
        {
            get
            {
                return this.StudyDescriptionField;
            }
            set
            {
                this.StudyDescriptionField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    public partial class SearchRemoteStudiesRequest : ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchStudiesRequest
    {
        
        private string AETitleField;
        
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    public partial class SearchLocalStudiesRequest : ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchStudiesRequest
    {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    public partial class SearchLocalStudiesResult : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    public partial class SearchRemoteStudiesResult : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    public partial class DicomExplorerNotFoundFault : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    public partial class NoLocalStoreFault : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation")]
    [System.SerializableAttribute()]
    public partial class ServerNotFoundFault : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.clearcanvas.ca/imageViewer/automation", ConfigurationName="ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutoma" +
        "tion.IDicomExplorerAutomation")]
    public interface IDicomExplorerAutomation
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.clearcanvas.ca/imageViewer/automation/IDicomExplorerAutomation/SearchL" +
            "ocalStudies", ReplyAction="http://www.clearcanvas.ca/imageViewer/automation/IDicomExplorerAutomation/SearchL" +
            "ocalStudiesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.DicomExplorerNotFoundFault), Action="http://www.clearcanvas.ca/imageViewer/automation/IDicomExplorerAutomation/SearchL" +
            "ocalStudiesDicomExplorerNotFoundFaultFault", Name="DicomExplorerNotFoundFault")]
        [System.ServiceModel.FaultContractAttribute(typeof(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.NoLocalStoreFault), Action="http://www.clearcanvas.ca/imageViewer/automation/IDicomExplorerAutomation/SearchL" +
            "ocalStudiesNoLocalStoreFaultFault", Name="NoLocalStoreFault")]
        ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchLocalStudiesResult SearchLocalStudies(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchLocalStudiesRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.clearcanvas.ca/imageViewer/automation/IDicomExplorerAutomation/SearchR" +
            "emoteStudies", ReplyAction="http://www.clearcanvas.ca/imageViewer/automation/IDicomExplorerAutomation/SearchR" +
            "emoteStudiesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.ServerNotFoundFault), Action="http://www.clearcanvas.ca/imageViewer/automation/IDicomExplorerAutomation/SearchR" +
            "emoteStudiesServerNotFoundFaultFault", Name="ServerNotFoundFault")]
        [System.ServiceModel.FaultContractAttribute(typeof(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.DicomExplorerNotFoundFault), Action="http://www.clearcanvas.ca/imageViewer/automation/IDicomExplorerAutomation/SearchR" +
            "emoteStudiesDicomExplorerNotFoundFaultFault", Name="DicomExplorerNotFoundFault")]
        ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchRemoteStudiesResult SearchRemoteStudies(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchRemoteStudiesRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IDicomExplorerAutomationChannel : ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.IDicomExplorerAutomation, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class DicomExplorerAutomationClient : System.ServiceModel.ClientBase<ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.IDicomExplorerAutomation>, ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.IDicomExplorerAutomation
    {
        
        public DicomExplorerAutomationClient()
        {
        }
        
        public DicomExplorerAutomationClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public DicomExplorerAutomationClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public DicomExplorerAutomationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public DicomExplorerAutomationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchLocalStudiesResult SearchLocalStudies(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchLocalStudiesRequest request)
        {
            return base.Channel.SearchLocalStudies(request);
        }
        
        public ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchRemoteStudiesResult SearchRemoteStudies(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.DicomExplorerAutomation.SearchRemoteStudiesRequest request)
        {
            return base.Channel.SearchRemoteStudies(request);
        }
    }
}
