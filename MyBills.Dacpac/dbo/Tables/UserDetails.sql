CREATE TABLE [dbo].[UserDetails] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [FirstName]      NVARCHAR (50)   NULL,
    [ProfilePicture] VARBINARY (MAX) NULL,
    [UserId]         INT             NOT NULL,
    CONSTRAINT [PK_dbo.UserDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UserDetails_dbo.Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserDetails]([UserId] ASC);

