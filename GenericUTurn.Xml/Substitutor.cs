using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GenericUTurn.Xml
{
    public class Substitutor : Dictionary<string, string>
    {
        internal Dictionary<string, string> optional;
        internal Dictionary<string, string> xml;

        internal Substitutor()
        {
            optional = new Dictionary<string, string> ();
            xml = new Dictionary<string, string>();
        }

        public static Substitutor Load(System.IO.FileInfo fileInfo)
        {
            if (!fileInfo.Exists) throw new System.IO.FileNotFoundException("Substitutor files was not found:" + fileInfo.FullName, fileInfo.FullName);

            var substitutor = new Substitutor();
            
            var doc = new System.Xml.XmlDocument();
            doc.Load(fileInfo.FullName);

            // replace all the values
            foreach (System.Xml.XmlNode row in doc.SelectNodes("/substitutor/entry"))
            {
                var key = row.SelectSingleNode("@key");
                if(key.Value.Length > 0)
                { 
                    try
                    {
                        substitutor.Add(key.Value, row.InnerText);
                        if (row.Attributes["format"] != null)
                        {
                            substitutor.xml.Add(key.Value, row.Attributes["format"].Value);
                        }
                    }
                    catch (System.ArgumentException ex)
                    {
                        throw new System.ArgumentException("Cannot add the key: '" + key.Value + "'. Maybe it was already added to the collection?", ex);
                    }
                }
            }
            // remove the optional node's
            foreach (System.Xml.XmlNode row in doc.SelectNodes("/substitutor/optional"))
            {
                var key = row.SelectSingleNode("@key");
                if(key.Value.Length > 0) {
                    // 
                    substitutor.optional.Add(key.Value, row.InnerText);
                }
                //if (key.Value.Length > 0)
                //{
                //    try
                //    {
                //        substituror.Add(key.Value, row.InnerText);
                //    }
                //    catch (System.ArgumentException ex)
                //    {
                //        throw new System.ArgumentException("Cannot add the key: '" + key.Value + "'. Maybe it was already added to the collection?", ex);
                //    }
                //}
            }
            return substitutor;
        }
    }
}