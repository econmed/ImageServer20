﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Configuration {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class ToolSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ToolSettings defaultInstance = ((ToolSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ToolSettings())));
        
        public static ToolSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Xml.XmlDocument ActionFilterRulesXml {
            get {
                return ((global::System.Xml.XmlDocument)(this["ActionFilterRulesXml"]));
            }
            set {
                this["ActionFilterRulesXml"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ApplyActionFilterRules {
            get {
                return ((bool)(this["ApplyActionFilterRules"]));
            }
            set {
                this["ApplyActionFilterRules"] = value;
            }
        }
    }
}