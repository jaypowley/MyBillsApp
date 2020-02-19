CREATE TABLE [dbo].[Users] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Username]     NVARCHAR (50)  NOT NULL,
    [Email]        NVARCHAR (50)  NOT NULL,
    [PasswordHash] NVARCHAR (100) NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [UpdatedDate]  DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT UQ_Username UNIQUE(Username),
    CONSTRAINT UQ_Email UNIQUE(Email)
);