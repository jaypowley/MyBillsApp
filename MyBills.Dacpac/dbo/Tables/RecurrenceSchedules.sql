CREATE TABLE [dbo].[RecurrenceSchedules] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [RecurrenceTypeId] INT            NOT NULL,
    [Schedule]         NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_dbo.RecurrenceSchedules] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.RecurrenceSchedules_dbo.RecurrenceTypes_RecurrenceTypeId] FOREIGN KEY ([RecurrenceTypeId]) REFERENCES [dbo].[RecurrenceTypes] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_RecurrenceTypeId]
    ON [dbo].[RecurrenceSchedules]([RecurrenceTypeId] ASC);

