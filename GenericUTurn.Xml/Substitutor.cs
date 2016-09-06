using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GenericUTurn.Xml
{
    public class Substitutor: Dictionary<String, String>
    {
        public static Substitutor Load(System.IO.FileInfo fileInfo)
        {
            if (!fileInfo.Exists) throw new System.IO.FileNotFoundException("Substitutor files was not found:" + fileInfo.FullName, fileInfo.FullName);

            var substituror = new Substitutor();
            
            var doc = new System.Xml.XmlDocument();
            doc.Load(fileInfo.FullName);

            foreach (System.Xml.XmlNode row in doc.SelectNodes("/substitutor/entry"))
            {
                var key = row.SelectSingleNode("@key");
                if(key.Value.Length > 0)
                { 
                    try
                    {
                        substituror.Add(key.Value, row.InnerText);
                    }
                    catch (System.ArgumentException ex)
                    {
                        throw new System.ArgumentException("Cannot add the key: '" + key.Value + "'. Maybe it was already added to the collection?", ex);
                    }
                }
            }
            return substituror;
        }
    }
}