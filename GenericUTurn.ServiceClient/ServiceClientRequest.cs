using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericUTurn.ServiceClient
{
    public class ServiceClientRequest
    {
        public string Action { get; set; }

        public GenericUTurn.Xml.Substitutor Substitutor = null;
        public GenericUTurn.Xml.Namespaces Namespaces = null;
        public GenericUTurn.Xml.TemplaceDocument Template { get; set; }

        public ServiceClientRequest(string action, System.IO.FileInfo template, GenericUTurn.Xml.Substitutor substitutor = null, GenericUTurn.Xml.Namespaces namespaces = null)
        {
            this.Action = action;
            if (!template.Exists) throw new System.IO.FileNotFoundException("Requste template not found", template.FullName);
            this.Template = new GenericUTurn.Xml.TemplaceDocument(template);
            this.Substitutor = substitutor;
            this.Namespaces = namespaces;
        }

        public ServiceClientRequest(string action, System.IO.Stream stream, GenericUTurn.Xml.Substitutor substitutor = null, GenericUTurn.Xml.Namespaces namespaces = null)
        {
            this.Action = action;
            this.Template = new GenericUTurn.Xml.TemplaceDocument(stream);
            this.Substitutor = substitutor;
            this.Namespaces = namespaces;
        }
    }
}
