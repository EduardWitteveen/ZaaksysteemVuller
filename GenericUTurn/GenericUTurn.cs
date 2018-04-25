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
        private Output logging = null;

        public GenericUTurn()
        {
            logging = new Output(this);

            uturn = new POCO.Session(Properties.Settings.Default.UTurnDatabaseProvider, Properties.Settings.Default.UTurnDatabaseConnection);            
            //uturn = new POCO.Session(System.Configuration.ConfigurationManager.ConnectionStrings["UTurnDatabase"].ProviderName, System.Configuration.ConfigurationManager.ConnectionStrings["UTurnDatabase"].ConnectionString);
            client = new ServiceClient.ServiceClient(uturn);
            substitutor = Xml.Substitutor.Load(new System.IO.FileInfo(Properties.Settings.Default.Substitutor));
            namespaces = Xml.Namespaces.Load(new System.IO.FileInfo(Properties.Settings.Default.Namespaces));

            koppelingen = new System.IO.DirectoryInfo(Properties.Settings.Default.Koppelingen);
            logging.Info("Using zaaktype directory:" + koppelingen.FullName);
            
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
                    zaaktype.logging.Info("inactive config file:" + configfile.FullName);
                }
            }
            return result.AsEnumerable<Zaaktype>();
        }


        public int Synchronize(Zaaktype zaaktype, int waittime)
        {
            int changes = 0;

            var koppeling = new Koppeling(uturn, client, substitutor, namespaces);
            zaaktype.logging.Info("Loaded zaaktype: " + zaaktype.Code + " from config file:" + zaaktype.ConfigFile.FullName);
            var sql = File.ReadAllText(zaaktype.SqlFile.FullName);
            zaaktype.logging.Info("Loaded sql from: " + zaaktype.SqlFile.FullName);

#if !DEBUG
            try
            { 
#endif
            changes = koppeling.Synchronize(zaaktype, sql, waittime);
#if !DEBUG
            }
            catch (Exception ex) {
                var msg = "Fout bij synchroniseren van zaaktype:" + zaaktype.Code;
                zaaktype.logging.Error(msg, ex);
                logging.Warn(msg, ex);

                try
                {
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
                catch (Exception ex2)
                {
                    logging.Error("error sending email to:" + zaaktype.Owner, ex);
                }
            }
#endif
            return changes;
        }
    }
}
