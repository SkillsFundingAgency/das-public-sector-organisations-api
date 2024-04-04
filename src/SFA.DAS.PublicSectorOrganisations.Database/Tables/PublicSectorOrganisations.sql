CREATE TABLE dbo.[PublicSectorOrganisation] (
    [Id]                uniqueidentifier	NOT NULL,
    [Name]	            nvarchar(350)	    NOT NULL,
    [Source]	        smallint        	NOT NULL,
    [AddressLine1]		nvarchar(150)	    NULL,
    [AddressLine2]		nvarchar(150)	    NULL,
    [AddressLine3]		nvarchar(150)	    NULL,
    [Town]		        nvarchar(150)	    NULL,
    [PostCode]			nvarchar(8)		    NULL,
    [Country]   	    nvarchar(150)	    NULL,
    [UPRN]	            nvarchar(20)		NULL,
    [OrganisationCode]	nvarchar(20)		NULL,
    [Active]	        bit         		NOT NULL,
    CONSTRAINT [PK_PublicSectorOrganisation_Id] PRIMARY KEY (Id),
    INDEX [IX_PublicSectorOrganisation_Name] NONCLUSTERED(Name)
    )