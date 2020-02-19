CREATE TABLE [ent].[SqlLog] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Value]       NVARCHAR (MAX) NULL,
    [CreatedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_ent.SqlLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

