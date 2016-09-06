using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn.Xml
{
    public class TemplaceDocument
    {
        public System.Xml.XmlDocument Document { get; set; }

        public TemplaceDocument(System.IO.Stream stream)
        {
            var reader = new System.IO.StreamReader(stream);
            var content = reader.ReadToEnd();
            this.Document = new System.Xml.XmlDocument();
            this.Document.LoadXml(content);

        }

        public TemplaceDocument(System.IO.FileInfo template)
        {
            if (!template.Exists) throw new System.IO.FileNotFoundException("Template file was not found:" + template.FullName, template.FullName);

            this.Document = new System.Xml.XmlDocument();
            this.Document.Load(template.FullName);
        }



        public System.Xml.XmlDocument Substitute(Substitutor substitutor, Namespaces namespaces, Dictionary<string, string> variables)
        {
            // copy the document
            System.Xml.XmlDocument result = (System.Xml.XmlDocument)this.Document.Clone();
            // also do somthing with the namespaces
            var manager = new System.Xml.XmlNamespaceManager(result.NameTable);
            foreach (KeyValuePair<string, string> ns in namespaces)
            {
                manager.AddNamespace(ns.Key, ns.Value);
            }

            var navigator = result.CreateNavigator();
            foreach (string xpath in substitutor.Keys)
            {
                System.Diagnostics.Debug.WriteLine("replacing xpath:" + xpath);
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
                            nav.SetValue(value);
                        }
                        System.Diagnostics.Debug.WriteLine("\tsuccesfull set the variable: '" + search + "' : " + nav.Value);
                    }
#if DEBUG
                    else if (search.StartsWith("${")) {
                        string keys = null;
                        foreach (string key in variables.Keys)
                        {
                            if (keys == null) keys = key; else keys += ",\n" + key;
                        }
                        System.Diagnostics.Debug.WriteLine("Subsitutor could not replace variable with name:" + search + " was not found! (" + keys + ")");
                    }
#endif
                    else 
                    {
                        nav.SetValue(search);
                        System.Diagnostics.Debug.WriteLine("\tsuccesfull set the value: " + nav.Value);
                    }                    
                }
            }
            return result;
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
    }
}