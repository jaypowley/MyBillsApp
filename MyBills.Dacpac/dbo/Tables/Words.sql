CREATE TABLE [dbo].[Words] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Word] NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.Words] PRIMARY KEY CLUSTERED ([Id] ASC)
);

