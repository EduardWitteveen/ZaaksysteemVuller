﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GenericUTurn.Properties {
    
    
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
        [global::System.Configuration.DefaultSettingValueAttribute("eduard@witteveen-automatisering.nl")]
        public string EmailAfzender {
            get {
                return ((string)(this["EmailAfzender"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int MaximumWorkQueue {
            get {
                return ((int)(this["MaximumWorkQueue"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("template\\substitutor.xml")]
        public string Substitutor {
            get {
                return ((string)(this["Substitutor"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("template\\namespaces.xml")]
        public string Namespaces {
            get {
                return ((string)(this["Namespaces"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("template\\GenereerZaakIdentificatie.xml")]
        public string TemplateGenereerZaakIdentificatie {
            get {
                return ((string)(this["TemplateGenereerZaakIdentificatie"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("template\\UpdateZaak.xml")]
        public string TemplateUpdateZaak {
            get {
                return ((string)(this["TemplateUpdateZaak"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("koppeling")]
        public string Koppelingen {
            get {
                return ((string)(this["Koppelingen"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("template\\CreeerZaak.xml")]
        public string TemplateCreeerZaak {
            get {
                return ((string)(this["TemplateCreeerZaak"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://zaaksysteem.local/ZaakDocumentServices/VrijBerichtService.svc")]
        public string StandaardZaakDocumentServicesVrijBerichtService {
            get {
                return ((string)(this["StandaardZaakDocumentServicesVrijBerichtService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://zaaksysteem.local/ZaakDocumentServices/OntvangAsynchroonService.svc")]
        public string StandaardZaakDocumentServicesOntvangAsynchroonService {
            get {
                return ((string)(this["StandaardZaakDocumentServicesOntvangAsynchroonService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://zaaksysteem.local/ZaakDocumentServices/BeantwoordVraagService.svc")]
        public string StandaardZaakDocumentServicesBeantwoordVraagService {
            get {
                return ((string)(this["StandaardZaakDocumentServicesBeantwoordVraagService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("smtp")]
        public string EmailSmtp {
            get {
                return ((string)(this["EmailSmtp"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[FOUT] ZaaksysteemVuller: fout betreffende zaaktype")]
        public string EmailTitel {
            get {
                return ((string)(this["EmailTitel"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("System.Data.OleDb")]
        public string UTurnDatabaseProvider {
            get {
                return ((string)(this["UTurnDatabaseProvider"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=data\\uturn.accdb")]
        public string UTurnDatabaseConnection {
            get {
                return ((string)(this["UTurnDatabaseConnection"]));
            }
        }
    }
}
