﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftwareEng {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class promptStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal promptStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SoftwareEng.promptStrings", typeof(promptStrings).Assembly);
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
        ///   Looks up a localized string similar to Jpeg images|*.jpg;*.jpeg;*.jpe;*.jfif;.
        /// </summary>
        internal static string addFileDialogueFilter {
            get {
                return ResourceManager.GetString("addFileDialogueFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^[\w\d][\w\d ]{0,31}$.
        /// </summary>
        internal static string albumValidationRegex {
            get {
                return ResourceManager.GetString("albumValidationRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^[\w\d ]{0,140}$.
        /// </summary>
        internal static string captionValidationRegex {
            get {
                return ResourceManager.GetString("captionValidationRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Images were skipped because they are already in this album. .
        /// </summary>
        internal static string importSkippedMessage {
            get {
                return ResourceManager.GetString("importSkippedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Images added to this album..
        /// </summary>
        internal static string importSuccessMessage {
            get {
                return ResourceManager.GetString("importSuccessMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose name:.
        /// </summary>
        internal static string newAlbumNameLabel {
            get {
                return ResourceManager.GetString("newAlbumNameLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose caption:.
        /// </summary>
        internal static string newImageCommentLabel {
            get {
                return ResourceManager.GetString("newImageCommentLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose name:.
        /// </summary>
        internal static string newImageNameLabel {
            get {
                return ResourceManager.GetString("newImageNameLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^[\w\d][\w\d ]{0,31}$.
        /// </summary>
        internal static string photoValidationRegex {
            get {
                return ResourceManager.GetString("photoValidationRegex", resourceCulture);
            }
        }
    }
}
