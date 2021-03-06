﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Shreds {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ClearCanvas.Ris.Shreds.SR", typeof(SR).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This shred hosts the ImageAvailability Processor which is responsible for querying the DICOM server and update the image availability of each procedure.
        /// </summary>
        internal static string ImageAvailabilityShredDescription {
            get {
                return ResourceManager.GetString("ImageAvailabilityShredDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image Availability.
        /// </summary>
        internal static string ImageAvailabilityShredName {
            get {
                return ResourceManager.GetString("ImageAvailabilityShredName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection failed ({0})..
        /// </summary>
        internal static string MessageFormatDicomConnectionFailed {
            get {
                return ResourceManager.GetString("MessageFormatDicomConnectionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection timeout expired ({0})..
        /// </summary>
        internal static string MessageFormatDicomConnectTimeoutExpired {
            get {
                return ResourceManager.GetString("MessageFormatDicomConnectTimeoutExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The query operation failed ({0})..
        /// </summary>
        internal static string MessageFormatDicomQueryOperationFailed {
            get {
                return ResourceManager.GetString("MessageFormatDicomQueryOperationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The remote server cancelled the query ({0})..
        /// </summary>
        internal static string MessageFormatDicomRemoteServerCancelledFind {
            get {
                return ResourceManager.GetString("MessageFormatDicomRemoteServerCancelledFind", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This shred hosts the Publication Processor which is responsible for processing scheduled publication steps.
        /// </summary>
        internal static string PublicationShredDescription {
            get {
                return ResourceManager.GetString("PublicationShredDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Publication.
        /// </summary>
        internal static string PublicationShredName {
            get {
                return ResourceManager.GetString("PublicationShredName", resourceCulture);
            }
        }
    }
}
