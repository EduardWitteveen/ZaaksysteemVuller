using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn.ServiceClient
{

    public class ServiceClientException : Exception
    {
        public int HttpStatusCode;

        public ServiceClientException(int httpStatusCode, string message, System.Net.WebException exception)
            : base(message, exception)
        {
            this.HttpStatusCode = httpStatusCode;
        }

    }

    public class ServiceClient
    {

        private POCO.Session logging;

        public ServiceClient(POCO.Session logging)
        {
            this.logging = logging;
        }

        // TODO: misschien Substitutor niet in request, maar als een parameter hier?
        public static System.Xml.Schema.ValidationEventArgs parsererror = null;
        public ServiceClientResponse Call(string kenmerk, Uri url, ServiceClientRequest request, Dictionary<string, string> variables)
        {
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", request.Action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            var entry = new POCO.Bericht();
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
            parsererror = null;
            content.Schemas.Add(new System.Xml.Schema.XmlSchema());
            var eventHandler = new System.Xml.Schema.ValidationEventHandler(ValidationEventHandler);
            content.Validate(eventHandler);
            if (parsererror != null) throw parsererror.Exception;
            // STOP: xml validation
            
            // here we are in the SoapEnvelope
            if (!GenericUTurn.Xml.SoapEnvelope.Wrapped(content))
            {
                content = GenericUTurn.Xml.SoapEnvelope.Wrap(content);
            }

            entry.RequestBody = GenericUTurn.Xml.TemplaceDocument.PrettyPrint(content.OuterXml);
            logging.Update(entry);
            System.Diagnostics.Debug.Print(">>>>>>>>>>\nrequesting:" + url);
            System.Diagnostics.Debug.Print("action:" + entry.Action);
            System.Diagnostics.Debug.Print(entry.RequestBody);            

            ServiceClientResponse result = null;
            try
            {
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
                entry.ResponseCode = response.StatusDescription;
                result = new ServiceClientResponse(response, variables);   

            }
            catch (System.Net.WebException ex)
            {

                var response =  ex.Response as System.Net.HttpWebResponse;
                if(response == null) {
                    // nothing we can use!
                    throw ex;
                }
                
                /*
                entry.Response = ex.ToString();
                Exception inner = ex.InnerException;
                while(inner != null) {
                    entry.Response += inner.ToString();
                    inner = inner.InnerException;
                } 
                */
                using (System.IO.Stream stream = response.GetResponseStream())
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
                    entry.ResponseBody = reader.ReadToEnd();
                }                
                logging.Update(entry);
                // and throw again (we did our logging)
                throw new ServiceClientException((int)response.StatusCode, entry.ResponseBody, ex);
            }
            entry.ResponseBody = GenericUTurn.Xml.TemplaceDocument.PrettyPrint(result.Content.Document.OuterXml);
            logging.Update(entry);
            System.Diagnostics.Debug.Print("<<<<<<<<<<\nresponse:" + url);
            System.Diagnostics.Debug.Print("action:" + entry.Action);
            System.Diagnostics.Debug.Print(entry.ResponseBody);

            return result;
        }

        private void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            parsererror = e;
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
