﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftwareEng.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Image")]
        public string DefaultImageName {
            get {
                return ((string)(this["DefaultImageName"]));
            }
            set {
                this["DefaultImageName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("albums.xml")]
        public string AlbumXMLFile {
            get {
                return ((string)(this["AlbumXMLFile"]));
            }
            set {
                this["AlbumXMLFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("photos.xml")]
        public string PhotoXMLFile {
            get {
                return ((string)(this["PhotoXMLFile"]));
            }
            set {
                this["PhotoXMLFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("photo library")]
        public string PhotoLibraryName {
            get {
                return ((string)(this["PhotoLibraryName"]));
            }
            set {
                this["PhotoLibraryName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Backup Recover Album")]
        public string PhotoLibraryBackupName {
            get {
                return ((string)(this["PhotoLibraryBackupName"]));
            }
            set {
                this["PhotoLibraryBackupName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("thumbs_db")]
        public string PhotoLibraryThumbsDir {
            get {
                return ((string)(this["PhotoLibraryThumbsDir"]));
            }
            set {
                this["PhotoLibraryThumbsDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("database")]
        public string XMLRootElement {
            get {
                return ((string)(this["XMLRootElement"]));
            }
            set {
                this["XMLRootElement"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PhotoBomber Studios")]
        public string OrgName {
            get {
                return ((string)(this["OrgName"]));
            }
            set {
                this["OrgName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120")]
        public int smThumbSize {
            get {
                return ((int)(this["smThumbSize"]));
            }
            set {
                this["smThumbSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("240")]
        public int medThumbSize {
            get {
                return ((int)(this["medThumbSize"]));
            }
            set {
                this["medThumbSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("480")]
        public int lrgThumbSize {
            get {
                return ((int)(this["lrgThumbSize"]));
            }
            set {
                this["lrgThumbSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sm")]
        public string smThumbDir {
            get {
                return ((string)(this["smThumbDir"]));
            }
            set {
                this["smThumbDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("med")]
        public string medThumbDir {
            get {
                return ((string)(this["medThumbDir"]));
            }
            set {
                this["medThumbDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("lrg")]
        public string lrgThumbDir {
            get {
                return ((string)(this["lrgThumbDir"]));
            }
            set {
                this["lrgThumbDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("140")]
        public int CaptionMaxLength {
            get {
                return ((int)(this["CaptionMaxLength"]));
            }
            set {
                this["CaptionMaxLength"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PhotoBomber - My Photo Library!")]
        public string AppTitleBarText {
            get {
                return ((string)(this["AppTitleBarText"]));
            }
            set {
                this["AppTitleBarText"] = value;
            }
        }
    }
}
