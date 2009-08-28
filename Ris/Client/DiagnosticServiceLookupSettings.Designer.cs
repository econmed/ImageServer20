﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Client {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class DiagnosticServiceLookupSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static DiagnosticServiceLookupSettings defaultInstance = ((DiagnosticServiceLookupSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new DiagnosticServiceLookupSettings())));
        
        public static DiagnosticServiceLookupSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// The minimum length of query string required to obtain suggestions
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("The minimum length of query string required to obtain suggestions")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int MinQueryStringLength {
            get {
                return ((int)(this["MinQueryStringLength"]));
            }
        }
        
        /// <summary>
        /// The maximum number of suggestions that a given query string can return.  A query that would return more suggestions will not return any.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("The maximum number of suggestions that a given query string can return.  A query " +
            "that would return more suggestions will not return any.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("200")]
        public int QuerySpecificityThreshold {
            get {
                return ((int)(this["QuerySpecificityThreshold"]));
            }
        }
    }
}
