﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3082
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Client {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class FolderExplorerComponentSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static FolderExplorerComponentSettings defaultInstance = ((FolderExplorerComponentSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new FolderExplorerComponentSettings())));
        
        public static FolderExplorerComponentSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// XML document that describes the ordering of folders in folder systems.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("XML document that describes the ordering of folders in folder systems.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("FolderExplorerComponentSettings.xml")]
        public string FolderPathXml {
            get {
                return ((string)(this["FolderPathXml"]));
            }
            set {
                this["FolderPathXml"] = value;
            }
        }
    }
}
