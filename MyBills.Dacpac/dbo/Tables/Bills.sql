CREATE TABLE [dbo].[Bills] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50)   NOT NULL,
    [Amount]     DECIMAL (18, 2) NOT NULL,
    [IsComplete] BIT             NOT NULL,
    [IsAutoPaid] BIT             NOT NULL,
    CONSTRAINT [PK_dbo.Bills] PRIMARY KEY CLUSTERED ([Id] ASC)
);

