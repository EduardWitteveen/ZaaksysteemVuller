~ Midoffice Vuller ~

[Doel]
Het ontsluiten van backoffice-proces-informatie naar het klantcontactsysteem en mijn.overheid.nl (http://demo.mijn.overheid.nl/)
Daarnaast transparant maken en daarmee kunnen monitoren van de afhandeling van deze processen.

[Werking]
Een service die ervoor zorgt dat de processen van een willekeurige backoffice applicatie worden gesynchroniseerd naar zaken in het zaaksysteem.
Het is hiervoor is het alleen nodig om een sql aanmaken die de processen van de backoffice uitlijst.
Deze processen worden dan als zaken aangemaakt in het zaaksysteem (middels stuf-zkn-dms)
De service zal zelf detecteren of er wijzigingen plaatsvinden processen (bijvoorbeeld in de status) en deze wijzigingen ook naar het zaaksysteem synchroniseren

Een concreet voorbeeld van een toepassing van deze generieke oplossing is: 
- In key2Burgerzaken wordt gewoon de rijbewijsaanvragen, paspoortaanvragen, identiteitskaarten en verhuizingen verwerkt (we hoeven niets dubbel te registreren)
- De MidofficeVuller-service kijkt of er in de procesgang van de bovenstaande processen iets is veranderd (middels sql wordt de lijst van onderhande werk met status, aanvrager en behandelaar gemaakt)
- De MidofficeVuller maakt/update de relevante zaak in het zaaksysteem, indien er wijzigingen noodzakelijk zijn.

[Info]
Koppelingen zijn terug te vinden in: .\koppeling\ (of na contact met de auteur)
Een beschrijving van de noodzakelijke velden die gebruikt moeten worden, staan in het bestand .\doc\mapping.xlsx
Hoe je een nieuw zaaktype moet toevoegen staat in .\doc\key2zaken.docx

[Gerealiseerd]
- 	DEMO:	Voor het testen/debuggen
-	GBA: 	Indentiteitsbewijs/paspoort	(in productie)
-	GBA: 	Rijbewijs
-	GBA: 	BRP verhuizing aangifte
-	SOZA: 	Betaling
-	Corsa: 	Telefoonnotities

[Planning]
-	GISVG: Vergunningen (met Geo)
-	Corsa: Brieven
-	Corsa: Mor meldingen
-	Corsa: Mol meldingen