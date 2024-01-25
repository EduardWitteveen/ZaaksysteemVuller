using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        //private System.Data.Common.DbDataAdapter zakenAdapter = null;
        private string zaaktype = null;

        public Session(string provider, string connectionstring, string zaaktype)
        {
            // SqlServerSpatial.dll is unmanaged code. 
            // You have to install the correct version (64bit) on the server. 
            // Add the DLL to your project. Set the properties of SqlServerSpatial110.dll to “Copy to Output directory = Copy always”
            // https://www.nuget.org/packages/Microsoft.SqlServer.Types/
            // https://www.microsoft.com/en-us/download/details.aspx?id=26728
//            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);


            // ERROR:   'Microsoft.ACE.OLEDB.12.0' provider is not registered on the local machine
            // INSTALL: http://www.microsoft.com/en-us/download/details.aspx?id=23734
            Console.WriteLine("Loading provider: " + provider); 
            factory = System.Data.Common.DbProviderFactories.GetFactory(provider);
            connection = factory.CreateConnection();
            connection.ConnectionString = connectionstring;
            this.zaaktype = zaaktype;

            /*
            // if zaaktype is not defined, we want to update our records
            // we are the localdatabase!
            if (zaaktype == null)
            { 
                // Create the adapter
                zakenAdapter = factory.CreateDataAdapter();
                var selectCmd = factory.CreateCommand();
                selectCmd.CommandType = CommandType.Text;
                //selectCmd.CommandText = "SELECT * FROM zaken WHERE [ZaakId] = @ZaakId";
                selectCmd.CommandText = "SELECT * FROM zaken WHERE 4=2";
                selectCmd.Connection = connection;

                zakenAdapter.SelectCommand = selectCmd;
                zakenAdapter.InsertCommand = GetZaakInsertCommand();
                zakenAdapter.UpdateCommand = GetZaakUpdateCommand();
            }
            */
        }
        /*
        private System.Data.Common.DbCommand GetZaakInsertCommand()
        {
            var command= factory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                INSERT INTO zaken 
                (
                    [timestamp], 
                    [procesid], 
                    [zaaktypecode], 
                    [zaaktypeomschrijving], 
                    [zaakid], 
                    [zaakomschrijving], 
                    [zaaktoelichting], 
                    [zaakstatuscode], 
                    [zaakstatusomschrijving], 
                    [startdatum], 
                    [registratiedatum], 
                    [publicatiedatum], 
                    [geplandeeinddatum], 
                    [uiterlijkeeinddatum], 
                    [einddatum], 
                    [kanaalcode], 
                    [resultaatcode], 
                    [resultaatomschrijving], 
                    [aanvragerNpsNaam], 
                    [aanvragerNpsBSN], 
                    [aanvragerNpsGeslachtsnaam], 
                    [aanvragerNpsVoorvoegsel], 
                    [aanvragerNpsVoorletters], 
                    [aanvragerNpsVoornamen], 
                    [aanvragerNpsGeslacht], 
                    [aanvragerNpsGeboortedatum], 
                    [aanvragerNnpRsin], 
                    [aanvragerNnpStatutaireNaam], 
                    [aanvragerVesVestigingsNummer], 
                    [aanvragerVesHandelsnaam], 
                    [lokatiePolygon], 
                    [medewerkerIdentificatie]
                ) VALUES (
                    @timestamp, 
                    @procesid, 
                    @zaaktypecode, 
                    @zaaktypeomschrijving, 
                    @zaakid, 
                    @zaakomschrijving, 
                    @zaaktoelichting, 
                    @zaakstatuscode, 
                    @zaakstatusomschrijving, 
                    @startdatum, 
                    @registratiedatum, 
                    @publicatiedatum, 
                    @geplandeeinddatum, 
                    @uiterlijkeeinddatum, 
                    @einddatum, 
                    @kanaalcode, 
                    @resultaatcode, 
                    @resultaatomschrijving, 
                    @aanvragerNpsNaam, 
                    @aanvragerNpsBSN, 
                    @aanvragerNpsGeslachtsnaam, 
                    @aanvragerNpsVoorvoegsel, 
                    @aanvragerNpsVoorletters, 
                    @aanvragerNpsVoornamen, 
                    @aanvragerNpsGeslacht, 
                    @aanvragerNpsGeboortedatum, 
                    @aanvragerNnpRsin, 
                    @aanvragerNnpStatutaireNaam, 
                    @aanvragerVesVestigingsNummer, 
                    @aanvragerVesHandelsnaam, 
                    @lokatiePolygon, 
                    @medewerkerIdentificatie
                )";

            command.Parameters.Add(addParameter(insertcommand, "@timestamp", System.Data.DbType.DateTime, System.Data.DataRowVersion.Current, "timestamp"));
            command.Parameters.Add(addParameter(insertcommand, "@procesid", System.Data.DbType.String, System.Data.DataRowVersion.Current, "procesid"));
            command.Parameters.Add(addParameter(insertcommand, "@zaaktypecode", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaaktypecode"));
            command.Parameters.Add(addParameter(insertcommand, "@zaaktypeomschrijving", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaaktypeomschrijving"));
            command.Parameters.Add(addParameter(insertcommand, "@zaakid", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaakid"));
            command.Parameters.Add(addParameter(insertcommand, "@zaakomschrijving", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaakomschrijving"));
            command.Parameters.Add(addParameter(insertcommand, "@zaaktoelichting", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaaktoelichting"));
            command.Parameters.Add(addParameter(insertcommand, "@zaakstatuscode", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaakstatuscode"));
            command.Parameters.Add(addParameter(insertcommand, "@zaakstatusomschrijving", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaakstatusomschrijving"));
            command.Parameters.Add(addParameter(insertcommand, "@startdatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "startdatum"));
            command.Parameters.Add(addParameter(insertcommand, "@registratiedatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "registratiedatum"));
            command.Parameters.Add(addParameter(insertcommand, "@publicatiedatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "publicatiedatum"));
            command.Parameters.Add(addParameter(insertcommand, "@geplandeeinddatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "geplandeeinddatum"));
            command.Parameters.Add(addParameter(insertcommand, "@uiterlijkeeinddatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "uiterlijkeeinddatum"));
            command.Parameters.Add(addParameter(insertcommand, "@einddatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "einddatum"));
            command.Parameters.Add(addParameter(insertcommand, "@kanaalcode", System.Data.DbType.String, System.Data.DataRowVersion.Current, "kanaalcode"));
            command.Parameters.Add(addParameter(insertcommand, "@resultaatcode", System.Data.DbType.String, System.Data.DataRowVersion.Current, "resultaatcode"));
            command.Parameters.Add(addParameter(insertcommand, "@resultaatomschrijving", System.Data.DbType.String, System.Data.DataRowVersion.Current, "resultaatomschrijving"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsNaam", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsNaam"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsBSN", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsBSN"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsGeslachtsnaam", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsGeslachtsnaam"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsVoorvoegsel", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsVoorvoegsel"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsVoorletters", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsVoorletters"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsVoornamen", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsVoornamen"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsGeslacht", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsGeslacht"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsGeboortedatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsGeboortedatum"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNnpRsin", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNnpRsin"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNnpStatutaireNaam", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNnpStatutaireNaam"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerVesVestigingsNummer", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerVesVestigingsNummer"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerVesHandelsnaam", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerVesHandelsnaam"));
            command.Parameters.Add(addParameter(insertcommand, "@lokatiePolygon", System.Data.DbType.String, System.Data.DataRowVersion.Current, "lokatiePolygon"));
            command.Parameters.Add(addParameter(insertcommand, "@medewerkerIdentificatie", System.Data.DbType.String, System.Data.DataRowVersion.Current, "medewerkerIdentificatie"));

            command.Connection = connection;
            return command;
        }
        */

        /*
        private System.Data.Common.DbCommand GetZaakUpdateCommand()
        {
            var command = factory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = @"
                UPDATE zaken SET 
                    [timestamp] = @timestamp,
                    [procesid] = @procesid,
                    [zaaktypecode] = @zaaktypecode,
                    [zaaktypeomschrijving] = @zaaktypeomschrijving,
                    [zaakomschrijving] = @zaakomschrijving,
                    [zaaktoelichting] = @zaaktoelichting,
                    [zaakstatuscode] = @zaakstatuscode,
                    [zaakstatusomschrijving] = @zaakstatusomschrijving,
                    [startdatum] = @startdatum,
                    [registratiedatum] = @registratiedatum,
                    [publicatiedatum] = @publicatiedatum,
                    [geplandeeinddatum] = @geplandeeinddatum,
                    [uiterlijkeeinddatum] = @uiterlijkeeinddatum,
                    [einddatum] = @einddatum,
                    [kanaalcode] = @kanaalcode,
                    [resultaatcode] = @resultaatcode,
                    [resultaatomschrijving] = @resultaatomschrijving,
                    [aanvragerNpsNaam] = @aanvragerNpsNaam,
                    [aanvragerNpsBSN] = @aanvragerNpsBSN,
                    [aanvragerNpsGeslachtsnaam] = @aanvragerNpsGeslachtsnaam,
                    [aanvragerNpsVoorvoegsel] = @aanvragerNpsVoorvoegsel,
                    [aanvragerNpsVoorletters] = @aanvragerNpsVoorletters,
                    [aanvragerNpsVoornamen] = @aanvragerNpsVoornamen,
                    [aanvragerNpsGeslacht] = @aanvragerNpsGeslacht,
                    [aanvragerNpsGeboortedatum] = @aanvragerNpsGeboortedatum,
                    [aanvragerNnpRsin] = @aanvragerNnpRsin,
                    [aanvragerNnpStatutaireNaam] = @aanvragerNnpStatutaireNaam,
                    [aanvragerVesVestigingsNummer] = @aanvragerVesVestigingsNummer,
                    [aanvragerVesHandelsnaam] = @aanvragerVesHandelsnaam,
                    [lokatiePolygon] = @lokatiePolygon,
                    [medewerkerIdentificatie] = @medewerkerIdentificatie
                WHERE [zaakid] = @zaakid ";

            command.Parameters.Add(addParameter(insertcommand, "@timestamp", System.Data.DbType.DateTime, System.Data.DataRowVersion.Current, "timestamp"));
            command.Parameters.Add(addParameter(insertcommand, "@procesid", System.Data.DbType.String, System.Data.DataRowVersion.Current, "procesid"));
            command.Parameters.Add(addParameter(insertcommand, "@zaaktypecode", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaaktypecode"));
            command.Parameters.Add(addParameter(insertcommand, "@zaaktypeomschrijving", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaaktypeomschrijving"));
            command.Parameters.Add(addParameter(insertcommand, "@zaakid", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaakid"));
            command.Parameters.Add(addParameter(insertcommand, "@zaakomschrijving", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaakomschrijving"));
            command.Parameters.Add(addParameter(insertcommand, "@zaaktoelichting", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaaktoelichting"));
            command.Parameters.Add(addParameter(insertcommand, "@zaakstatuscode", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaakstatuscode"));
            command.Parameters.Add(addParameter(insertcommand, "@zaakstatusomschrijving", System.Data.DbType.String, System.Data.DataRowVersion.Current, "zaakstatusomschrijving"));
            command.Parameters.Add(addParameter(insertcommand, "@startdatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "startdatum"));
            command.Parameters.Add(addParameter(insertcommand, "@registratiedatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "registratiedatum"));
            command.Parameters.Add(addParameter(insertcommand, "@publicatiedatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "publicatiedatum"));
            command.Parameters.Add(addParameter(insertcommand, "@geplandeeinddatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "geplandeeinddatum"));
            command.Parameters.Add(addParameter(insertcommand, "@uiterlijkeeinddatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "uiterlijkeeinddatum"));
            command.Parameters.Add(addParameter(insertcommand, "@einddatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "einddatum"));
            command.Parameters.Add(addParameter(insertcommand, "@kanaalcode", System.Data.DbType.String, System.Data.DataRowVersion.Current, "kanaalcode"));
            command.Parameters.Add(addParameter(insertcommand, "@resultaatcode", System.Data.DbType.String, System.Data.DataRowVersion.Current, "resultaatcode"));
            command.Parameters.Add(addParameter(insertcommand, "@resultaatomschrijving", System.Data.DbType.String, System.Data.DataRowVersion.Current, "resultaatomschrijving"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsNaam", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsNaam"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsBSN", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsBSN"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsGeslachtsnaam", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsGeslachtsnaam"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsVoorvoegsel", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsVoorvoegsel"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsVoorletters", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsVoorletters"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsVoornamen", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsVoornamen"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsGeslacht", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsGeslacht"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNpsGeboortedatum", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNpsGeboortedatum"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNnpRsin", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNnpRsin"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerNnpStatutaireNaam", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerNnpStatutaireNaam"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerVesVestigingsNummer", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerVesVestigingsNummer"));
            command.Parameters.Add(addParameter(insertcommand, "@aanvragerVesHandelsnaam", System.Data.DbType.String, System.Data.DataRowVersion.Current, "aanvragerVesHandelsnaam"));
            command.Parameters.Add(addParameter(insertcommand, "@lokatiePolygon", System.Data.DbType.String, System.Data.DataRowVersion.Current, "lokatiePolygon"));
            command.Parameters.Add(addParameter(insertcommand, "@medewerkerIdentificatie", System.Data.DbType.String, System.Data.DataRowVersion.Current, "medewerkerIdentificatie"));

            command.Connection = connection;
            return command;
        }
        */
        /*
        private System.Data.Common.DbParameter GetCommandParameter(DbCommand command, string parametername, DbType fieldtype, DataRowVersion version, string columnname)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parametername;
            parameter.DbType = fieldtype;
            parameter.SourceVersion = version;
            parameter.SourceColumn = columnname;
            return parameter;
        }
        */
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

        private System.Data.Common.DbCommand getCommand(string sql)
        {
            var command = factory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection = connection;
            return command;
        }

        private void addParameter(System.Data.Common.DbCommand cmd, string parametername, object value, DbType? dbtype = null)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parametername;
            if (dbtype != null)
            {
                parameter.DbType = (DbType)dbtype;
            }
            // handle null in some nice way
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }
            cmd.Parameters.Add(parameter);
        }

        public UTurnZaak GetZaak(string zaaktype, string procesid)
        {
            var sql = @"
                SELECT * 
                FROM zaken 
                WHERE [procesid] = @procesid
                AND [zaaktypecode] = @zaaktypecode
                ";
            var command = factory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection = connection;
            var adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            addParameter(adapter.SelectCommand, "@procesid", procesid);
            addParameter(adapter.SelectCommand, "@zaaktypecode", zaaktype);
            var table = new System.Data.DataTable();    // TODO: check if no change in zaakid!
            adapter.Fill(table);
            if (table.Rows.Count > 1) throw new Exception("Query returned more than 1 record, should never be possible");
            if (table.Rows.Count > 0) return new UTurnZaak(table.Rows[0]);
            return null;
        }

        public List<BackofficeZaak> GetZaken(string zaaktype, string sql) {
            System.Diagnostics.Debug.WriteLine("GetZaken: begin");
            var result = new List<BackofficeZaak>();
            var command = factory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection = connection;
            var adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            var table = new System.Data.DataTable();

            System.Diagnostics.Debug.WriteLine("GetZaken: before execute sql");
            adapter.Fill(table);
            System.Diagnostics.Debug.WriteLine("GetZaken: after execute sql");
            foreach(System.Data.DataRow row in table.Rows) {
                System.Diagnostics.Debug.Write(".");

                var zaak = new BackofficeZaak(row);

                // HACK HACK: we willen maar één initiator
                //  1 - natuurlijkPersoon
                //  2 - vestiging
                //  3 - nietNatuurlijkPersoon
                if(zaak.AanvragerNpsBsn != "") {
                    zaak.UseAanvragerNatuurlijkPeroon();
                }
                if (zaak.AanvragerVesVestigingsNummer != "")
                {
                    zaak.UseAanvragerVestiging();
                }

                if (zaak.ZaaktypeCode != zaaktype) throw new ArgumentException(String.Format("zaaktype from configuration {0} does not match the zaaktype {1} from the query", zaak.ZaaktypeCode, zaaktype));
                zaak.Validate();
                result.Add(zaak);
            }
            System.Diagnostics.Debug.WriteLine("GetZaken: done");
            return result;
        }

        public UTurnZaak Add(BackofficeZaak zaak)
        {
            DbCommand insertcommand = factory.CreateCommand();
            insertcommand.Connection = connection;
            insertcommand.CommandType = CommandType.Text;

            insertcommand.CommandText = @"
                INSERT INTO zaken 
                (
                    [timestamp], 
                    [procesid], 
                    [zaaktypecode], 
                    [zaaktypeomschrijving], 
                    [zaakid], 
                    [zaakomschrijving], 
                    [zaaktoelichting], 
                    [zaakstatuscode], 
                    [zaakstatusomschrijving], 
                    [startdatum], 
                    [registratiedatum], 
                    [publicatiedatum], 
                    [geplandeeinddatum], 
                    [uiterlijkeeinddatum], 
                    [einddatum], 
                    [kanaalcode], 
                    [resultaatcode], 
                    [resultaatomschrijving], 
                    [aanvragerNpsNaam], 
                    [aanvragerNpsBSN], 
                    [aanvragerNpsGeslachtsnaam], 
                    [aanvragerNpsVoorvoegsel], 
                    [aanvragerNpsVoorletters], 
                    [aanvragerNpsVoornamen], 
                    [aanvragerNpsGeslacht], 
                    [aanvragerNpsGeboortedatum], 
                    [aanvragerNnpRsin], 
                    [aanvragerNnpStatutaireNaam], 
                    [aanvragerVesVestigingsNummer], 
                    [aanvragerVesHandelsnaam], 
                    [lokatiePolygon], 
                    [medewerkerIdentificatie]
                ) VALUES (
                    @timestamp, 
                    @procesid, 
                    @zaaktypecode, 
                    @zaaktypeomschrijving, 
                    @zaakid, 
                    @zaakomschrijving, 
                    @zaaktoelichting, 
                    @zaakstatuscode, 
                    @zaakstatusomschrijving, 
                    @startdatum, 
                    @registratiedatum, 
                    @publicatiedatum, 
                    @geplandeeinddatum, 
                    @uiterlijkeeinddatum, 
                    @einddatum, 
                    @kanaalcode, 
                    @resultaatcode, 
                    @resultaatomschrijving, 
                    @aanvragerNpsNaam, 
                    @aanvragerNpsBSN, 
                    @aanvragerNpsGeslachtsnaam, 
                    @aanvragerNpsVoorvoegsel, 
                    @aanvragerNpsVoorletters, 
                    @aanvragerNpsVoornamen, 
                    @aanvragerNpsGeslacht, 
                    @aanvragerNpsGeboortedatum, 
                    @aanvragerNnpRsin, 
                    @aanvragerNnpStatutaireNaam, 
                    @aanvragerVesVestigingsNummer, 
                    @aanvragerVesHandelsnaam, 
                    @lokatiePolygon, 
                    @medewerkerIdentificatie
                )";

            addParameter(insertcommand, "@timestamp", Convert.ToDateTime(zaak.Row["timestamp"]));
            addParameter(insertcommand, "@procesid", zaak.Row["procesid"]);
            addParameter(insertcommand, "@zaaktypecode", zaak.Row["zaaktypecode"]);
            addParameter(insertcommand, "@zaaktypeomschrijving", zaak.Row["zaaktypeomschrijving"]);
            addParameter(insertcommand, "@zaakid", zaak.Row["zaakid"]);
            addParameter(insertcommand, "@zaakomschrijving", zaak.Row["zaakomschrijving"]);
            addParameter(insertcommand, "@zaaktoelichting", zaak.Row["zaaktoelichting"]);
            addParameter(insertcommand, "@zaakstatuscode", zaak.Row["zaakstatuscode"]);
            addParameter(insertcommand, "@zaakstatusomschrijving", zaak.Row["zaakstatusomschrijving"]);
            addParameter(insertcommand, "@startdatum", zaak.Row["startdatum"]);
            addParameter(insertcommand, "@registratiedatum", zaak.Row["registratiedatum"]);
            addParameter(insertcommand, "@publicatiedatum", zaak.Row["publicatiedatum"]);
            addParameter(insertcommand, "@geplandeeinddatum", zaak.Row["geplandeeinddatum"]);
            addParameter(insertcommand, "@uiterlijkeeinddatum", zaak.Row["uiterlijkeeinddatum"]);
            addParameter(insertcommand, "@einddatum", zaak.Row["einddatum"]);
            addParameter(insertcommand, "@kanaalcode", zaak.Row["kanaalcode"]);
            addParameter(insertcommand, "@resultaatcode", zaak.Row["resultaatcode"]);
            addParameter(insertcommand, "@resultaatomschrijving", zaak.Row["resultaatomschrijving"]);
            addParameter(insertcommand, "@aanvragerNpsNaam", zaak.Row["aanvragerNpsNaam"]);
            addParameter(insertcommand, "@aanvragerNpsBSN", zaak.Row["aanvragerNpsBSN"]);
            addParameter(insertcommand, "@aanvragerNpsGeslachtsnaam", zaak.Row["aanvragerNpsGeslachtsnaam"]);
            addParameter(insertcommand, "@aanvragerNpsVoorvoegsel", zaak.Row["aanvragerNpsVoorvoegsel"]);
            addParameter(insertcommand, "@aanvragerNpsVoorletters", zaak.Row["aanvragerNpsVoorletters"]);
            addParameter(insertcommand, "@aanvragerNpsVoornamen", zaak.Row["aanvragerNpsVoornamen"]);
            addParameter(insertcommand, "@aanvragerNpsGeslacht", zaak.Row["aanvragerNpsGeslacht"]);
            addParameter(insertcommand, "@aanvragerNpsGeboortedatum", zaak.Row["aanvragerNpsGeboortedatum"]);
            addParameter(insertcommand, "@aanvragerNnpRsin", zaak.Row["aanvragerNnpRsin"]);
            addParameter(insertcommand, "@aanvragerNnpStatutaireNaam", zaak.Row["aanvragerNnpStatutaireNaam"]);
            addParameter(insertcommand, "@aanvragerVesVestigingsNummer", zaak.Row["aanvragerVesVestigingsNummer"]);
            addParameter(insertcommand, "@aanvragerVesHandelsnaam", zaak.Row["aanvragerVesHandelsnaam"]);
            addParameter(insertcommand, "@lokatiePolygon", zaak.Row["lokatiePolygon"]);
            addParameter(insertcommand, "@medewerkerIdentificatie", zaak.Row["medewerkerIdentificatie"]);

            /*
            var zakenTable = new DataTable();
            zakenAdapter.Fill(zakenTable);
            DataRow newRow = zakenTable.NewRow();

            
            foreach (DataColumn destinationcolumn in zakenTable.Columns)
            {
                DataColumn sourcecolumn = zaak.Row.Table.Columns[destinationcolumn.ColumnName];
                System.Diagnostics.Debug.Assert(sourcecolumn.DataType == destinationcolumn.DataType);
                System.Diagnostics.Debug.WriteLine("["+ destinationcolumn.DataType.ToString() + "]\t" + destinationcolumn.ColumnName + "\t==> " + zaak.Row[destinationcolumn.ColumnName].ToString());
                newRow[destinationcolumn.ColumnName] = zaak.Row[destinationcolumn.ColumnName];
                //if(sourcecolumn.DataType == typeof(string))
                //{
                //    newRow[destinationcolumn.ColumnName] = "test";
                //}
            }
           
            //newRow.ItemArray = zaak.Row.ItemArray;

            zakenTable.Rows.Add(newRow);
            zakenAdapter.Update(zakenTable);
            return new UTurnZaak(newRow);
            */
            if (insertcommand.ExecuteNonQuery() == 0) throw new Exception("Could not insert zaak #" + zaak.ZaakId);

            return GetZaak(zaak.ZaaktypeCode, zaak.Procesid);
        }


        public Zaak Update(Zaak zaak)
        {
            //zakenAdapter.Update(new System.Data.DataRow[] {zaak.Row});
            //zakenAdapter.Update(zaak.Row.Table);
            DbCommand updatecommand = factory.CreateCommand();
            updatecommand.Connection = connection;
            updatecommand.CommandType = CommandType.Text;

            updatecommand.CommandText = @"
                UPDATE zaken SET 
                    [timestamp] = @timestamp,
                    [procesid] = @procesid,
                    [zaaktypecode] = @zaaktypecode,
                    [zaaktypeomschrijving] = @zaaktypeomschrijving,
                    [zaakomschrijving] = @zaakomschrijving,
                    [zaaktoelichting] = @zaaktoelichting,
                    [zaakstatuscode] = @zaakstatuscode,
                    [zaakstatusomschrijving] = @zaakstatusomschrijving,
                    [startdatum] = @startdatum,
                    [registratiedatum] = @registratiedatum,
                    [publicatiedatum] = @publicatiedatum,
                    [geplandeeinddatum] = @geplandeeinddatum,
                    [uiterlijkeeinddatum] = @uiterlijkeeinddatum,
                    [einddatum] = @einddatum,
                    [kanaalcode] = @kanaalcode,
                    [resultaatcode] = @resultaatcode,
                    [resultaatomschrijving] = @resultaatomschrijving,
                    [aanvragerNpsNaam] = @aanvragerNpsNaam,
                    [aanvragerNpsBSN] = @aanvragerNpsBSN,
                    [aanvragerNpsGeslachtsnaam] = @aanvragerNpsGeslachtsnaam,
                    [aanvragerNpsVoorvoegsel] = @aanvragerNpsVoorvoegsel,
                    [aanvragerNpsVoorletters] = @aanvragerNpsVoorletters,
                    [aanvragerNpsVoornamen] = @aanvragerNpsVoornamen,
                    [aanvragerNpsGeslacht] = @aanvragerNpsGeslacht,
                    [aanvragerNpsGeboortedatum] = @aanvragerNpsGeboortedatum,
                    [aanvragerNnpRsin] = @aanvragerNnpRsin,
                    [aanvragerNnpStatutaireNaam] = @aanvragerNnpStatutaireNaam,
                    [aanvragerVesVestigingsNummer] = @aanvragerVesVestigingsNummer,
                    [aanvragerVesHandelsnaam] = @aanvragerVesHandelsnaam,
                    [lokatiePolygon] = @lokatiePolygon,
                    [medewerkerIdentificatie] = @medewerkerIdentificatie
                WHERE [zaakid] = @zaakid ";
            // SET
            addParameter(updatecommand, "@timestamp", zaak.Row["timestamp"]);
            addParameter(updatecommand, "@procesid", zaak.Row["procesid"]);
            addParameter(updatecommand, "@zaaktypecode", zaak.Row["zaaktypecode"]);
            addParameter(updatecommand, "@zaaktypeomschrijving", zaak.Row["zaaktypeomschrijving"]);
            addParameter(updatecommand, "@zaakomschrijving", zaak.Row["zaakomschrijving"]);
            addParameter(updatecommand, "@zaaktoelichting", zaak.Row["zaaktoelichting"]);
            addParameter(updatecommand, "@zaakstatuscode", zaak.Row["zaakstatuscode"]);
            addParameter(updatecommand, "@zaakstatusomschrijving", zaak.Row["zaakstatusomschrijving"]);
            addParameter(updatecommand, "@startdatum", zaak.Row["startdatum"]);
            addParameter(updatecommand, "@registratiedatum", zaak.Row["registratiedatum"]);
            addParameter(updatecommand, "@publicatiedatum", zaak.Row["publicatiedatum"]);
            addParameter(updatecommand, "@geplandeeinddatum", zaak.Row["geplandeeinddatum"]);
            addParameter(updatecommand, "@uiterlijkeeinddatum", zaak.Row["uiterlijkeeinddatum"]);
            addParameter(updatecommand, "@einddatum", zaak.Row["einddatum"]);
            addParameter(updatecommand, "@kanaalcode", zaak.Row["kanaalcode"]);
            addParameter(updatecommand, "@resultaatcode", zaak.Row["resultaatcode"]);
            addParameter(updatecommand, "@resultaatomschrijving", zaak.Row["resultaatomschrijving"]);
            addParameter(updatecommand, "@aanvragerNpsNaam", zaak.Row["aanvragerNpsNaam"]);
            addParameter(updatecommand, "@aanvragerNpsBSN", zaak.Row["aanvragerNpsBSN"]);
            addParameter(updatecommand, "@aanvragerNpsGeslachtsnaam", zaak.Row["aanvragerNpsGeslachtsnaam"]);
            addParameter(updatecommand, "@aanvragerNpsVoorvoegsel", zaak.Row["aanvragerNpsVoorvoegsel"]);
            addParameter(updatecommand, "@aanvragerNpsVoorletters", zaak.Row["aanvragerNpsVoorletters"]);
            addParameter(updatecommand, "@aanvragerNpsVoornamen", zaak.Row["aanvragerNpsVoornamen"]);
            addParameter(updatecommand, "@aanvragerNpsGeslacht", zaak.Row["aanvragerNpsGeslacht"]);
            addParameter(updatecommand, "@aanvragerNpsGeboortedatum", zaak.Row["aanvragerNpsGeboortedatum"]);
            addParameter(updatecommand, "@aanvragerNnpRsin", zaak.Row["aanvragerNnpRsin"]);
            addParameter(updatecommand, "@aanvragerNnpStatutaireNaam", zaak.Row["aanvragerNnpStatutaireNaam"]);
            addParameter(updatecommand, "@aanvragerVesVestigingsNummer", zaak.Row["aanvragerVesVestigingsNummer"]);
            addParameter(updatecommand, "@aanvragerVesHandelsnaam", zaak.Row["aanvragerVesHandelsnaam"]);
            addParameter(updatecommand, "@lokatiePolygon", zaak.Row["lokatiePolygon"]);
            addParameter(updatecommand, "@medewerkerIdentificatie", zaak.Row["medewerkerIdentificatie"]);
            // WHERE 
            addParameter(updatecommand, "@zaakid", zaak.Row["zaakid"]);

            if (updatecommand.ExecuteNonQuery() == 0) throw new Exception("Could not update zaak #" + zaak.ZaakId);

            return zaak;
        }

        public UTurnBericht Add(UTurnBericht bericht)
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
            addParameter(cmd, "@responsebody", bericht.ResponseBody);
            // Insert            
            if (cmd.ExecuteNonQuery() != 1) throw new Exception("Could not add bericht with sql:" + sql);
            // Retrieve our id
            sql = "SELECT @@IDENTITY";
            cmd = getCommand(sql);
            var result = cmd.ExecuteScalar();
            bericht.Berichtid = Convert.ToInt64(result);
            return bericht;
        }

        public UTurnBericht Update(UTurnBericht bericht)
        {
            // werkte niet met Berichtid als parameter!!
            // hoe stom is dat!
            /*
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
            */
            var sql =
                @"
                UPDATE berichten SET 
                    [timestamp] = @timestamp,
                    [url] = @url,
                    [action] = @action,
                    [kenmerk] = @kenmerk,
                    [requestbody] = @requestbody,
                    [responsecode] = @responsecode,
                    [responsebody] = @responsebody
                WHERE [berichtid] = @berichtid";

            var cmd = getCommand(sql);
            // required fields
            // addParameter(cmd, "@berichtid", bericht.Berichtid);
            // optional fields
            addParameter(cmd, "@timestamp", DateTime.Now);
            addParameter(cmd, "@url", bericht.Url);
            addParameter(cmd, "@action", bericht.Action);
            addParameter(cmd, "@kenmerk", bericht.Kenmerk);
            addParameter(cmd, "@requestbody", bericht.RequestBody);
            addParameter(cmd, "@responsecode", bericht.ResponseCode);
            addParameter(cmd, "@responsebody", bericht.ResponseBody);
            addParameter(cmd, "@berichtid", bericht.Berichtid);
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
            AddVariable(variables, prefix, "ZaaktypeCodeProcesid", zaak.ZaaktypeCode + "#" + zaak.Procesid);
            System.DirectoryServices.AccountManagement.UserPrincipal result = null;
            if (zaak.MedewerkerIdentificatie != null &&  zaak.MedewerkerIdentificatie.Trim().Length > 0) {
                // create your domain context

                System.DirectoryServices.AccountManagement.PrincipalContext ctx = null;
                try { 
                    ctx = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain);
                }
                catch(System.DirectoryServices.AccountManagement.PrincipalServerDownException ex)
                {
                    Console.WriteLine("[!!!ERROR!!!] could not connect to LDAP for user check [!!!ERROR!!!]");
                }
                if (ctx != null) { 
                    var  query = new System.DirectoryServices.AccountManagement.UserPrincipal(ctx);

                    // step 1:  a logon name that supports previous version of Windows
                    var identificatie=zaak.MedewerkerIdentificatie.Trim().ToLower();
                    if (identificatie.IndexOf("@") > 0) identificatie = identificatie.Substring(0, identificatie.IndexOf("@"));
                    query.SamAccountName = identificatie;
                    var search = new System.DirectoryServices.AccountManagement.PrincipalSearcher(query);
                    //var result = (System.DirectoryServices.AccountManagement.UserPrincipal) search.FindOne();
                    if (result == null) {
                        // step 2:  the logon name for the user
                        query = new System.DirectoryServices.AccountManagement.UserPrincipal(ctx);
                        query.UserPrincipalName = identificatie + "*";
                        search = new System.DirectoryServices.AccountManagement.PrincipalSearcher(query);
                        result = (System.DirectoryServices.AccountManagement.UserPrincipal)search.FindOne();
                    }
                    if (result == null)
                    {
                        // step 3:  the email adres
                        query = new System.DirectoryServices.AccountManagement.UserPrincipal(ctx);
                        query.EmailAddress = zaak.MedewerkerIdentificatie.Trim().ToLower();
                        search = new System.DirectoryServices.AccountManagement.PrincipalSearcher(query);
                        result = (System.DirectoryServices.AccountManagement.UserPrincipal)search.FindOne();
                    }
                    if (result == null)
                    {
                        Console.WriteLine("[!!!ERROR!!!] user not found in LDAP:" + zaak.MedewerkerIdentificatie + " [!!!ERROR!!!]");
                    }
                }
            }
            if (result != null)
            {
                var resultde = result.GetUnderlyingObject() as System.DirectoryServices.DirectoryEntry;
                variables.Remove("${variable-" + prefix + "MedewerkerIdentificatie}");
                AddVariable(variables, prefix, "MedewerkerIdentificatie", result.SamAccountName);
                if (resultde.Properties["initials"].Count > 0) { 
                    AddVariable(variables, prefix, "MedewerkerVoorletters", resultde.Properties["initials"][0].ToString());
                }
                else
                {
                    AddVariable(variables, prefix, "MedewerkerVoorletters", null);
                }
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