using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn.Xml
{
    public class SoapEnvelope
    {
        public static bool Wrapped(System.Xml.XmlDocument content)
        {
            // just "Envelope" is enough for me now...
            return content.DocumentElement.LocalName == "Envelope";
        }

        public static System.Xml.XmlDocument Wrap(System.Xml.XmlDocument content)
        {

            var envelope =
@"<?xml version=""1.0""?>
<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:cal=""http://www.stgregorioschurchdc.org/Calendar"">
    <soapenv:Header/>
    <soapenv:Body />
</soapenv:Envelope>";
            var document = new System.Xml.XmlDocument();
            document.LoadXml(envelope);
            var import = document.ImportNode((System.Xml.XmlNode)content.DocumentElement, true);
            document.DocumentElement.ChildNodes[1].AppendChild(import);

            return document;
        }
    }
}
