CREATE TABLE [dbo].[zaken](
	[timestamp] [datetime] NULL,
	[procesid] [nvarchar](125) NOT NULL,
	[zaaktypecode] [nvarchar](125) NOT NULL,
	[zaaktypeomschrijving] [nvarchar](255) NULL,
	[zaakid] [nvarchar](255) NULL,
	[zaakomschrijving] [nvarchar](255) NULL,
	[zaaktoelichting] [nvarchar](max) NULL,
	[resultaatcode] [nvarchar](255) NULL,
	[resultaatomschrijving] [nvarchar](255) NULL,
	[zaakstatuscode] [nvarchar](255) NOT NULL,
	[zaakstatusomschrijving] [nvarchar](255) NULL,
	[startdatum] [nvarchar](255) NULL,
	[registratiedatum] [nvarchar](255) NULL,
	[PUBLICATIEDATUM] [nvarchar](255) NULL,
	[geplandeeinddatum] [nvarchar](255) NULL,
	[uiterlijkeeinddatum] [nvarchar](255) NULL,
	[einddatum] [nvarchar](255) NULL,
	[AanvragerNpsNaam] [nvarchar](255) NULL,
	[AanvragerNpsBsn] [nvarchar](255) NULL,
	[AanvragerNpsGeslachtsnaam] [nvarchar](255) NULL,
	[AanvragerNpsVoorvoegsel] [nvarchar](255) NULL,
	[AanvragerNpsVoorletters] [nvarchar](255) NULL,
	[AanvragerNpsVoornamen] [nvarchar](255) NULL,
	[AanvragerNpsGeslacht] [nvarchar](255) NULL,
	[AanvragerNpsGeboortedatum] [nvarchar](255) NULL,
	[medewerkerIdentificatie] [nvarchar](255) NULL,
	[Kanaalcode] [nvarchar](255) NULL,
	[aanvragerNnpRsin] [nvarchar](255) NULL,
	[aanvragerNnpStatutaireNaam] [nvarchar](255) NULL,
	[aanvragerVesVestigingsNummer] [nvarchar](255) NULL,
	[aanvragerVesHandelsnaam] [nvarchar](255) NULL,
	[lokatiePolygon] [nvarchar](max) NULL,
 CONSTRAINT [PK_zaken] PRIMARY KEY CLUSTERED 
(
	[procesid] ASC,
	[zaaktypecode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;
CREATE TABLE [dbo].[berichten](
	[berichtid] [int] IDENTITY(1,1) NOT NULL,
	[timestamp] [datetime] NULL,
	[kenmerk] [nvarchar](255) NULL,
	[url] [nvarchar](255) NULL,
	[action] [nvarchar](255) NULL,
	[requestbody] [nvarchar](max) NULL,
	[responsecode] [nvarchar](255) NULL,
	[responsebody] [nvarchar](max) NULL,
 CONSTRAINT [PK_berichten] PRIMARY KEY CLUSTERED 
(
	[berichtid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;