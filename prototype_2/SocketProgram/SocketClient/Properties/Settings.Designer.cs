﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocketClient.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=172.20.1.20;Initial Catalog=ProjectDb_Bram;Persist Security Info=True" +
            ";User ID=Student.Voorbeeld;Password=Student+2019")]
        public string ProjectDb_BramConnectionString {
            get {
                return ((string)(this["ProjectDb_BramConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\lobbe\\OneDrive\\Docum" +
            "ents\\ErasmusHogeschool\\Project\\prototype_2\\SocketProgram\\SocketServer\\ProjectDb_" +
            "BramLocal.mdf;Integrated Security=True;Connect Timeout=30")]
        public string ProjectDb_BramLocalConnectionString {
            get {
                return ((string)(this["ProjectDb_BramLocalConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\lobbe\\OneDrive\\Docum" +
            "ents\\ProjectDb_Bram.mdf;Integrated Security=True;Connect Timeout=30")]
        public string ProjectDb_BramConnectionStringLocal {
            get {
                return ((string)(this["ProjectDb_BramConnectionStringLocal"]));
            }
        }
    }
}
