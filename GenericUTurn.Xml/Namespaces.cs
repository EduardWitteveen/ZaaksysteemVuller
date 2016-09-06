using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GenericUTurn.Xml
{
    public class Namespaces: Dictionary<String, String>
    {
        public static Namespaces Load(System.IO.FileInfo fileInfo)
        {
            if (!fileInfo.Exists) throw new System.IO.FileNotFoundException("Namespaces files was not found:" + fileInfo.FullName, fileInfo.FullName);

            var namespaces = new Namespaces();
            
            var doc = new System.Xml.XmlDocument();
            doc.Load(fileInfo.FullName);

            foreach (System.Xml.XmlNode row in doc.SelectNodes("/namespaces/entry"))
            {
                var key = row.SelectSingleNode("@key");
                try
                {
                    namespaces.Add(key.Value, row.InnerText);
                }
                catch (System.ArgumentException ex)
                {
                    throw new System.ArgumentException("Cannot add the key: '" + key.Value + "'. Maybe it was already added to the collection?", ex);
                }
            }
            return namespaces;
        }
    }
}