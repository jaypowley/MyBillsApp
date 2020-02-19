CREATE TABLE [dbo].[UserBills] (
    [Id]                   INT IDENTITY (1, 1) NOT NULL,
    [BillId]               INT NOT NULL,
    [Day]                  INT NOT NULL,
    [Month]                INT NOT NULL,
    [Year]                 INT NOT NULL,
    [IsPaid]               BIT NOT NULL,
    [UserId]               INT NOT NULL,
    [RecurrenceScheduleId] INT NOT NULL,
    CONSTRAINT [PK_dbo.UserBills] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UserBills_dbo.Bills_BillId] FOREIGN KEY ([BillId]) REFERENCES [dbo].[Bills] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UserBills_dbo.RecurrenceSchedules_RecurrenceScheduleId] FOREIGN KEY ([RecurrenceScheduleId]) REFERENCES [dbo].[RecurrenceSchedules] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UserBills_dbo.Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_BillId]
    ON [dbo].[UserBills]([BillId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserBills]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RecurrenceScheduleId]
    ON [dbo].[UserBills]([RecurrenceScheduleId] ASC);

