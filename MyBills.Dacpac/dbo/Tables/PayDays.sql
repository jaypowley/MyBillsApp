CREATE TABLE [dbo].[PayDays] (
    [Id]     INT             IDENTITY (1, 1) NOT NULL,
    [Name]   NVARCHAR (50)   NOT NULL,
    [Amount] DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.PayDays] PRIMARY KEY CLUSTERED ([Id] ASC)
);

