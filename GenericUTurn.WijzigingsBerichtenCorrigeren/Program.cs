using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace GenericUTurn.WijzigingsBerichtenCorrigeren
{
    class Program
    {
        // productie
        const string path = @"C:\Users\e.witteveen\ownCloud\Midoffice\exceptions\handmatige fouten\Entity Centric.Publieksdiensten.Zakenmagazijn.DataAccess.EF.Entities.Zaak";
        /*
        const string vraagurl = @"http://iszfmid02.iszf.local:53803/PSWFZKS-ZaakDocumentServices/BeantwoordVraagService.svc";
        const string ontvangurl = @"http://iszfmid02.iszf.local:53803/PSWFZKS-ZaakDocumentServices/OntvangAsynchroonService.svc";
        http://localhost:8280/services/swf/prod/stuf/zkn0310/zds/beantwoordVraag
        http://localhost:8280/services/swf/prod/stuf/zkn0310/zds/ontvangAsynchroon
        http://localhost:8280/services//swf/prod/stuf/zkn0310/zds/vrijBericht
        */
        const string vraagurl = @"http://localhost:8280/services/swf/prod/stuf/zkn0310/zds/beantwoordVraag2";
        const string ontvangurl = @"http://localhost:8280/services/swf/prod/stuf/zkn0310/zds/ontvangAsynchroon2";

        /*
        // test
        const string path = @"C:\Temp\fouten";
        const string vraagurl = @"http://iszfmid03.iszf.local:53803/TSWFZKS-ZaakDocumentServices/BeantwoordVraagService.svc";
        const string ontvangurl = @"http://iszfmid03.iszf.local:53803/TSWFZKS-ZaakDocumentServices/OntvangAsynchroonService.svc";       
        */
 
        static void Main(string[] args)
        {
            var workingdirectory = new DirectoryInfo(path);
            Console.WriteLine("working directory:" + workingdirectory.FullName);
            var gewijzigd = workingdirectory.CreateSubdirectory("gewijzigd");
            var anders = workingdirectory.CreateSubdirectory("anders");
            var dubbel = workingdirectory.CreateSubdirectory("dubbel");

            foreach (FileInfo file in workingdirectory.GetFiles().OrderBy(file => file.CreationTime))
            {
                Console.WriteLine("found file :" + file.FullName);

                if (!file.Extension.Equals(".xml"))
                {
                    Console.WriteLine("wrong extension for:" + file);
                    continue;
                }

                // reading as xml
                var document = new System.Xml.XmlDocument();
                document.Load(file.FullName);

                // verify that it is de goede type
                var application = document.SelectSingleNode("//Header/Application");
                if (application == null || application.Name != "Application" || !(application.InnerText == "Conductor.Koppelvlakken" || application.InnerText == "Conductor.System"))
                {
                    Console.WriteLine("wrong value for: //Header/Application ");
                    file.MoveTo(anders.FullName + @"\" + file);
                    continue;
                }
                var description = document.SelectSingleNode("//Header/Description");
                if (description == null || description.Name != "Description" || !description.InnerText.StartsWith("Code = StUF058"))
                {
                    Console.WriteLine("wrong value for: //Header/Description ");
                    file.MoveTo(anders.FullName + @"\" + file);
                    continue;
                }
                // foutbericht
                var messagedata = document.SelectSingleNode("//Messages/Message/MessageData");
                var any = messagedata.FirstChild;
                var foutbericht = any.FirstChild;

                // wijzigingbericht
                XmlNamespaceManager ns = new XmlNamespaceManager(document.NameTable);
                ns.AddNamespace("StUF", "http://www.egem.nl/StUF/StUF0301");
                var foutdetails = foutbericht.SelectSingleNode("//StUF:details", ns);
                Console.WriteLine("Trying to fix:" + foutdetails.InnerText);
                var wijzigingsbericht = foutbericht.SelectSingleNode("//StUF:detailsXML", ns).FirstChild;
                
                // Wat is ons zaakid?                
                ns.AddNamespace("ZKN", "http://www.egem.nl/StUF/sector/zkn/0310");
                // meerdere antwoorden, maar de eerste is wel goed genoeg
                var zaakid = wijzigingsbericht.SelectSingleNode("//ZKN:object[@StUF:entiteittype='ZAK']/ZKN:identificatie", ns).InnerText;

                // maar eens bevragen wat de stavaza volgens het zaaksysteem is,...
                var huidigezaak = HaalZaakObject(zaakid);
                var nieuwezaak = wijzigingsbericht.SelectNodes("//ZKN:object[@StUF:entiteittype='ZAK']", ns)[1];

                // verzend updatezaak
                File.WriteAllText(gewijzigd.FullName + @"\" + file + ".log" , VerzendZaakWijziging(huidigezaak, nieuwezaak));
                Console.WriteLine("[Zaak with id: " + zaakid + " is aangepast!]\n");

                // wanneer goed, dan verplaatsen
                if (!File.Exists(gewijzigd.FullName + @"\" + file))
                {
                    file.MoveTo(gewijzigd.FullName + @"\" + file);
                }
                else
                {
                    Console.WriteLine("[WARNING: Zaak with id: " + zaakid + " was ook al eerder aangepast!]\n");
                    file.MoveTo(dubbel.FullName + @"\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-" +  file);
                }
                

                // wacht maar eens even 10 seconden
                System.Threading.Thread.Sleep(10 * 1000);
            }
            Console.WriteLine("press [enter] to continue!");
            Console.ReadLine();
        }

        private static XmlNode HaalZaakObject(string zaakid)
        {
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(vraagurl);
            // http://www.egem.nl/StUF/sector/zkn/0310/geefZaakdetails_Lv01
            webRequest.Headers.Add("SOAPAction", "http://www.egem.nl/StUF/sector/zkn/0310/geefZaakdetails_Lv01");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            #region content
            var content = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
	<s:Body>
		<zakLv01 xmlns=""http://www.egem.nl/StUF/sector/zkn/0310"" xmlns:StUF=""http://www.egem.nl/StUF/StUF0301"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
			<stuurgegevens>
				<StUF:berichtcode>Lv01</StUF:berichtcode>
				<StUF:zender>
					<StUF:organisatie>1900</StUF:organisatie>
					<StUF:applicatie>GenericUTurn.WijzigingsBerichtenCorrigeren</StUF:applicatie>
				</StUF:zender>
				<StUF:ontvanger>
					<StUF:organisatie>1900</StUF:organisatie>
					<StUF:applicatie>CDR</StUF:applicatie>
				</StUF:ontvanger>
				<StUF:referentienummer>" + Guid.NewGuid().ToString() + @"</StUF:referentienummer>
				<StUF:tijdstipBericht>" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @"</StUF:tijdstipBericht>
				<StUF:entiteittype>ZAK</StUF:entiteittype>
			</stuurgegevens>
			<parameters>
				<StUF:sortering>0</StUF:sortering>
				<StUF:indicatorVervolgvraag>false</StUF:indicatorVervolgvraag>
			</parameters>
			<gelijk StUF:entiteittype=""ZAK"">
				<identificatie>" + zaakid +  @"</identificatie>
			</gelijk>
			<scope>
				<object StUF:entiteittype=""ZAK"">
					<identificatie xsi:nil=""true""/>
					<omschrijving xsi:nil=""true""/>
					<toelichting xsi:nil=""true""/>
					<kenmerk>
						<kenmerk xsi:nil=""true""/>
						<bron xsi:nil=""true""/>
					</kenmerk>
					<anderZaakObject>
						<omschrijving xsi:nil=""true""/>
						<aanduiding xsi:nil=""true""/>
						<lokatie xsi:nil=""true""/>
						<registratie xsi:nil=""true""/>
					</anderZaakObject>
					<resultaat>
						<omschrijving xsi:nil=""true""/>
						<toelichting xsi:nil=""true""/>
					</resultaat>
					<startdatum xsi:nil=""true""/>
					<registratiedatum xsi:nil=""true""/>
					<publicatiedatum xsi:nil=""true""/>
					<einddatumGepland xsi:nil=""true""/>
					<uiterlijkeEinddatum xsi:nil=""true""/>
					<einddatum xsi:nil=""true""/>
					<opschorting>
						<indicatie xsi:nil=""true""/>
						<reden xsi:nil=""true""/>
					</opschorting>
					<verlenging>
						<duur xsi:nil=""true""/>
						<reden xsi:nil=""true""/>
					</verlenging>
					<betalingsIndicatie xsi:nil=""true""/>
					<laatsteBetaaldatum xsi:nil=""true""/>
					<archiefnominatie xsi:nil=""true""/>
					<datumVernietigingDossier xsi:nil=""true""/>
					<zaakniveau xsi:nil=""true""/>
					<deelzakenIndicatie xsi:nil=""true""/>
					<isVan StUF:entiteittype=""ZAKZKT"">
						<gerelateerde StUF:entiteittype=""ZKT"">
							<omschrijving xsi:nil=""true""/>
							<code xsi:nil=""true""/>
						</gerelateerde>
					</isVan>
					<heeftBetrekkingOp StUF:entiteittype=""ZAKOBJ"">
						<gerelateerde/>
					</heeftBetrekkingOp>
					<heeftAlsBelanghebbende StUF:entiteittype=""ZAKBTRBLH"">
						<gerelateerde/>
					</heeftAlsBelanghebbende>
					<heeftAlsGemachtigde StUF:entiteittype=""ZAKBTRGMC"">
						<gerelateerde/>
					</heeftAlsGemachtigde>
					<heeftAlsInitiator StUF:entiteittype=""ZAKBTRINI"">
						<gerelateerde/>
					</heeftAlsInitiator>
					<heeftAlsUitvoerende StUF:entiteittype=""ZAKBTRUTV"">
						<gerelateerde/>
					</heeftAlsUitvoerende>
					<heeftAlsVerantwoordelijke StUF:entiteittype=""ZAKBTRVRA"">
						<gerelateerde/>
					</heeftAlsVerantwoordelijke>
					<heeftAlsOverigBetrokkene StUF:entiteittype=""ZAKBTROVR"">
						<gerelateerde/>
					</heeftAlsOverigBetrokkene>
				</object>
			</scope>
		</zakLv01>
	</s:Body>
</s:Envelope>";
#endregion
            string result;
            using (System.IO.Stream stream = webRequest.GetRequestStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(content);
                stream.Write(data, 0, data.Length);
            }
            // begin async call to web request.
            var asyncResult = webRequest.BeginGetResponse(null, null);
            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();
            // get the response from the completed web request.
            var response = (System.Net.HttpWebResponse)webRequest.EndGetResponse(asyncResult);
            // (int)response.StatusCode;

            using (System.IO.Stream stream = response.GetResponseStream())
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }
            var envelope = new XmlDocument();
            envelope.LoadXml(result);

            XmlNamespaceManager ns = new XmlNamespaceManager(envelope.NameTable);
            ns.AddNamespace("StUF", "http://www.egem.nl/StUF/StUF0301");
            ns.AddNamespace("ZKN", "http://www.egem.nl/StUF/sector/zkn/0310");

            var zakla01 = envelope.SelectSingleNode("//ZKN:zakLa01", ns);
            //var zaakobject = zakla01.SelectSingleNode("//ZKN:object[StUF:entiteittype='ZAK']", ns);
            var zaakobject = zakla01.SelectSingleNode("//ZKN:object[@StUF:entiteittype='ZAK']", ns);
            return zaakobject;
        }

        private static string VerzendZaakWijziging(XmlNode huidigezaak, XmlNode nieuwezaak)
        {
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(ontvangurl);
            // http://www.egem.nl/StUF/sector/zkn/0310/updateZaak_Lk01
            webRequest.Headers.Add("SOAPAction", "http://www.egem.nl/StUF/sector/zkn/0310/updateZaak_Lk01");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            var huidigezaakxml = huidigezaak.InnerXml.ToString();
            var nieuwezaakxml = nieuwezaak.InnerXml.ToString();
            #region content
            var content = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
	<s:Body>        
		<ZKN:zakLk01 xmlns:ZKN=""http://www.egem.nl/StUF/sector/zkn/0310"" xmlns:BG=""http://www.egem.nl/StUF/sector/bg/0310"" xmlns:StUF=""http://www.egem.nl/StUF/StUF0301"" xmlns:gml=""http://www.opengis.net/gml"" xmlns:xlink=""http://www.w3.org/1999/xlink"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.egem.nl/StUF/sector/zkn/0310 ../../gemmaonline.nl/Zkn0310_20140107_patch18/zkn0310/zkn0310_msg_totaal.xsd"">
			<ZKN:stuurgegevens>
				<StUF:berichtcode>Lk01</StUF:berichtcode>				
				<StUF:zender>
					<StUF:organisatie>1900</StUF:organisatie>
					<StUF:applicatie>GenericUTurn.WijzigingsBerichtenCorrigeren</StUF:applicatie>
				</StUF:zender>
				<StUF:ontvanger>
					<StUF:organisatie>1900</StUF:organisatie>
					<StUF:applicatie>CDR</StUF:applicatie>
				</StUF:ontvanger>
				<StUF:referentienummer>" + Guid.NewGuid().ToString() + @"</StUF:referentienummer>
				<StUF:tijdstipBericht>" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @"</StUF:tijdstipBericht>
				<StUF:entiteittype>ZAK</StUF:entiteittype>
			</ZKN:stuurgegevens>
			<ZKN:parameters>
				<StUF:mutatiesoort>W</StUF:mutatiesoort>
				<StUF:indicatorOvername>V</StUF:indicatorOvername>
			</ZKN:parameters>            
			<!-- <ZKN:object StUF:entiteittype=""ZAK"" StUF:sleutelVerzendend=""procesid"" StUF:verwerkingssoort=""W""> -->
            <ZKN:object StUF:entiteittype=""ZAK"" StUF:verwerkingssoort=""W"">"
                + huidigezaakxml +
@"			</ZKN:object>
			<!-- <ZKN:object StUF:entiteittype=""ZAK"" StUF:sleutelVerzendend=""procesid"" StUF:verwerkingssoort=""W""> -->
            <ZKN:object StUF:entiteittype=""ZAK"" StUF:verwerkingssoort=""W"">"
                + nieuwezaakxml +
@"          </ZKN:object>
		</ZKN:zakLk01>
	</s:Body>
</s:Envelope>";
            #endregion
            string result;
            using (System.IO.Stream stream = webRequest.GetRequestStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(content);
                stream.Write(data, 0, data.Length);
            }
            // begin async call to web request.
            var asyncResult = webRequest.BeginGetResponse(null, null);
            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();
            // get the response from the completed web request.
            var response = (System.Net.HttpWebResponse)webRequest.EndGetResponse(asyncResult);
            // (int)response.StatusCode;

            using (System.IO.Stream stream = response.GetResponseStream())
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }
            var envelope = new XmlDocument();
            envelope.LoadXml(result);

            return "====================================================\n\r" + ontvangurl + "\n\r====================================================" + content + "\n\r\n\r====================================================\n\r\n\r" + result;
        }
    }
}
