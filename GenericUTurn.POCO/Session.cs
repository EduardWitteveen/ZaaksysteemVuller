using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn.POCO
{
    public class Session
    {
        private System.Data.Common.DbProviderFactory factory = null;
        private System.Data.Common.DbConnection connection = null;
        private string zaaktype = null;

        public Session(string provider, string connectionstring, string zaaktype)
        {
            // ERROR:   'Microsoft.ACE.OLEDB.12.0' provider is not registered on the local machine
            // INSTALL: http://www.microsoft.com/en-us/download/details.aspx?id=23734
            Console.WriteLine("Loading provider: " + provider); 
            factory = System.Data.Common.DbProviderFactories.GetFactory(provider);
            connection = factory.CreateConnection();
            connection.ConnectionString = connectionstring;
            this.zaaktype = zaaktype;
        }

        public Session(string provider, string connectionstring)
            : this(provider, connectionstring, null)
        {
        }

        public void Open()
        {
            Console.WriteLine("Connecting to: " + connection.ConnectionString);
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }


        private Zaak CreateZaak(string zaaktype, System.Data.Common.DbDataReader reader)
        {
            var zaak = new Zaak();
            zaak.ZaaktypeCode = zaaktype;

            Type type = zaak.GetType();
            System.Reflection.PropertyInfo[] properties = type.GetProperties();

            // omdat we optionele velden hebben op deze manier
            for (int i = 0; i < reader.FieldCount; i++)
            {
                System.Reflection.PropertyInfo property = null;
                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    // field == property                    
                    if(reader.GetName(i).ToUpper() == p.Name.ToUpper()) {
                        property = p;
                    }
                }
                System.Diagnostics.Debug.WriteLine("Matching sql field:" + reader.GetName(i) + " on property: " + property.Name);
                if(property == null) throw new ArgumentException("No property found on zaak for the parameter with name:" + reader.GetName(i));
                var data = reader[i];
                if(data.GetType() == typeof(DateTime)) {
                    property.SetValue(zaak, ((DateTime)data).ToString("yyyyMMdd"));
                }
                else if (data.GetType() == typeof(Decimal))
                {
                    property.SetValue(zaak, ((Decimal)data).ToString());
                }
                else if (data.GetType() == typeof(Double))
                {
                    property.SetValue(zaak, ((Double)data).ToString());
                }
                else if (data.GetType() == typeof(DBNull))
                {
                    property.SetValue(zaak, null);
                }
                else {
                    property.SetValue(zaak, data);
                }
            }

            zaak.Validate();
            return zaak;
        }

        private System.Data.Common.DbCommand getCommand(string sql)
        {
            var command = factory.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;
            return command;
        }        

        private void addParameter(System.Data.Common.DbCommand cmd, string parametername, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parametername;
            // handle null in some nice way
            if (value == null) parameter.Value = DBNull.Value;
            else parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }

        public Zaak GetZaak(string zaaktype, string procesid)
        {
            var sql =
                @"
                SELECT * 
                FROM zaken 
                WHERE procesid=@procesid
                AND zaaktypecode=@zaaktypecode
                ";
            var cmd = getCommand(sql);
            addParameter(cmd, "@procesid", procesid);
            addParameter(cmd, "@zaaktypecode", zaaktype);
            var reader = cmd.ExecuteReader();
            Zaak result = null;
            if (reader.Read())
            {
                result =  CreateZaak(zaaktype, reader);
            }
            reader.Close();
            return result;
        }

        public List<Zaak> GetZaken(string zaaktype, string source)
        {
            System.Diagnostics.Debug.WriteLine("GetZaken: begin");
            var result = new List<Zaak>();
            var sql = source;
            var cmd = getCommand(sql);
            System.Diagnostics.Debug.WriteLine("GetZaken: before execute sql");
            var reader = cmd.ExecuteReader();
            System.Diagnostics.Debug.WriteLine("GetZaken: after execute sql");
            while (reader.Read())
            {
                System.Diagnostics.Debug.Write(".");
                result.Add(CreateZaak(zaaktype, reader));
            }
            reader.Close();
            System.Diagnostics.Debug.WriteLine("GetZaken: done");
            return result;
        }

        public Zaak Add(Zaak zaak)
        {
            String fields = null;
            String values = null;

            var type = zaak.GetType();
            var properties = type.GetProperties();
            var cmd = connection.CreateCommand();
            zaak.Timestamp = DateTime.Now.ToString();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                if (fields == null)
                {
                    fields = "[" + property.Name + "]";
                    values = "@" + property.Name;                                         
                }
                else
                {
                    fields += ",\n\t[" + property.Name + "]";
                    values += ",\n\t@" + property.Name;   
                }
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "@" + property.Name;
                var value = property.GetValue(zaak);
                if (value != null) parameter.Value = value; else parameter.Value = DBNull.Value;
                property.GetValue(zaak);                
                cmd.Parameters.Add(parameter);
            }
            cmd.CommandText = "INSERT INTO zaken (" + fields + ") VALUES (" + values + ")";
            if (cmd.ExecuteNonQuery() != 1) throw new Exception("Could not add Zaak with sql:" + cmd.CommandText);

            return zaak;
        }

        public Zaak Update(Zaak zaak)
        {
            var type = zaak.GetType();
            var properties = type.GetProperties();
            var cmd = connection.CreateCommand();
            zaak.Timestamp = DateTime.Now.ToString();

            string fieldvalues = null;
            //var sql = "UPDATE zaken SET ";
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                if (fieldvalues == null)
                {
                    fieldvalues = "[" + property.Name + "] = @" + property.Name;
                }
                else
                {
                    fieldvalues += ",\n\t[" + property.Name + "] = @" + property.Name;
                }
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "@" + property.Name;
                var value = property.GetValue(zaak);
                if (value != null) parameter.Value = value; else parameter.Value = DBNull.Value;
                property.GetValue(zaak);
                cmd.Parameters.Add(parameter);
            }
            // ZaakId zou al bekend moeten zijn! ;-)
            cmd.CommandText = " UPDATE zaken SET " + fieldvalues + " WHERE [ZaakId] = @ZaakId";

            if (cmd.ExecuteNonQuery() != 1) throw new Exception("Could not add Zaak with sql:" + cmd.CommandText);

            return zaak;
        }

        public Bericht Add(Bericht bericht)
        {
            var sql =
                @"
                INSERT INTO berichten (
                        [url],
                        [action],
                        [kenmerk],
                        [requestbody],
                        [responsecode],
                        [responsebody]
                )
                VALUES (
                        @url,
                        @action,
                        @kenmerk,
                        @requestbody,
                        @responsecode,
                        @responsebody
                )";

            var cmd = getCommand(sql);
            // optional fields
            addParameter(cmd, "@url", bericht.Url);
            addParameter(cmd, "@action", bericht.Action);
            addParameter(cmd, "@kenmerk", bericht.Kenmerk);
            addParameter(cmd, "@requestbody", bericht.RequestBody);
            addParameter(cmd, "@responsecode", bericht.ResponseCode);
            addParameter(cmd, "@response", bericht.ResponseBody);
            // Insert            
            if (cmd.ExecuteNonQuery() != 1) throw new Exception("Could not add bericht with sql:" + sql);
            // Retrieve our id
            sql = "SELECT @@IDENTITY";
            cmd = getCommand(sql);
            var result = cmd.ExecuteScalar();
            bericht.Berichtid = Convert.ToInt64(result);
            return bericht;
        }

        public Bericht Update(Bericht bericht)
        {
            // werkte niet met Berichtid als parameter!!
            // hoe stom is dat!
            var sql =
                @"
                UPDATE berichten SET 
                    [timestamp] = NOW(),
                    [url] = @url,
                    [action] = @action,
                    [kenmerk] = @kenmerk,
                    [requestbody] = @requestbody,
                    [responsecode] = @responsecode,
                    [responsebody] = @responsebody
                WHERE [berichtid] = " + bericht.Berichtid;
            var cmd = getCommand(sql);
            // required fields
            // addParameter(cmd, "@berichtid", bericht.Berichtid);
            // optional fields
            addParameter(cmd, "@url", bericht.Url);
            addParameter(cmd, "@action", bericht.Action);
            addParameter(cmd, "@kenmerk", bericht.Kenmerk);
            addParameter(cmd, "@requestbody", bericht.RequestBody);
            addParameter(cmd, "@responsecode", bericht.ResponseCode);
            addParameter(cmd, "@responsebody", bericht.ResponseBody);
            if (cmd.ExecuteNonQuery() != 1) throw new Exception("Could not update bericht with sql:" + sql);

            return bericht;
        }

        private static void AddVariable(Dictionary<string, string> variables, string prefix, string name, object value)
        {
            string data = (string) value;
            var key = "${variable-" + prefix + name + "}";
            if (variables.ContainsKey(key)) variables[key] = data;
            else variables.Add(key,data);
        }
        public static void Serialize(Zaak zaak, Dictionary<string, string> variables, string prefix)
        {
            Type type = zaak.GetType();
            System.Reflection.PropertyInfo[] properties = type.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                AddVariable(variables, prefix, property.Name, property.GetValue(zaak));
            }

            if (zaak.MedewerkerIdentificatie != null &&  zaak.MedewerkerIdentificatie.Trim().Length > 0) { 
                // create your domain context
                var ctx = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain);
                var  query = new System.DirectoryServices.AccountManagement.UserPrincipal(ctx);
                query.SamAccountName = zaak.MedewerkerIdentificatie.Trim().ToLower();
                var search = new System.DirectoryServices.AccountManagement.PrincipalSearcher(query);
                var result = (System.DirectoryServices.AccountManagement.UserPrincipal) search.FindOne();
                if (result == null) {
                    query = new System.DirectoryServices.AccountManagement.UserPrincipal(ctx);
                    query.UserPrincipalName = zaak.MedewerkerIdentificatie.ToLower() + "*";
                    search = new System.DirectoryServices.AccountManagement.PrincipalSearcher(query);
                    result = (System.DirectoryServices.AccountManagement.UserPrincipal)search.FindOne();
                    if (result == null) throw new Exception("user not found in LDAP:" + zaak.MedewerkerIdentificatie);
                }
                var resultde = result.GetUnderlyingObject() as System.DirectoryServices.DirectoryEntry;
                variables.Remove("${variable-" + prefix + "MedewerkerIdentificatie}");
                AddVariable(variables,prefix, "MedewerkerIdentificatie", result.SamAccountName);
                AddVariable(variables, prefix, "MedewerkerVoorletters", resultde.Properties["initials"][0].ToString());
                AddVariable(variables, prefix, "MedewerkerVoorvoegsel", result.MiddleName);
                AddVariable(variables, prefix, "MedewerkerAchternaam", result.Surname);
                AddVariable(variables, prefix, "MedewerkerTelefoon", result.VoiceTelephoneNumber);
                AddVariable(variables, prefix, "MedewerkerEmail", result.EmailAddress);
            }
            else
            {
                variables.Remove("${variable-" + prefix + "MedewerkerIdentificatie}");
                AddVariable(variables, prefix, "MedewerkerIdentificatie", null);
                AddVariable(variables, prefix, "MedewerkerVoorletters", null);
                AddVariable(variables, prefix, "MedewerkerVoorvoegsel", null);
                AddVariable(variables, prefix, "MedewerkerAchternaam", null);
                AddVariable(variables, prefix, "MedewerkerTelefoon", null);
                AddVariable(variables, prefix, "MedewerkerEmail", null);
            }
        }
    }
}
