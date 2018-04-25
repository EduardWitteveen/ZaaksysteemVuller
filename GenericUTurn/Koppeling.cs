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
        private Output logging = null;

        public Koppeling(POCO.Session uturn, ServiceClient.ServiceClient client, Xml.Substitutor substitutor, Xml.Namespaces namespaces)
        {
            // TODO: Complete member initialization
            this.uturn = uturn;
            this.client = client;
            this.substitutor = substitutor;
            this.namespaces = namespaces;

            logging = new Output(this);
        }
        internal int Synchronize(Zaaktype zaaktype, string sql, int waittime)
        {
            
            logging.Info("Starting:" + zaaktype.Code + " (" + zaaktype.Description + ")");
            var changes = 0;
            var backoffice = new POCO.Session(zaaktype.Provider, zaaktype.Connection, zaaktype.Code);
            zaaktype.logging.Info("Executing query....");
            backoffice.Open();
            var backofficezaken = backoffice.GetZaken(zaaktype.Code, sql);
            backoffice.Close();
            zaaktype.logging.Info("Found #" + backofficezaken.Count + " records....");
            uturn.Open();
            var processed = new Dictionary<string, string>();

            foreach (var backofficezaak in backofficezaken)
            {
                // some validations:
                //  input should always be strict, otherwise garbage in, garbage out!
                if (backofficezaak.ZaaktypeCode != zaaktype.Code) throw new Exception("Backoffice validation failed for procesid #" + backofficezaak.Procesid + " ERROR: zaaktype code did not match, backoffice:" + backofficezaak.ZaaktypeCode +  " vs. " + zaaktype.Code);
                if (!zaaktype.Statusus.Contains(backofficezaak.ZaakstatusCode)) throw new Exception("Backoffice validation failed for procesid #" + backofficezaak.Procesid + " ERROR: status was not defined in the xml, backoffice result:" + backofficezaak.ZaakstatusCode);
                if (backofficezaak.ResultaatOmschrijving != null && backofficezaak.ResultaatOmschrijving != "" && !zaaktype.Results.Contains(backofficezaak.ResultaatOmschrijving)) throw new Exception("Backoffice validation failed for procesid #" + backofficezaak.Procesid + " ERROR: result was not defined in the xml, backoffice result:" + backofficezaak.ResultaatOmschrijving);

                var uturnzaak = uturn.GetZaak(zaaktype.Code, backofficezaak.Procesid);
                // does this zaaak already exist?
                bool fallbackscenario = false;
                if (uturnzaak != null && uturnzaak.Dirty(backofficezaak))
                {
                    if (processed.ContainsKey(backofficezaak.Procesid))
                    {
                        throw new Exception("zaakcode:" + backofficezaak.ZaaktypeCode + " zaak:" + backofficezaak.ZaaktypeOmschrijving + " already processed procesid #" + backofficezaak.Procesid + " (zaakid#" + processed[backofficezaak.Procesid] + ")");                        
                    }

                    // check on the zaakid
                    try { 
                        POCO.UTurnZaak sendzaak = VerzendUpdateZaak(client, substitutor, namespaces, uturnzaak, backofficezaak, waittime);
                        uturn.Update(sendzaak);
                        zaaktype.logging.Info("[" + changes + "] Updated zaak #" + sendzaak.ZaakId + "(" + sendzaak.Procesid + ") van type:" + sendzaak.ZaaktypeCode + "(" + sendzaak.ZaaktypeOmschrijving + ") : " + sendzaak.ZaakOmschrijving);
                        processed.Add(sendzaak.Procesid, sendzaak.ZaakId);
                        if (zaaktype.Stoppers.Contains(sendzaak.ZaakstatusCode))
                        {
                            zaaktype.logging.Warn("PROCESERROR: UPDATE zaak #" + sendzaak.ZaakId + "(procesid:" + sendzaak.Procesid + ") van type:" + sendzaak.ZaaktypeCode + " STATUS:" + sendzaak.ZaakstatusCode + " is the end status!");
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
                            zaaktype.logging.Warn("ZAAK NOT FOUND IN ZAAKSYSTEEM, ADDING AGAIN: zaak #" + uturnzaak.ZaakId + "(" + uturnzaak.Procesid + ") van type:" + uturnzaak.ZaaktypeCode + "(" + uturnzaak.ZaaktypeOmschrijving + ") : " + uturnzaak.ZaakOmschrijving);
                            uturnzaak = null;
                            fallbackscenario = true;
                        }
                        else throw sce;
                    }
                }
                if (uturnzaak == null)
                {
                    POCO.BackofficeZaak sendzaak = VerzendNieuweZaak(client, substitutor, namespaces, backofficezaak, waittime);
                    if (fallbackscenario)
                    {
                        uturn.Update(sendzaak);
                        zaaktype.logging.Info("[" + changes + "][fallback] Updated zaak #" + sendzaak.ZaakId + " van type:" + sendzaak.ZaaktypeCode);
                        processed.Add(sendzaak.Procesid, sendzaak.ZaakId);
                    }
                    else
                    {
                        uturn.Add(sendzaak);
                        zaaktype.logging.Info("[" + changes + "] Added zaak #" + sendzaak.ZaakId + "(" + sendzaak.Procesid + ") van type:" + sendzaak.ZaaktypeCode + "(" + sendzaak.ZaaktypeOmschrijving + ") : " + sendzaak.ZaakOmschrijving);
                        processed.Add(sendzaak.Procesid, sendzaak.ZaakId);
                    }
                    if (zaaktype.Starters.Contains(sendzaak.ZaakstatusCode))
                    {
                        zaaktype.logging.Warn("PROCESERROR: Proces didnt start with the first status for zaak #" + backofficezaak.ZaakId + "(procesid:" + backofficezaak.Procesid + ") van type:" + sendzaak.ZaaktypeCode + " STATUS:" + sendzaak.ZaakstatusCode + " cannot be the first status!");
                    }
                    changes++;
                }
                else
                {
                    // Console.WriteLine("Skipping zaak #" + uturnzaak.Zaakid + " van type:" + uturnzaak.ZaaktypeCode + " (NO CHANGES)");
                    // System.Diagnostics.Debug.WriteLine("Skipping zaak #" + uturnzaak.ZaakId + "(" + uturnzaak.Procesid + ") van type:" + uturnzaak.ZaaktypeCode + "(" + uturnzaak.ZaaktypeOmschrijving + ") : " + uturnzaak.ZaakOmschrijving);
                }
                //uturnzaak = null;

                if(processed.Count >= Properties.Settings.Default.MaximumWorkQueue)
                {
                    zaaktype.logging.Info("MaximumWorkQueue for Zaaktype:" + backofficezaak.ZaaktypeCode + "(" + backofficezaak.ZaaktypeOmschrijving + ") reached, count was:" + Properties.Settings.Default.MaximumWorkQueue);
                    break;
                }
            }
            uturn.Close();
            return changes;                
        }
        private POCO.BackofficeZaak VerzendNieuweZaak(ServiceClient.ServiceClient client, Xml.Substitutor substitutor, Xml.Namespaces namespaces, POCO.BackofficeZaak backofficezaak, int waittime)
        {
            // bepaal de variabelen
            var variables = new Dictionary<string, string>();
            //variables.Add("${defined-timestamp}", DateTime.Now.ToString("yyyyMMddhhmmssfff"));
            //moet zijn: 20160218 ipv. 29-11-2016 14:39:48
            //variables.Add("${defined-timestamp}", DateTime.Now.ToString("yyyyMMdd"));
            POCO.Session.Serialize(backofficezaak, variables, "eerste-");

            if (backofficezaak.ZaakId == null || backofficezaak.ZaakId.Length == 0) { 
                // bepaal een zaakid
                var GenereerZaakIdentificatieRequest = new ServiceClient.ServiceClientRequest("http://www.egem.nl/StUF/sector/zkn/0310/genereerZaakIdentificatie_Di02", new System.IO.FileInfo(Properties.Settings.Default.TemplateGenereerZaakIdentificatie), substitutor, namespaces);
                var GenereerZaakIdentificatieResponse = client.Call(backofficezaak.ZaaktypeCode + ":" + backofficezaak.Procesid, new Uri(Properties.Settings.Default.StandaardZaakDocumentServicesVrijBerichtService), GenereerZaakIdentificatieRequest, waittime, variables);
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
            var CreeerZaakResponse = client.Call(backofficezaak.ZaaktypeCode + ":" + backofficezaak.Procesid, new Uri(Properties.Settings.Default.StandaardZaakDocumentServicesOntvangAsynchroonService), CreeerZaakRequest, waittime, variables);

            return backofficezaak;
        }

        private POCO.UTurnZaak VerzendUpdateZaak(ServiceClient.ServiceClient client, Xml.Substitutor substitutor, Xml.Namespaces namespaces, POCO.UTurnZaak uturnzaak, POCO.Zaak backofficezaak, int waittime)
        {

            var variables = new Dictionary<string, string>();
            variables.Add("${defined-timestamp}", DateTime.Now.ToString("yyyyMMddhhmmssfff"));
            // old version has been serialized
            POCO.Session.Serialize(uturnzaak, variables, "eerste-");
            
            // now we can overwrite with the new version
            uturnzaak.Update(backofficezaak);
            POCO.Session.Serialize(uturnzaak, variables, "tweede-");

            // update de zaak
            var UpdateZaakRequest = new ServiceClient.ServiceClientRequest("http://www.egem.nl/StUF/sector/zkn/0310/updateZaak_Lk01", new System.IO.FileInfo(Properties.Settings.Default.TemplateUpdateZaak), substitutor, namespaces);
            var UpdateZaakResponse = client.Call(uturnzaak.ZaaktypeCode + ":" + uturnzaak.Procesid, new Uri(Properties.Settings.Default.StandaardZaakDocumentServicesOntvangAsynchroonService), UpdateZaakRequest, waittime, variables);

            return uturnzaak;
        }
    }
}
