using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericUTurn.POCO
{
    public class Zaak
    {
        public string	Timestamp	{ get; set; }
        public string	Procesid	{ get; set; }
        private string _ZaaktypeCode;
        public string	ZaaktypeCode
        {
            get
            {
                return _ZaaktypeCode;
            }
            set
            {
                _ZaaktypeCode = value.ToUpper();
            }
        }
        public string	ZaaktypeOmschrijving	{ get; set; }
        public string	ZaakId	{ get; set; }
        public string	ZaakOmschrijving	{ get; set; }
        public string	ZaakToelichting	{ get; set; }
        public string   ResultaatCode { get; set; }
        public string	ResultaatOmschrijving	{ get; set; }
        public string	ResultaatToelichting	{ get; set; }
        public string	ZaakstatusCode	{ get; set; }
        public string	ZaakstatusOmschrijving	{ get; set; }
        public string	StartDatum	{ get; set; }
        public string	RegistratieDatum	{ get; set; }
        public string	PublicatieDatum	{ get; set; }
        public string	GeplandeeindDatum	{ get; set; }
        public string	UiterlijkeeindDatum	{ get; set; }
        public string	EindDatum	{ get; set; }
        public string	AanvragerNaam	{ get; set; }
        public string	AanvragerTelefoon	{ get; set; }
        public string	AanvragerEmail	{ get; set; }
        public string	AanvragerBsn	{ get; set; }
        public string	AanvragerGeslachtsnaam	{ get; set; }
        public string	AanvragerVoorvoegsel	{ get; set; }
        public string	AanvragerVoorletters	{ get; set; }
        public string	AanvragerVoornamen	{ get; set; }
        public string	AanvragerGeslachtsaanduiding	{ get; set; }
        public string	AanvragerGeboortedatum	{ get; set; }
        public string	MedewerkerIdentificatie	{ get; set; }
        public string   Medium { get; set; }

        public void Validate()
        {
            if (Procesid == null) throw new ArgumentNullException("veld Procesid was niet aangeleverd");
            if (ZaaktypeCode == null) throw new ArgumentNullException("veld ZaaktypeCode was niet aangeleverd voor procesid:" + Procesid);
            if (ZaakstatusCode == null) throw new ArgumentNullException("veld ZaakstatusCode was niet aangeleverd voor procesid:" + Procesid);
            if (StartDatum == null) throw new ArgumentNullException("veld Startdatum was niet aangeleverd voor procesid:" + Procesid);
            if (RegistratieDatum == null) throw new ArgumentNullException("veld Registratiedatum was niet aangeleverd voor procesid:" + Procesid);

            // some checks on errors we encountered in de xml during transmission
            // leuk dat je hier geen foutmeldingen van krijgt overigens,...
            if (ZaakstatusCode.Length > 10) throw new ArgumentOutOfRangeException("zaak.ZaakstatusCode length > 10 :" + ZaakstatusCode + " voor procesid:" + Procesid);

            //ZaakstatusOmschrijving = ZaakstatusOmschrijving.Substring(0, 10);
        }

        public bool Dirty(Zaak compare)
        {
            if (compare == null) throw new ArgumentNullException();
            // do not compare the zaakid!
            // if(this.Zaakid != compare.Zaakid) return false;
            if(this.Procesid != compare.Procesid) throw new Exception("are you sure you want to compare this zaak with the other?");

            //if(this.Timestamp	!= compare.Timestamp	) return true;
            if(this.ZaaktypeCode	!= compare.ZaaktypeCode	) return true;
            if(this.ZaaktypeOmschrijving	!= compare.ZaaktypeOmschrijving	) return true;
            // if(this.ZaakId	!= compare.ZaakId	) return true;
            if(this.ZaakOmschrijving	!= compare.ZaakOmschrijving	) return true;
            if(this.ZaakToelichting	!= compare.ZaakToelichting	) return true;
            if(this.ResultaatCode != compare.ResultaatCode) return true;
            if(this.ResultaatOmschrijving	!= compare.ResultaatOmschrijving	) return true;
            if(this.ResultaatToelichting	!= compare.ResultaatToelichting	) return true;
            if(this.ZaakstatusCode	!= compare.ZaakstatusCode	) return true;
            if(this.ZaakstatusOmschrijving	!= compare.ZaakstatusOmschrijving	) return true;
            if(this.StartDatum	!= compare.StartDatum	) return true;
            if(this.RegistratieDatum	!= compare.RegistratieDatum	) return true;
            if(this.PublicatieDatum	!= compare.PublicatieDatum	) return true;
            if(this.GeplandeeindDatum	!= compare.GeplandeeindDatum	) return true;
            if(this.UiterlijkeeindDatum	!= compare.UiterlijkeeindDatum	) return true;
            if(this.EindDatum	!= compare.EindDatum	) return true;
            if(this.AanvragerNaam	!= compare.AanvragerNaam	) return true;
            if(this.AanvragerTelefoon	!= compare.AanvragerTelefoon	) return true;
            if(this.AanvragerEmail	!= compare.AanvragerEmail	) return true;
            if(this.AanvragerBsn	!= compare.AanvragerBsn	) return true;
            if(this.AanvragerGeslachtsnaam	!= compare.AanvragerGeslachtsnaam	) return true;
            if(this.AanvragerVoorvoegsel	!= compare.AanvragerVoorvoegsel	) return true;
            if(this.AanvragerVoorletters	!= compare.AanvragerVoorletters	) return true;
            if(this.AanvragerVoornamen	!= compare.AanvragerVoornamen	) return true;
            if(this.AanvragerGeslachtsaanduiding	!= compare.AanvragerGeslachtsaanduiding	) return true;
            if(this.AanvragerGeboortedatum	!= compare.AanvragerGeboortedatum	) return true;
            if(this.MedewerkerIdentificatie	!= compare.MedewerkerIdentificatie	) return true;
            if (this.Medium != compare.Medium) return true;
            return false;
        }
    }
}
