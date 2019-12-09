using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericUTurn.POCO
{
    public abstract class Zaak
    {
        public System.Data.DataRow Row;
        internal Zaak(System.Data.DataRow row)
        {
            this.Row = row;
        }

        public string Timestamp
        {
            get { return ConvertToDateString(Row["Timestamp"], "yyyyMMddhhmmssfff"); }
            set { Row["Timestamp"] = value; }
        }
        //public string Timestamp	{
        //    get { return Convert.ToString(Row["Timestamp"]); }
        //    set { Row["Timestamp"] = value; }
        //}
        public string Procesid {
            get { return Convert.ToString(Row["Procesid"]); }
            //set { Row["Procesid"] = value; }
        }
        public string ZaaktypeCode {
            get { return ConvertToString(Row["ZaaktypeCode"]).ToUpper(); }
            //set { Row["ZaaktypeCode"] = value.ToUpper(); }
        }
        public string ZaaktypeOmschrijving {
            get { return ConvertToString(Row["ZaaktypeOmschrijving"]); }
            //set { Row["ZaaktypeOmschrijving"] = value; }
        }
        public string ZaakId
        {
            get { return ConvertToString(Row["ZaakId"]); }
            set { Row["ZaakId"] = value; }
        }
        public string ZaakOmschrijving
        {
            get { return ConvertToString(Row["ZaakOmschrijving"]); }
            //set { Row["ZaakOmschrijving"] = value; }
        }
        public string ZaakToelichting
        {
            get { return ConvertToString(Row["ZaakToelichting"]); }
            //set { Row["ZaakToelichting"] = value; }
        }
        public string ZaakstatusCode
        {
            get { return ConvertToString(Row["ZaakstatusCode"]); }
            //set { Row["ZaakstatusCode"] = value; }
        }
        public string ZaakstatusOmschrijving
        {
            get {
                return ConvertToString(Row["ZaakstatusOmschrijving"]);
            }
            //set { Row["ZaakstatusOmschrijving"] = value; }
        }

        private string ConvertToString(object obj)
        {
            var result = Convert.ToString(obj);
            if (result.Contains((char) 11)) return "appelmoes";
            // result = System.Text.RegularExpressions.Regex.Replace(result, @"[^\u0000-\u007F]+", string.Empty);
            // je wilt niet weten wat je soms voor troep tegen komt!
            result = new string(result.Where(c => char.IsLetter(c) || char.IsDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c)).ToArray());
            return result;
        }

        private string ConvertToDateString(object obj, string format = "yyyyMMdd")
        {
            if (DBNull.Value.Equals(obj)) return null;
            string str =  Convert.ToString(obj);
            DateTime dt;
            if(DateTime.TryParse(str, out dt)) {
                return dt.ToString(format);
            }
            else {
                return str;
            }
            /*
            try { 
                DateTime dt = Convert.ToDateTime(obj);
                return dt.ToString(format);
            }
            catch(Exception e) {
                return Convert.ToString(obj);
            } 
            */
        }
        public string StartDatum
        {
            get { return ConvertToDateString(Row["StartDatum"]); }
            //set { Row["StartDatum"] = value; }
        }
        public string RegistratieDatum
        {
            get { return ConvertToDateString(Row["RegistratieDatum"]); }
            //set { Row["RegistratieDatum"] = value; }
        }
        public string PublicatieDatum
        {
            get { return ConvertToDateString(Row["PublicatieDatum"]); }
            //set { Row["PublicatieDatum"] = value; }
        }
        public string GeplandeeindDatum
        {
            get { return ConvertToDateString(Row["GeplandeeindDatum"]); }
            //set { Row["GeplandeeindDatum"] = value; }
        }
        public string UiterlijkeeindDatum
        {
            get { return ConvertToDateString(Row["UiterlijkeeindDatum"]); }
            //set { Row["UiterlijkeeindDatum"] = value; }
        }
        public string EindDatum
        {
            get { return ConvertToDateString(Row["EindDatum"]); }
            //set { Row["EindDatum"] = value; }
        }
        public string Kanaalcode
        {
            get { return Convert.ToString(Row["Kanaalcode"]); }
            //set { Row["Kanaalcode"] = value; }
        }
        public string ResultaatCode
        {
            get { return Convert.ToString(Row["ResultaatCode"]); }
            //set { Row["ResultaatCode"] = value; }
        }
        public string ResultaatOmschrijving
        {
            get { return Convert.ToString(Row["ResultaatOmschrijving"]); }
            //set { Row["ResultaatOmschrijving"] = value; }
        }
//        public string ResultaatToelichting
//        {
//            get { return Convert.ToString(Row["ResultaatToelichting"]); }
//            set { Row["ResultaatToelichting"] = value; }
//        }
        public string AanvragerNpsNaam
        {
            get { return Convert.ToString(Row["AanvragerNpsNaam"]); }
            //set { Row["AanvragerNpsNaam"] = value; }
        }
        public string AanvragerNpsBsn
        {
            get {
                var nummer = Convert.ToString(Row["AanvragerNpsBsn"]);
                if (nummer.Trim().Length == 0) return "";
                return nummer.PadLeft(9, '0');
            }
            //set { Row["AanvragerNpsBsn"] = value; }
        }
        public string AanvragerNpsGeslachtsnaam
        {
            get { return Convert.ToString(Row["AanvragerNpsGeslachtsnaam"]); }
            //set { Row["AanvragerNpsGeslachtsnaam"] = value; }
        }
        public string AanvragerNpsVoorvoegsel
        {
            get { return Convert.ToString(Row["AanvragerNpsVoorvoegsel"]); }
            //set { Row["AanvragerNpsVoorvoegsel"] = value; }
        }
        public string AanvragerNpsVoorletters
        {
            get { return Convert.ToString(Row["AanvragerNpsVoorletters"]); }
            //set { Row["AanvragerNpsVoorletters"] = value; }
        }
        public string AanvragerNpsVoornamen
        {
            get { return Convert.ToString(Row["AanvragerNpsVoornamen"]); }
            //set { Row["AanvragerNpsVoornamen"] = value; }
        }
        public string AanvragerNpsGeslachtsaanduiding
        {
            get { return Convert.ToString(Row["AanvragerNpsGeslacht"]); }
            //set { Row["AanvragerNpsGeslacht"] = value; }
        }
        public string AanvragerNpsGeboortedatum
        {
            get { return ConvertToDateString(Row["AanvragerNpsGeboortedatum"]); }
            //set { Row["AanvragerNpsGeboortedatum"] = value; }
        }
        public string AanvragerNnpRsin
        {
            get { 
                var nummer = Convert.ToString(Row["aanvragerNnpRsin"]);
                if(nummer.Trim().Length == 0) return "";
                return nummer.PadLeft(9, '0');
            }
            //set { Row["aanvragerNnpRsin"] = value; }
        }
        public string AanvragerNnpStatutaireNaam
        {
            get { return Convert.ToString(Row["aanvragerNnpStatutaireNaam"]);
            }
            //set { Row["aanvragerNnpStatutaireNaam"] = value; }
        }
        public string AanvragerVesVestigingsNummer
        {
            // lengte 12 nodig!
            get {
                var nummer = Convert.ToString(Row["aanvragerVesVestigingsNummer"]);
                if (nummer.Trim().Length == 0) return "";
                return nummer.PadLeft(12, '0');
            }
            //set { Row["aanvragerVesVestigingsNummer"] = value; }
        }
        public string AanvragerVesHandelsnaam
        {
            get { return Convert.ToString(Row["aanvragerVesHandelsnaam"]); }
            //set { Row["aanvragerVesHandelsnaam"] = value; }
        }
        public string lokatiePolygon
        {
            // We expect WKT in database, GML from the properties
            //      Tools > NuGet Package Manager > Package Manager Console
            //      PM> Install-Package Microsoft.SqlServer.Types
            get
            {
                // always perform before using 
                var wkt = Convert.ToString(Row["lokatiePolygon"]);
                if (wkt == null || wkt.Length == 0)
                {
                    return null;
                }
                else
                {
                    /*
                    Volgens king:

				    <gml:Polygon srsDimension="2" srsName="urn:ogc:def:crs:EPSG::28992">
					    <gml:exterior>
						    <gml:LinearRing>
							    <gml:pos>3272.222</gml:pos>
                                ....
							    <gml:pos>565622.222</gml:pos>
						    </gml:LinearRing>
					    </gml:exterior>
				    </gml:Polygon>			
			
				    vs: 
			
                    Volgens huidige code
				    <MultiSurface xmlns="http://www.opengis.net/gml">
					    <surfaceMembers>
						    <Polygon>
							    <exterior>
								    <LinearRing>
									    <posList>176439.94377716 562751.18565465 176438.398957219 562744.542928906 176442.569971059 562746.860158816 176439.94377716 562751.18565465</posList>
								    </LinearRing>
							    </exterior>
						    </Polygon>
					    </surfaceMembers>
				    </MultiSurface>
                    */
                    // SqlServerSpatial.dll is unmanaged code. 
                    // You have to install the correct version (64bit) on the server. 
                    // Add the DLL to your project. Set the properties of SqlServerSpatial110.dll to “Copy to Output directory = Copy always”
                    // https://www.nuget.org/packages/Microsoft.SqlServer.Types/
                    // https://www.microsoft.com/en-us/download/details.aspx?id=26728
                    // 

                    Microsoft.SqlServer.Types.SqlGeometry geom = Microsoft.SqlServer.Types.SqlGeometry.Parse(wkt);
                    // Converts an invalid SqlGeometry instance into a SqlGeometry instance with a valid Open Geospatial Consortium (OGC) type.
                    geom = geom.MakeValid();

                    // solve error: // The element 'lokatie' in namespace 'http://www.egem.nl/StUF/sector/zkn/0310' has invalid child element 'MultiSurface' in namespace 'http://www.opengis.net/gml'. List of possible elements expected: '_Surface, Polygon, Surface, OrientableSurface, CompositeSurface, PolyhedralSurface, TriangulatedSurface, Tin' in namespace 'http://www.opengis.net/gml'
                    var validGeometryTypes = new List<string>() { "_Surface", "Polygon", "Surface", "OrientableSurface", "CompositeSurface", "PolyhedralSurface", "TriangulatedSurface", "Tin" };
                    var geometryType = geom.STGeometryType();
                    if (validGeometryTypes.Contains(geometryType.Value))
                    {
                        var gmlstring = geom.AsGml().Value;
                        var doc = new System.Xml.XmlDocument();
                        doc.LoadXml(gmlstring);
                        var documentnode = doc.DocumentElement;
                        documentnode.SetAttribute("srsDimension", "2");
                        documentnode.SetAttribute("srsName", "urn:ogc:def:crs:EPSG::28992");
                        return documentnode.OuterXml;
                    }
                    var msg = "ERROR: cannot create a valid GML geom from the given geomtype:" + geometryType.Value + " in processid:" + Procesid + " bij zaaktype:" + ZaaktypeOmschrijving + "(" + ZaaktypeCode + ")";
                    System.Diagnostics.Debug.WriteLine(msg);
                    Console.WriteLine(msg);

                    return null;
                }
            }
            //set {
            //    Row["lokatiePolygon"] = value;
            //}
        }
        public string MedewerkerIdentificatie
        {
            get { return Convert.ToString(Row["MedewerkerIdentificatie"]); }
            //set { Row["MedewerkerIdentificatie"] = value; }
        }

        public void Validate()
        {
            if (DBNull.Value.Equals(Procesid)) throw new ArgumentNullException("veld Procesid was niet aangeleverd");
            if (DBNull.Value.Equals(ZaaktypeCode)) throw new ArgumentNullException("veld ZaaktypeCode was niet aangeleverd voor procesid:" + Procesid);
            if (DBNull.Value.Equals(ZaakstatusCode)) throw new ArgumentNullException("veld ZaakstatusCode was niet aangeleverd voor procesid:" + Procesid);
            if (DBNull.Value.Equals(StartDatum)) throw new ArgumentNullException("veld Startdatum was niet aangeleverd voor procesid:" + Procesid);
            if (DBNull.Value.Equals(RegistratieDatum)) throw new ArgumentNullException("veld Registratiedatum was niet aangeleverd voor procesid:" + Procesid);

            if (ZaakOmschrijving.Length > 80) throw new ArgumentOutOfRangeException("zaak.ZaakOmschrijving length > 80 :" + ZaakOmschrijving + " voor procesid:" + Procesid);
            // some checks on errors we encountered in de xml during transmission
            // leuk dat je hier geen foutmeldingen van krijgt overigens,...
            if (ZaakstatusCode.Length > 10) throw new ArgumentOutOfRangeException("zaak.ZaakstatusCode length > 10 :" + ZaakstatusCode + " voor procesid:" + Procesid);
            if (ZaakstatusOmschrijving.Length > 80) throw new ArgumentOutOfRangeException("zaak.ZaakstatusOmschrijving length > 80 :" + ZaakstatusOmschrijving + " voor procesid:" + Procesid);
            //ZaakstatusOmschrijving = ZaakstatusOmschrijving.Substring(0, 10);


            // Controleer of alle velden gedefinieerd zijn!
            var o = Timestamp;
            o = Procesid;
            o = ZaaktypeCode;
            o = ZaaktypeOmschrijving;
            o = ZaakId;
            o = ZaakOmschrijving;
            o = ZaakToelichting;
            o = ZaakstatusCode;
            o = ZaakstatusOmschrijving;
            o = StartDatum;
            o = RegistratieDatum;
            o = PublicatieDatum;
            o = GeplandeeindDatum;
            o = UiterlijkeeindDatum;
            o = EindDatum;
            o = Kanaalcode;
            o = ResultaatCode;
            o = ResultaatOmschrijving;
//            o = ResultaatToelichting;
            o = AanvragerNpsNaam;
            o = AanvragerNpsBsn;
            o = AanvragerNpsGeslachtsnaam;
            o = AanvragerNpsVoorvoegsel;
            o = AanvragerNpsVoorletters;
            o = AanvragerNpsVoornamen;
            o = AanvragerNpsGeslachtsaanduiding;
            o = AanvragerNpsGeboortedatum;
            o = AanvragerNnpRsin;
            o = AanvragerNnpStatutaireNaam;
            o = AanvragerVesVestigingsNummer;
            o = AanvragerVesHandelsnaam;
            o = lokatiePolygon;
            o = MedewerkerIdentificatie;
        }

        public bool Dirty(Zaak compare)
        {
            if (compare == null) throw new ArgumentNullException();
            // do not compare the zaakid!
            if (this.ZaakId != compare.ZaakId && this.ZaakId != "" && compare.ZaakId != "") throw new Exception("Zaakid may never change!");

            if(this.Procesid != compare.Procesid) throw new Exception("are you sure you want to compare this zaak with the other?");

            //if(this.Timestamp	!= compare.Timestamp	) return true;
            if (this.ZaaktypeCode != compare.ZaaktypeCode)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " ZaaktypeCode: '" + this.ZaaktypeCode + "' <> '" + compare.ZaaktypeCode + "'");
                return true;
            }
            if(this.ZaaktypeOmschrijving	!= compare.ZaaktypeOmschrijving	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " ZaaktypeOmschrijving: '" + this.ZaaktypeOmschrijving + "' <> '" + compare.ZaaktypeOmschrijving + "'");
                return true;
            }
            // if(this.ZaakId	!= compare.ZaakId	) return true;
            if(this.ZaakOmschrijving	!= compare.ZaakOmschrijving	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " ZaakOmschrijving: '" + this.ZaakOmschrijving + "' <> '" + compare.ZaakOmschrijving + "'");
                return true;
            }
            if (this.ZaakToelichting != compare.ZaakToelichting)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " ZaakToelichting: '" + this.ZaakToelichting + "' <> '" + compare.ZaakToelichting + "'");
                return true;
            }
            if(this.ResultaatCode != compare.ResultaatCode)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " ResultaatCode: '" + this.ResultaatCode + "' <> '" + compare.ResultaatCode + "'");
                return true;
            }
            if(this.ResultaatOmschrijving	!= compare.ResultaatOmschrijving	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " ResultaatOmschrijving: '" + this.ResultaatOmschrijving + "' <> '" + compare.ResultaatOmschrijving + "'");
                return true;
            }

