﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class PresetVoiLutSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static PresetVoiLutSettings defaultInstance = ((PresetVoiLutSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new PresetVoiLutSettings())));
        
        public static PresetVoiLutSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DefaultPresetVoiLutConfiguration.xml")]
        public string SettingsXml {
            get {
                return ((string)(this["SettingsXml"]));
            }
            set {
                this["SettingsXml"] = value;
            }
        }
    }
}
