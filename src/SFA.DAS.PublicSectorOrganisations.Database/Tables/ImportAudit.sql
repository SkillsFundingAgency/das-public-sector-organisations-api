CREATE TABLE [dbo].[ImportAudit]
(
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[TimeStarted] DATETIME2 NOT NULL,
	[TimeFinished] DATETIME2 NOT NULL,
	[RowsAdded] INT NOT NULL,
	[RowsUpdated] INT NOT NULL,
	[Source] TINYINT NOT NULL
)
GO
