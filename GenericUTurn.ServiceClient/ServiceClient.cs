//using HdrHistogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn.ServiceClient
{

    public class ServiceClientException : Exception
    {
        // A Histogram covering the range from ~466 nanoseconds to 1 hour (3,600,000,000,000 ns) with a resolution of 3 significant figures:
        public int HttpStatusCode;
        //public Exception innerexception;
        //public string message;

        public ServiceClientException(int status, string message, WebException innerexception) : base(message, innerexception)
        {
            this.HttpStatusCode = status;
            //this.message = message;
            //this.innerexception = innerexception;
        }
    }

    public class ServiceClient
    {
        //HdrHistogram.LongHistogram histogram = new HdrHistogram.LongHistogram(HdrHistogram.TimeStamp.Hours(1), 3);
        //public static ServiceClient Test1Client
        //{
        //    get
        //    {
        //        return new ServiceClient(new Uri("http://www.stgregorioschurchdc.org/cgi/websvccal.cgi"));
        //    }
        //}
        /*
            // System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            Uri uri = new Uri("https://192.168.111.30:8080/");
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(uri);
            webRequest.Proxy = new System.Net.WebProxy();
            System.Net.WebResponse webResponse = webRequest.GetResponse();
            //ReadFrom(webResponse.GetResponseStream());
        */
        private POCO.Session logging;

        public ServiceClient(POCO.Session logging)
        {
            this.logging = logging;
        }

        // TODO: misschien Substitutor niet in request, maar als een parameter hier?
        public ServiceClientResponse Call(string kenmerk, Uri url, ServiceClientRequest request, int WaitTime, Dictionary<string, string> variables) 
        {
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", request.Action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            var entry = new POCO.UTurnBericht();
            entry.Action = request.Action;
            entry.Url = url.ToString();
            entry.Kenmerk = kenmerk;
            logging.Add(entry);

            const string MESSAGEID = "${defined-messageid}";
            if (variables.ContainsKey(MESSAGEID)) variables[MESSAGEID] = entry.Berichtid.ToString();
            else variables.Add(MESSAGEID, entry.Berichtid.ToString()); 
            /*
            // controle!
            if (request.Template.ValidateDocument())
            {
                string error = "error validating the xml document:\n";
                error += request.Template.ValidationLog;
                error += "xml:\n";
                error +=  GenericUTurn.Xml.TemplaceDocument.PrettyPrint(request.Template.Document);
                throw new Exception(error);
            }
            */

            var content = request.Template.Document;
            if (request.Substitutor != null)
            {
                content = request.Template.Substitute(request.Substitutor, request.Namespaces, variables);
            }

            // WE ARE STRICT!!!
            // START: xml validation
            content.Schemas.Add(new System.Xml.Schema.XmlSchema());
            var eventHandler = new System.Xml.Schema.ValidationEventHandler(ValidationEventHandler);
            content.Validate(eventHandler);
            // STOP: xml validation
            
            // here we are in the SoapEnvelope
            if (!GenericUTurn.Xml.SoapEnvelope.Wrapped(content))
            {
                content = GenericUTurn.Xml.SoapEnvelope.Wrap(content);
            }

            entry.RequestBody = GenericUTurn.Xml.TemplateDocument.PrettyPrint(content.OuterXml);
            logging.Update(entry);
//            System.Diagnostics.Debug.Print(">>>>>>>>>>\nrequesting:" + url);
//            System.Diagnostics.Debug.Print("action:" + entry.Action);
//            System.Diagnostics.Debug.Print(entry.RequestBody);            

            ServiceClientResponse result = null;
            try
            {
                long startTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();

                using (System.IO.Stream stream = webRequest.GetRequestStream())
                {
                    content.Save(stream);
                }
                // begin async call to web request.
                var asyncResult = webRequest.BeginGetResponse(null, null);
                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();
                // get the response from the completed web request.
                var response = (System.Net.HttpWebResponse) webRequest.EndGetResponse(asyncResult);
                entry.ResponseCode = (int) response.StatusCode;
                result = new ServiceClientResponse(response, variables);

                long elapsed = System.Diagnostics.Stopwatch.GetTimestamp() - startTimestamp;
                //histogram.RecordValue(elapsed);

            }
            catch (System.Net.WebException ex)
            {
                /*
                entry.Response = ex.ToString();
                Exception inner = ex.InnerException;
                while(inner != null) {
                    entry.Response += inner.ToString();
                    inner = inner.InnerException;
                } 
                */

                // there was connection, but something in content wrong
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        using (System.IO.Stream stream = response.GetResponseStream())
                        {
                            System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
                            entry.ResponseBody = reader.ReadToEnd();
                        }   
                        entry.ResponseCode = (int) response.StatusCode;
                        logging.Update(entry);
                        // and throw again (we did our logging)
                        throw new ServiceClientException((int)response.StatusCode, entry.ResponseBody, ex);
                    }
                }
                else {
                    entry.ResponseBody = ex.ToString();
                    logging.Update(entry);
                    throw ex;
                    // throw new Exception("error connecting to:" + url + " soapaction:" + entry.Action + " xml:" + entry.RequestBody, ex);
                }
            }
            entry.ResponseBody = GenericUTurn.Xml.TemplateDocument.PrettyPrint(result.Content.Document.OuterXml);
            logging.Update(entry);

            if (WaitTime > 0)
            {
                System.Diagnostics.Debug.Print("sleeping for {0} milliseconds, to keep BCT-CMIS alive!", WaitTime);
                System.Threading.Thread.Sleep(WaitTime);
            }

            //            System.Diagnostics.Debug.Print("<<<<<<<<<<\nresponse:" + url);
            //            System.Diagnostics.Debug.Print("action:" + entry.Action);
            //            System.Diagnostics.Debug.Print(entry.ResponseBody);

            // using (var writer = new System.IO.StreamWriter("HdrHistogram-webservice.hgrm"))
            // {
            //histogram.OutputPercentileDistribution(writer);
            // }

            return result;
        }

        private void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case System.Xml.Schema.XmlSeverityType.Error:
                    throw new Exception("Error:" +  e.Message);
                    //break;
                case System.Xml.Schema.XmlSeverityType.Warning:
                    throw new Exception("Warning:" + e.Message);
                    //break;
                default:
                    throw new Exception("Unknown:" + e.Message);
            }
        }


        private  void InsertSoapEnvelopeIntoWebRequest(System.Xml.XmlDocument soapEnvelopeXml, System.Net.HttpWebRequest webRequest)
        {
            using (System.IO.Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}
