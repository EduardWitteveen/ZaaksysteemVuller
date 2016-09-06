using System.Xml;
using System;

public class Zkn0310ZsDms : GenericHttpHandler
{
    public Zkn0310ZsDms()
    {
    }

    protected override XmlDocument ProcessXml(string url, string action, XmlDocument document)
    {
        if(action == "http://www.egem.nl/StUF/sector/zkn/0310/genereerZaakIdentificatie_Di02")
        {

            var bericht = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <ZKN:genereerZaakIdentificatie_Du02 xmlns:StUF=""http://www.egem.nl/StUF/StUF0301"" xmlns:ZKN=""http://www.egem.nl/StUF/sector/zkn/0310"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
      <ZKN:stuurgegevens>
        <StUF:berichtcode>Du02</StUF:berichtcode>
        <StUF:zender>
          <StUF:organisatie>Oranisatie</StUF:organisatie>
          <StUF:applicatie>CDR</StUF:applicatie>
        </StUF:zender>
        <StUF:ontvanger>
          <StUF:organisatie>Oranisatie</StUF:organisatie>
          <StUF:applicatie>Midoffice Vuller 1.0</StUF:applicatie>
          <StUF:gebruiker>gebruiker</StUF:gebruiker>
        </StUF:ontvanger>
        <StUF:referentienummer>" + Guid.NewGuid().ToString() + @"</StUF:referentienummer>
        <StUF:tijdstipBericht>" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + @"</StUF:tijdstipBericht>
        <StUF:functie>genereerZaakidentificatie</StUF:functie>
      </ZKN:stuurgegevens>
      <ZKN:zaak StUF:entiteittype=""ZAK"" StUF:functie=""entiteit"">
        <ZKN:identificatie>" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + @"</ZKN:identificatie>
      </ZKN:zaak>
    </ZKN:genereerZaakIdentificatie_Du02>
  </s:Body>
</s:Envelope>";
            var result = new XmlDocument();
            result.LoadXml(bericht);
            return result;
        }
        else { 
            // debug info:
            System.Diagnostics.Debug.WriteLine("==================");
            System.Diagnostics.Debug.WriteLine(document.OuterXml);
            System.Diagnostics.Debug.WriteLine("==================");
            // de namespace waar we inzoeken moet bekend zijn
            var soapnsmgr = new System.Xml.XmlNamespaceManager(document.NameTable);
            soapnsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            var message = new System.Xml.XmlDocument();
            // nu maar eens een xml met alleen de message (meteen een mooi xpath voorbeeld, hoe je bvb de objeecten met Document.SelectNodes)
            message.LoadXml(document.SelectSingleNode("//soapenv:Body", soapnsmgr).InnerXml);

            //////////////////////////////////////
            // RESULT:
            /////////////////////////////////////
            System.Diagnostics.Debug.WriteLine("URL:" + url);
            System.Diagnostics.Debug.WriteLine("ACTIE:" + action);
            System.Diagnostics.Debug.WriteLine("XML:" + document.OuterXml);
            //////////////////////////////////////

            // nu nog een einde eraanbreien....
            // ach, we geven gewoon terug wat we binnenkregen, zal vast wel iets debug-baars geven 
            document.DocumentElement.AppendChild(document.CreateComment("=== Groeten van Eduard! ==="));
            return document;
        }
    }
}