//            if(this.ResultaatToelichting	!= compare.ResultaatToelichting	) return true;
            if(this.ZaakstatusCode	!= compare.ZaakstatusCode	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " ZaakstatusCode: '" + this.ZaakstatusCode + "' <> '" + compare.ZaakstatusCode + "'");
                return true;
            }
            if(this.ZaakstatusOmschrijving	!= compare.ZaakstatusOmschrijving	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " ZaakstatusOmschrijving: '" + this.ZaakstatusOmschrijving + "' <> '" + compare.ZaakstatusOmschrijving + "'");
                return true;
            }
            if(this.StartDatum	!= compare.StartDatum	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " StartDatum: '" + this.StartDatum + "' <> '" + compare.StartDatum + "'");
                return true;
            }
            if(this.RegistratieDatum	!= compare.RegistratieDatum	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " RegistratieDatum: '" + this.RegistratieDatum + "' <> '" + compare.RegistratieDatum + "'");
                return true;
            }
            if(this.PublicatieDatum	!= compare.PublicatieDatum	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " PublicatieDatum: '" + this.PublicatieDatum + "' <> '" + compare.PublicatieDatum + "'");
                return true;
            }
            if(this.GeplandeeindDatum	!= compare.GeplandeeindDatum	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " GeplandeeindDatum: '" + this.GeplandeeindDatum + "' <> '" + compare.GeplandeeindDatum + "'");
                return true;
            }
            if(this.UiterlijkeeindDatum	!= compare.UiterlijkeeindDatum	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " UiterlijkeeindDatum: '" + this.UiterlijkeeindDatum + "' <> '" + compare.UiterlijkeeindDatum + "'");
                return true;
            }
            if(this.EindDatum	!= compare.EindDatum	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " EindDatum: '" + this.EindDatum + "' <> '" + compare.EindDatum + "'");
                return true;
            }
            if(this.AanvragerNpsNaam	!= compare.AanvragerNpsNaam	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNpsNaam: '" + this.AanvragerNpsNaam + "' <> '" + compare.AanvragerNpsNaam + "'");
                return true;
            }
            if(this.AanvragerNpsBsn	!= compare.AanvragerNpsBsn	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNpsBsn: '" + this.AanvragerNpsBsn + "' <> '" + compare.AanvragerNpsBsn + "'");
                return true;
            }
            if(this.AanvragerNpsGeslachtsnaam	!= compare.AanvragerNpsGeslachtsnaam	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNpsGeslachtsnaam: '" + this.AanvragerNpsGeslachtsnaam + "' <> '" + compare.AanvragerNpsGeslachtsnaam + "'");
                return true;
            } 
            if (this.AanvragerNpsVoorvoegsel != compare.AanvragerNpsVoorvoegsel) 
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNpsVoorvoegsel: '" + this.AanvragerNpsVoorvoegsel + "' <> '" + compare.AanvragerNpsVoorvoegsel + "'");
                return true;
            } 
            if(this.AanvragerNpsVoorletters	!= compare.AanvragerNpsVoorletters	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNpsVoorletters: '" + this.AanvragerNpsVoorletters + "' <> '" + compare.AanvragerNpsVoorletters + "'");
                return true;
            } 
            if(this.AanvragerNpsVoornamen	!= compare.AanvragerNpsVoornamen	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNpsVoornamen: '" + this.AanvragerNpsVoornamen + "' <> '" + compare.AanvragerNpsVoornamen + "'");
                return true;
            } 
            if(this.AanvragerNpsGeslachtsaanduiding	!= compare.AanvragerNpsGeslachtsaanduiding	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNpsGeslachtsaanduiding: '" + this.AanvragerNpsGeslachtsaanduiding + "' <> '" + compare.AanvragerNpsGeslachtsaanduiding + "'");
                return true;
            } 
            if(this.AanvragerNpsGeboortedatum	!= compare.AanvragerNpsGeboortedatum	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNpsGeboortedatum: '" + this.AanvragerNpsGeboortedatum + "' <> '" + compare.AanvragerNpsGeboortedatum + "'");
                return true;
            } 
            if (this.AanvragerNnpRsin != compare.AanvragerNnpRsin)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNnpRsin: '" + this.AanvragerNnpRsin + "' <> '" + compare.AanvragerNnpRsin + "'");
                return true;
            } 
            if (this.AanvragerNnpStatutaireNaam != compare.AanvragerNnpStatutaireNaam)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerNnpStatutaireNaam: '" + this.AanvragerNnpStatutaireNaam + "' <> '" + compare.AanvragerNnpStatutaireNaam + "'");
                return true;
            } 
            if (this.AanvragerVesVestigingsNummer != compare.AanvragerVesVestigingsNummer)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerVesVestigingsNummer: '" + this.AanvragerVesVestigingsNummer + "' <> '" + compare.AanvragerVesVestigingsNummer + "'");
                return true;
            } 
            if (this.AanvragerVesHandelsnaam != compare.AanvragerVesHandelsnaam)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " AanvragerVesHandelsnaam: '" + this.AanvragerVesHandelsnaam + "' <> '" + compare.AanvragerVesHandelsnaam + "'");
                return true;
            } 
            if (this.lokatiePolygon != compare.lokatiePolygon)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " lokatiePolygon: '" + this.lokatiePolygon + "' <> '" + compare.lokatiePolygon + "'");
                return true;
            } 
            if (this.MedewerkerIdentificatie	!= compare.MedewerkerIdentificatie	)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " MedewerkerIdentificatie: '" + this.MedewerkerIdentificatie + "' <> '" + compare.MedewerkerIdentificatie + "'");
                return true;
            } 
            if (this.Kanaalcode != compare.Kanaalcode)
            {
                System.Diagnostics.Debug.WriteLine("Is Dirty for zaak #" + Procesid + " Kanaalcode: '" + this.Kanaalcode + "' <> '" + compare.Kanaalcode + "'");
                return true;
            } 
            return false;
        }
    }
}