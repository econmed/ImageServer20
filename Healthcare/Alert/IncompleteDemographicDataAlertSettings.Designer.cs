﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Healthcare.Alert {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class IncompleteDemographicDataAlertSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static IncompleteDemographicDataAlertSettings defaultInstance = ((IncompleteDemographicDataAlertSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new IncompleteDemographicDataAlertSettings())));
        
        public static IncompleteDemographicDataAlertSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("IncompleteDemographicDataAlert.val.xml")]
        public string ValidationRules {
            get {
                return ((string)(this["ValidationRules"]));
            }
        }
    }
}
