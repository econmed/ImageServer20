﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ClearCanvas.ImageViewer {
    
    
    [CompilerGenerated()]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class StandardModalitySettings : ApplicationSettingsBase {
        
        private static StandardModalitySettings defaultInstance = ((StandardModalitySettings)(Synchronized(new StandardModalitySettings())));
        
        public static StandardModalitySettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [ApplicationScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("CR, CT, DX, ES, MG, MR, NM, OT, PT, RF, SC, US, XA")]
        public string Modalities {
            get {
                return ((string)(this["Modalities"]));
            }
        }
    }
}
