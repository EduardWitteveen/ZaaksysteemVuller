using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn.Xml
{
    public class TemplateDocument
    {
        public System.Xml.XmlDocument Document { get; set; }

        public TemplateDocument(System.IO.Stream stream)
        {
            var reader = new System.IO.StreamReader(stream);
            var content = reader.ReadToEnd();
            this.Document = new System.Xml.XmlDocument();
            this.Document.LoadXml(content);

        }

        public TemplateDocument(System.IO.FileInfo template)
        {
            if (!template.Exists) throw new System.IO.FileNotFoundException("Template file was not found:" + template.FullName, template.FullName);

            this.Document = new System.Xml.XmlDocument();
            this.Document.Load(template.FullName);
        }



        public System.Xml.XmlDocument Substitute(Substitutor substitutor, Namespaces namespaces, Dictionary<string, string> variables)
        {
            // copy the document
           System.Xml.XmlDocument template = (System.Xml.XmlDocument)this.Document.Clone();
            // also do somthing with the namespaces
            var manager = new System.Xml.XmlNamespaceManager(template.NameTable);
            foreach (KeyValuePair<string, string> ns in namespaces)
            {
                manager.AddNamespace(ns.Key, ns.Value);
            }

            var navigator = template.CreateNavigator();
            foreach (string xpath in substitutor.Keys)
            {
//                System.Diagnostics.Debug.WriteLine("replacing xpath:" + xpath);
                foreach (System.Xml.XPath.XPathNavigator nav in navigator.Select(xpath, manager))
                {
                    /*
                    <ZKN:voorvoegselAchternaam xsi:nil="true" StUF:noValue="geenWaarde" />
                    <ZKN:voorvoegselAchternaam>Voorvoegsel</ZKN:voorvoegselAchternaam>
                    */
                    var search = substitutor[xpath];
                    if (variables.ContainsKey(search))
                    {
                        var value = variables[search];

                        if (value == null || value.Trim().Length == 0)
                        {
                            // what will we do with empty values?
                            // HACK HACK
                            nav.InnerXml = "";
                            nav.CreateAttribute("xsi","nil", namespaces["xsi"],"true");
                            nav.CreateAttribute("StUF", "noValue", namespaces["StUF"], "geenWaarde");
                        }
                        else {
                            // should something which is valid in xml world
                            value = new string(value.Where(c => !char.IsControl(c)).ToArray());
                            //value = new string(value.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());

                            if (substitutor.xml.ContainsKey(xpath) && substitutor.xml[xpath] == "xml")
                            {
                                // set the xml value!
                                nav.InnerXml = value;
                            }
                            else {
                                nav.SetValue(value);
                            }
                        }
                        //System.Diagnostics.Debug.WriteLine("\tsuccesfull set the variable: '" + search + "' : " + nav.Value);
                    }
                    else if (search.StartsWith("${")) {
                       // if (!substitutor.optional.Values.Contains(search)) { 
                        /*
                        string keys = null;
                        foreach (string key in variables.Keys)
                        {
                            if (keys == null) keys = key; else keys += ",\n" + key;
                        }
                        System.Diagnostics.Debug.WriteLine("Subsitutor could not replace variable with name:" + search + " was not found! (" + keys + ")");
                        */
                        //}
                    }
                    else 
                    {
                        if (substitutor.xml.ContainsKey(xpath) && substitutor.xml[xpath] == "xml")
                        {
                            // set the xml value!
                            nav.InnerXml = search;
                        }
                        else {
                            nav.SetValue(search);
                        }
                        // System.Diagnostics.Debug.WriteLine("\tsuccesfull set the value: " + nav.Value);
                    }
                }
            }
            foreach (string xpath in substitutor.optional.Keys)
            {
                //System.Diagnostics.Debug.WriteLine("checking optional xpath:" + xpath);
                foreach (System.Xml.XPath.XPathNavigator nav in navigator.Select(xpath, manager))
                {
                    var search = substitutor.optional[xpath];
                    if (!variables.ContainsKey(search) || variables[search] == null || variables[search] == "" )
                    {
                        // no value for the specific variable ==> remove this entry from the xml!
                        // System.Diagnostics.Debug.WriteLine("searched for:" + search + " : removing the elements in xml for:" + xpath);
                        nav.DeleteSelf();                    
                    }
                }
            }
            return template;
        }

        public static String PrettyPrint(String XML)
        {
            String Result = "";
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(XML);
                Result  = PrettyPrint(document);
            }
            catch (System.Xml.XmlException)
            {
            }
            return Result;
        }

        public static String PrettyPrint(System.Xml.XmlDocument document )
        {
            String Result = "";

            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(mStream, Encoding.Unicode);

            try
            {
                writer.Formatting = System.Xml.Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                System.IO.StreamReader sReader = new System.IO.StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (System.Xml.XmlException)
            {
            }

            mStream.Close();
            writer.Close();
            return Result;
        }



        public String GetValue(Namespaces namespaces, string xpath)
        {
            var manager = new System.Xml.XmlNamespaceManager(Document.NameTable);
            foreach (KeyValuePair<string, string> ns in namespaces)
            { 
                manager.AddNamespace(ns.Key, ns.Value);
            }
            var navigator = Document.CreateNavigator();
            var result = navigator.SelectSingleNode(xpath, manager);

            return result.ToString();
        }
        /*
        static void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case System.Xml.Schema.XmlSeverityType.Error:
                    validationError = true;
                    validationLog += "Error: " + e.Message + "\n";
                    break;
                case System.Xml.Schema.XmlSeverityType.Warning:
                    validationLog += "Warning: " + e.Message + "\n";
                    break;
                default:
                    validationLog += "Unknown: " + e.Message + "\n";
                    break;
            }
        }

        public bool ValidateDocument()
        {
            // caution: not thread safe!
            validationLog = "";
            validationError = false;

            var eventHandler = new System.Xml.Schema.ValidationEventHandler(ValidationEventHandler);
            settings.ValidationType = ValidationType.Schema;
            Document.Validate(eventHandler);

            return validationError;
        }
        private static String validationLog;
        private static bool validationError;

        public String ValidationLog
        {
            get
            {
                return validationLog;
            }
        }
        */
    }
}