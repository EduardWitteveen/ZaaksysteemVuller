using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericUTurn.ServiceClient;

namespace GenericUTurn
{
    public class GenericUTurn
    {
        private ServiceClient.ServiceClient client;
        private DirectoryInfo koppelingen;
        private Xml.Namespaces namespaces;
        private Xml.Substitutor substitutor;
        private POCO.Session uturn;

        public GenericUTurn()
        {
            uturn = new POCO.Session(Properties.Settings.Default.UTurnDatabaseProvider, Properties.Settings.Default.UTurnDatabaseConnection);
            client = new ServiceClient.ServiceClient(uturn);
            substitutor = Xml.Substitutor.Load(new System.IO.FileInfo(Properties.Settings.Default.Substitutor));
            namespaces = Xml.Namespaces.Load(new System.IO.FileInfo(Properties.Settings.Default.Namespaces));

            koppelingen = new System.IO.DirectoryInfo(Properties.Settings.Default.Koppelingen);
            Output.Info("Using zaaktype directory:" + koppelingen.FullName);
            Output.Write(new FileInfo("GenericUTurn.log"));
            if (koppelingen.GetFiles("*.xml").Length == 0) throw new Exception("geen koppelingsbestanden(*.xml) gevonden in de directory:" + koppelingen.FullName);
        }

        public IEnumerable<Zaaktype> GetTypes()
        {
            List<Zaaktype> result = new List<Zaaktype>();
            foreach (var configfile in koppelingen.GetFiles("*.xml"))
            {
                var zaaktype = new Zaaktype(configfile);
                if (zaaktype.Active)
                {
                    result.Add(zaaktype);
                }
                else {
                    Output.Info("inactive config file:" + configfile.FullName);
                    Output.Write(zaaktype.LogFile);
                }
            }
            return result.AsEnumerable<Zaaktype>();
        }


        public int Synchronize(Zaaktype zaaktype)
        {
            int changes = 0;

            var koppeling = new Koppeling(uturn, client, substitutor, namespaces);
            Output.Info("Loading zaaktype: " + zaaktype.Code + " from config file:" + zaaktype.ConfigFile.FullName);            
            Output.Info("Loading sql from: " + zaaktype.SqlFile.FullName);
            var sql = File.ReadAllText(zaaktype.SqlFile.FullName);

            try { 
                changes = koppeling.Synchronize(zaaktype, sql);
            }
            catch (Exception ex) {
                Output.Warn("Fout bij synchroniseren van zaaktype:" + zaaktype.Code, ex);

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.To.Add(zaaktype.Owner);
                message.CC.Add(Properties.Settings.Default.EmailAfzender);
                message.Subject = Properties.Settings.Default.EmailTitel + " #" + zaaktype.Code + ": " + zaaktype.Description;
                message.From = new System.Net.Mail.MailAddress(Properties.Settings.Default.EmailAfzender);
                message.Body = ex.ToString();
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Properties.Settings.Default.EmailSmtp);
                smtp.UseDefaultCredentials = true;
                smtp.Send(message);
            }
            Output.Write(zaaktype.LogFile);
            return changes;
        }
    }
}
