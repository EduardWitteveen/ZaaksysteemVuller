using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericUTurn.ServiceClient
{
    public class ServiceClientResponse
    {
        private Dictionary<string, string> variables;

        public GenericUTurn.Xml.TemplaceDocument Content { get; set; }

        public ServiceClientResponse(System.Net.WebResponse webResponse, Dictionary<string, string> variables)
        {
            Content = new GenericUTurn.Xml.TemplaceDocument(webResponse.GetResponseStream());
            this.variables = variables;
        }
    }
}
