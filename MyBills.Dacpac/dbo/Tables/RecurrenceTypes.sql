CREATE TABLE [dbo].[RecurrenceTypes] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50)  NOT NULL,
    [Type] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_dbo.RecurrenceTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT UQ_Name UNIQUE([Name])
);

