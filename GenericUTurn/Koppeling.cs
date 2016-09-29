using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn
{
    public class Koppeling
    {
        private POCO.Session uturn;
        private ServiceClient.ServiceClient client;
        private Xml.Substitutor substitutor;
        private Xml.Namespaces namespaces;

        public Koppeling(POCO.Session uturn, ServiceClient.ServiceClient client, Xml.Substitutor substitutor, Xml.Namespaces namespaces)
        {
            // TODO: Complete member initialization
            this.uturn = uturn;
            this.client = client;
            this.substitutor = substitutor;
            this.namespaces = namespaces;
        }
        internal int Synchronize(Zaaktype zaaktype, string sql)
        {
            Output.Info("Starting:" + zaaktype.Code);
            var changes = 0;
            var backoffice = new POCO.Session(zaaktype.Provider, zaaktype.Connection, zaaktype.Code);
            Output.Info("Executing query....");
            backoffice.Open();
            var backofficezaken = backoffice.GetZaken(zaaktype.Code, sql);
            backoffice.Close();
            Output.Info("Found #" + backofficezaken.Count + " records....");
            uturn.Open();
            foreach (var backofficezaak in backofficezaken)
            {
                // some validations:
                //  input should always be strict, otherwise garbage in, garbage out!
                if (backofficezaak.ZaaktypeCode != zaaktype.Code) throw new Exception("Backoffice validation failed for procesid #" + backofficezaak.Procesid + " ERROR: zaaktype code did not match, backoffice:" + backofficezaak.ZaaktypeCode +  " vs. " + zaaktype.Code);
                if (!zaaktype.Statusus.Contains(backofficezaak.ZaakstatusCode)) throw new Exception("Backoffice validation failed for procesid #" + backofficezaak.Procesid + " ERROR: status was not defined in the xml, backoffice result:" + backofficezaak.ZaakstatusCode);
                if (backofficezaak.ResultaatOmschrijving != null && !zaaktype.Results.Contains(backofficezaak.ResultaatOmschrijving)) throw new Exception("Backoffice validation failed for procesid #" + backofficezaak.Procesid + " ERROR: result was not defined in the xml, backoffice result:" + backofficezaak.ResultaatOmschrijving);

                var uturnzaak = uturn.GetZaak(zaaktype.Code, backofficezaak.Procesid);
                // does this zaaak already exist?

                bool fallbackscenario = false;
                if (uturnzaak != null && uturnzaak.Dirty(backofficezaak))
                {
                    try { 
                        uturnzaak = VerzendUpdateZaak(client, substitutor, namespaces, uturnzaak, backofficezaak);
                        uturn.Update(uturnzaak);
                        Output.Info("Updated zaak #" + uturnzaak.ZaakId + " van type:" + uturnzaak.ZaaktypeCode);
                        if (zaaktype.Stoppers.Contains(uturnzaak.ZaakstatusCode))
                        {
                            Output.Warn("PROCESERROR: UPDATE zaak #" + uturnzaak.ZaakId + "(procesid:" + uturnzaak.Procesid + ") van type:" + uturnzaak.ZaaktypeCode + " STATUS:" + uturnzaak.ZaakstatusCode + " is the end status!");
                        }
                        changes++;
                    }
                    catch (ServiceClient.ServiceClientException sce)
                    {
                        // filter op de bestond al dingen!
                        if (sce.HttpStatusCode == 500 && sce.Message.Contains("<StUF:details>De zaak is niet gevonden."))
                        {
                            Console.WriteLine("ERROR: zaak niet gevonden:" + sce.Message);
                            // gebruik ons eerder vastgestelde zaakid
                            // en we gaan hem nogmaals toeveogen!
                            backofficezaak.ZaakId = uturnzaak.ZaakId;
                            Output.Warn("ZAAK NOT FOUND IN ZAAKSYSTEEM: UPDATE zaak #" + uturnzaak.ZaakId + "(procesid:" + uturnzaak.Procesid + ") van type:" + uturnzaak.ZaaktypeCode + ", adding again!");
                            uturnzaak = null;
                            fallbackscenario = true;
                        }
                        else throw sce;
                    }
                }
                if (uturnzaak == null)
                {
                    uturnzaak = VerzendNieuweZaak(client, substitutor, namespaces, backofficezaak);
                    if (fallbackscenario) uturn.Update(uturnzaak);
                    else uturn.Add(uturnzaak);
                    Output.Info("Added zaak #" + backofficezaak.ZaakId + " van type:" + backofficezaak.ZaaktypeCode);
                    if (zaaktype.Starters.Contains(backofficezaak.ZaakstatusCode))
                    {
                        Output.Warn("PROCESERROR: Proces didnt start with the first status for zaak #" + backofficezaak.ZaakId + "(procesid:" + backofficezaak.Procesid + ") van type:" + backofficezaak.ZaaktypeCode + " STATUS:" + backofficezaak.ZaakstatusCode + " cannot be the first status!");
                    }
                    changes++;

                }
                else
                {
                    // Console.WriteLine("Skipping zaak #" + uturnzaak.Zaakid + " van type:" + uturnzaak.ZaaktypeCode + " (NO CHANGES)");
                }
                //uturnzaak = null;
            }
            uturn.Close();
            return changes;                
        }
        private POCO.Zaak VerzendNieuweZaak(ServiceClient.ServiceClient client, Xml.Substitutor substitutor, Xml.Namespaces namespaces, POCO.Zaak backofficezaak)
        {
            // bepaal de variabelen
            var variables = new Dictionary<string, string>();
            variables.Add("${defined-timestamp}", DateTime.Now.ToString("yyyyMMddhhmmssfff"));
            POCO.Session.Serialize(backofficezaak, variables, "eerste-");

            if (backofficezaak.ZaakId == null) { 
                // bepaal een zaakid
                var GenereerZaakIdentificatieRequest = new ServiceClient.ServiceClientRequest("http://www.egem.nl/StUF/sector/zkn/0310/genereerZaakIdentificatie_Di02", new System.IO.FileInfo(Properties.Settings.Default.TemplateGenereerZaakIdentificatie), substitutor, namespaces);
                var GenereerZaakIdentificatieResponse = client.Call(backofficezaak.ZaaktypeCode + ":" + backofficezaak.Procesid, new Uri(Properties.Settings.Default.StandaardZaakDocumentServicesVrijBerichtService), GenereerZaakIdentificatieRequest, variables);
                // zet het zaakid en vul de variabelen opnieuw
                var content = GenereerZaakIdentificatieResponse.Content;
                backofficezaak.ZaakId = content.GetValue(namespaces, "//ZKN:zaak/ZKN:identificatie");
                POCO.Session.Serialize(backofficezaak, variables, "eerste-");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("skipping retrieval zaakid, we already have the id:" + backofficezaak.ZaakId);
            }
            variables["${defined-timestamp}"] = DateTime.Now.ToString("yyyyMMddhhmmssfff");

            // maak de zaak aan 
            var CreeerZaakRequest = new ServiceClient.ServiceClientRequest("http://www.egem.nl/StUF/sector/zkn/0310/creeerZaak_Lk01", new System.IO.FileInfo(Properties.Settings.Default.TemplateCreeerZaak), substitutor, namespaces);
            var CreeerZaakResponse = client.Call(backofficezaak.ZaaktypeCode + ":" + backofficezaak.Procesid, new Uri(Properties.Settings.Default.StandaardZaakDocumentServicesOntvangAsynchroonService), CreeerZaakRequest, variables);

            return backofficezaak;
        }

        private POCO.Zaak VerzendUpdateZaak(ServiceClient.ServiceClient client, Xml.Substitutor substitutor, Xml.Namespaces namespaces, POCO.Zaak uturnzaak, POCO.Zaak backofficezaak)
        {

            var variables = new Dictionary<string, string>();
            variables.Add("${defined-timestamp}", DateTime.Now.ToString("yyyyMMddhhmmssfff"));
            POCO.Session.Serialize(uturnzaak, variables, "eerste-");
            // zelfde procesid, dan hetzelfde zaakid
            backofficezaak.ZaakId = uturnzaak.ZaakId;
            POCO.Session.Serialize(backofficezaak, variables, "tweede-");

            // update de zaak
            var UpdateZaakRequest = new ServiceClient.ServiceClientRequest("http://www.egem.nl/StUF/sector/zkn/0310/updateZaak_Lk01", new System.IO.FileInfo(Properties.Settings.Default.TemplateUpdateZaak), substitutor, namespaces);
            var UpdateZaakResponse = client.Call(backofficezaak.ZaaktypeCode + ":" + backofficezaak.Procesid, new Uri(Properties.Settings.Default.StandaardZaakDocumentServicesOntvangAsynchroonService), UpdateZaakRequest, variables);

            return backofficezaak;
        }
    }
}
