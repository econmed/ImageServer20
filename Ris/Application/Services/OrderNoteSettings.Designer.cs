﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Application.Services {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class OrderNoteSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static OrderNoteSettings defaultInstance = ((OrderNoteSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new OrderNoteSettings())));
        
        public static OrderNoteSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// For Order Notebox queries, controls number of order-note items per page
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("For Order Notebox queries, controls number of order-note items per page")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int ItemsPerPage {
            get {
                return ((int)(this["ItemsPerPage"]));
            }
        }
    }
}
