﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Healthcare.Alerts {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class AlertsSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static AlertsSettings defaultInstance = ((AlertsSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new AlertsSettings())));
        
        public static AlertsSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("en")]
        [global::System.Configuration.SettingsDescriptionAttribute("A comma separated list of spoken language codes used to determine whether the language alert is generated.")]
        public string DefaultLanguages
        {
            get {
                return ((string)(this["DefaultLanguages"]));
            }
        }
    }
}
