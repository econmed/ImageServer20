﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Desktop.Explorer {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class ExplorerSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ExplorerSettings defaultInstance = ((ExplorerSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ExplorerSettings())));
        
        public static ExplorerSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool LaunchAsShelf {
            get {
                return ((bool)(this["LaunchAsShelf"]));
            }
            set {
                this["LaunchAsShelf"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool LaunchAtStartup {
            get {
                return ((bool)(this["LaunchAtStartup"]));
            }
            set {
                this["LaunchAtStartup"] = value;
            }
        }
    }
}
