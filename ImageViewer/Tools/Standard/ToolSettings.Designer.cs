﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Tools.Standard {
    
    
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
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowNonCTModPixelValue {
            get {
                return ((bool)(this["ShowNonCTModPixelValue"]));
            }
            set {
                this["ShowNonCTModPixelValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowCTRawPixelValue {
            get {
                return ((bool)(this["ShowCTRawPixelValue"]));
            }
            set {
                this["ShowCTRawPixelValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowVOIPixelValue {
            get {
                return ((bool)(this["ShowVOIPixelValue"]));
            }
            set {
                this["ShowVOIPixelValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("TextCallout")]
        public string TextCalloutMode {
            get {
                return ((string)(this["TextCalloutMode"]));
            }
            set {
                this["TextCalloutMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public float MagnificationFactor {
            get {
                return ((float)(this["MagnificationFactor"]));
            }
            set {
                this["MagnificationFactor"] = value;
            }
        }
    }
}
