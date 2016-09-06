using System.Web;
public class GenericHttpHandler : IHttpHandler
{
    public GenericHttpHandler()
    {
    }

    public bool IsReusable
    {
        // To enable pooling, return true here. This keeps the handler in memory.
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;

        // first detect if we are doing something soapich
        if(request.Headers["SOAPAction"] != null)
        {
            // we miepen alles in de xml
            var soapenvelop = new System.Xml.XmlDocument();
            var stream = request.InputStream;
            soapenvelop.Load(stream);
            stream.Close();
            // debug info:
            System.Diagnostics.Debug.WriteLine("==================");
            System.Diagnostics.Debug.WriteLine(soapenvelop.OuterXml);
            System.Diagnostics.Debug.WriteLine("==================");
            // de namespace waar we inzoeken moet bekend zijn
            var soapnsmgr = new System.Xml.XmlNamespaceManager(soapenvelop.NameTable);
            soapnsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");           
            var message = new System.Xml.XmlDocument();
            // nu maar eens een xml met alleen de message (meteen een mooi xpath voorbeeld, hoe je bvb de objeecten met Document.SelectNodes)
            message.LoadXml(soapenvelop.SelectSingleNode("//soapenv:Body", soapnsmgr).InnerXml);

            //////////////////////////////////////
            // RESULT:
            /////////////////////////////////////
            System.Diagnostics.Debug.WriteLine("URL:" + request.RawUrl);
            System.Diagnostics.Debug.WriteLine("ACTIE:" + request.Headers["SOAPAction"]);
            System.Diagnostics.Debug.WriteLine("XML:" + message.OuterXml);
            //////////////////////////////////////

            // nu nog een einde eraanbreien....
            // ach, we geven gewoon terug wat we binnenkregen, zal vast wel iets debug-baars geven 
            soapenvelop.DocumentElement.AppendChild(soapenvelop.CreateComment("=== Groeten van Eduard! ==="));
            response.Write(soapenvelop.OuterXml);
        }
        else {
            #region GEEN SOAP
            // nog iets om te debuggen, voor als het verkeerd gaat,..
            // TODO: voor alle variabelen moet er nog een html-encoding worden toegespast            
            response.Write("<html>\n\r");
            response.Write("<body>\n\r");
            response.Write("<h1>Request information:</h1>\n\r");
            response.Write("<h2>Request</h2>\n\r");
            response.Write("<table border='1'>\n\r");
            response.Write("<tr><td>HttpMethod</td><td>" + request.HttpMethod + "</td></tr>\n\r");
            response.Write("<tr><td>Path</td><td>" + request.Path + "</td></tr>\n\r");
            response.Write("<tr><td>RawUrl</td><td>" + request.RawUrl + "</td></tr>\n\r");
            response.Write("</table>\n\r");
            response.Write("<h2>Headers</h2>\n\r");
            response.Write("<table border='1'>\n\r");
            foreach (string key in request.Headers.Keys)
            {
                response.Write("<tr><td>" + key + "</td><td>" + request.Headers[key] + "</td></tr>\n\r");
            }
            response.Write("</table>\n\r");
            response.Write("<h2>Params</h2>\n\r");

            response.Write("<table border='1'>\n\r");
            foreach (string key in request.Params.Keys)
            {
                response.Write("<tr><td>" + key + "</td><td>" + request.Params[key] + "</td></tr>\n\r");
            }
            response.Write("</table>\n\r");
            response.Write("<h2>Data</h2>\n\r");

            var stream = new System.IO.StreamReader(request.InputStream);
            response.Write("<pre>" + stream.ReadToEnd()  + "</pre>\n\r");

            response.Write("</body>\n\r");
            response.Write("</html>\n\r");
            #endregion
        }
    }
}
