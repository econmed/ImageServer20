﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class StandardModalitySettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static StandardModalitySettings defaultInstance = ((StandardModalitySettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new StandardModalitySettings())));
        
        public static StandardModalitySettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CR, CT, DX, ES, MG, MR, NM, OT, PT, RF, SC, US, XA")]
        public string Modalities {
            get {
                return ((string)(this["Modalities"]));
            }
        }
    }
}
