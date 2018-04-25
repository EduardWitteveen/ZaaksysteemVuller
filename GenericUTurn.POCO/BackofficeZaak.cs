using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn.POCO
{
    public class BackofficeZaak : Zaak
    {
        internal BackofficeZaak(System.Data.DataRow row) : base(row)
        {
            if(DBNull.Value.Equals(this.Row["Timestamp"])) this.Row["Timestamp"] = DateTime.Now;
        }

        internal void UseAanvragerNatuurlijkPeroon()
        {
            this.Row["aanvragerNnpRsin"] = DBNull.Value;
            this.Row["aanvragerNnpStatutaireNaam"] = "";

            this.Row["aanvragerVesVestigingsNummer"] = DBNull.Value;
            this.Row["aanvragerVesHandelsnaam"] = "";
        }

        internal void UseAanvragerVestiging()
        {
            this.Row["AanvragerNpsBsn"] = DBNull.Value;
            this.Row["AanvragerNpsNaam"] = "";
            this.Row["AanvragerNpsGeslachtsnaam"] = "";
            this.Row["AanvragerNpsVoorvoegsel"] = "";
            this.Row["AanvragerNpsVoorletters"] = "";
            this.Row["AanvragerNpsVoornamen"] = "";
            this.Row["AanvragerNpsGeslacht"] = "";
            this.Row["AanvragerNpsGeboortedatum"] = DBNull.Value;

            this.Row["aanvragerNnpRsin"] = DBNull.Value;
            this.Row["aanvragerNnpStatutaireNaam"] = "";
        }
    }
}
