using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenericUTurn
{
    public class Zaaktype
    {
        System.Xml.XmlDocument config = new System.Xml.XmlDocument();
        public readonly string Code;
        public readonly System.IO.FileInfo SqlFile;
        public readonly System.IO.FileInfo LogFile;
        public readonly System.IO.FileInfo ConfigFile;

        public Zaaktype(System.IO.FileInfo configfile)
        {
            config.Load(configfile.FullName);
            Code = configfile.Name.Replace(configfile.Extension, "").ToUpper();
            if (config.SelectSingleNode("/config/code/text()").Value != this.Code) throw new Exception("zaaktypecode: " + this.Code + " does not match: /config/code/text() (" + config.SelectSingleNode("/config/code/text()").Value + ") in the file:" + configfile.FullName);

            ConfigFile = new System.IO.FileInfo(configfile.FullName.Replace(configfile.Extension, ".xml"));
            SqlFile = new System.IO.FileInfo(configfile.FullName.Replace(configfile.Extension, ".sql"));
            LogFile = new FileInfo(configfile.FullName.Replace(configfile.Extension, ".log"));
        }

        public bool Active {
            get {
                return Convert.ToBoolean(config.SelectSingleNode("/config/@active").Value);
            }
        }
        
        public string Description
        {
            get
            {
                return config.SelectSingleNode("/config/description/text()").Value;
            }
        }

        public string Connection
        {
            get
            {
                return config.SelectSingleNode("/config/connection/text()").Value;
            }
        }


        public string Provider
        {
            get
            {
                return config.SelectSingleNode("/config/provider/text()").Value;
            }
        }

        public string Owner
        {
            get
            {
                return config.SelectSingleNode("/config/email/text()").Value;
            }
        }

        public string[] Statusus
        {
            get
            {
                var nodes = config.SelectNodes("/config/statuscodes/status");
                var result = new List<String>();
                foreach (System.Xml.XmlNode node in nodes)
                {
                    result.Add(node.Attributes["code"].Value);
                }
                return result.ToArray();
            }
        }


        public string[] Starters
        {
            get
            {
                var nodes = config.SelectNodes("/config/statuscodes/status[@type='start']");
                var result = new List<String>();
                foreach(System.Xml.XmlNode node in nodes) {
                    result.Add(node.Attributes["code"].Value);
                }
                return result.ToArray();
            }
        }

        public string[] Stoppers
        {
            get
            {
                var nodes = config.SelectNodes("/config/statuscodes/status[@type='stop']");
                var result = new List<String>();
                foreach (System.Xml.XmlNode node in nodes)
                {
                    result.Add(node.Attributes["code"].Value);
                }
                return result.ToArray();
            }
        }

        public string[] Results
        {
            get
            {
                var nodes = config.SelectNodes("/config/resultaten/resultaat/text()");
                var result = new List<String>();
                foreach (System.Xml.XmlNode node in nodes)
                {
                    result.Add(node.Value);
                }
                return result.ToArray();
            }
        }   
    }
}
