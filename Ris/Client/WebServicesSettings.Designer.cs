﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Client {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class WebServicesSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static WebServicesSettings defaultInstance = ((WebServicesSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new WebServicesSettings())));
        
        public static WebServicesSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// Specifies the URL on which the application services are hosted.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies the URL on which the application services are hosted.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:8000/")]
        public string ApplicationServicesBaseUrl {
            get {
                return ((string)(this["ApplicationServicesBaseUrl"]));
            }
        }
        
        /// <summary>
        /// Specifies the name of the service configuration class.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies the name of the service configuration class.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ClearCanvas.Enterprise.Common.ServiceConfiguration.Client.WSHttpConfiguration, Cl" +
            "earCanvas.Enterprise.Common")]
        public string ConfigurationClass {
            get {
                return ((string)(this["ConfigurationClass"]));
            }
        }
        
        /// <summary>
        /// Specifies the maximum size of received messages in bytes.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies the maximum size of received messages in bytes.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2000000")]
        public int MaxReceivedMessageSize {
            get {
                return ((int)(this["MaxReceivedMessageSize"]));
            }
        }
    }
}
