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
    internal sealed partial class CannedTextEditorComponentSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static CannedTextEditorComponentSettings defaultInstance = ((CannedTextEditorComponentSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new CannedTextEditorComponentSettings())));
        
        public static CannedTextEditorComponentSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// Enable validation of characters in the canned text name.  If true, the name can only contain space and alphabets.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Enable validation of characters in the canned text name.  If true, the name can o" +
            "nly contain space and alphabets.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool RestrictCannedTextNameToAlphaChars {
            get {
                return ((bool)(this["RestrictCannedTextNameToAlphaChars"]));
            }
        }
    }
}
