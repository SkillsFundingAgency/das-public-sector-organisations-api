CREATE TABLE dbo.[NhsStaging] (
    [Name]	            nvarchar(350)	    NOT NULL,
    [AddressLine1]		nvarchar(150)	    NULL,
    [AddressLine2]		nvarchar(150)	    NULL,
    [AddressLine3]		nvarchar(150)	    NULL,
    [Town]		        nvarchar(150)	    NULL,
    [PostCode]			nvarchar(8)		    NULL,
    [Country]   	    nvarchar(150)	    NULL,
    [UPRN]	            nvarchar(20)		NULL,
    [OrganisationCode]	nvarchar(20)		NULL,
    )