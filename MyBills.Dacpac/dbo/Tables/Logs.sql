CREATE TABLE [dbo].[Logs] (
    [LogId]         INT             IDENTITY (1, 1) NOT NULL,
    [LogLevel]      INT             NOT NULL,
    [TimeStamp]     DATETIME        NOT NULL,
    [CurrentMethod] NVARCHAR (100)  NULL,
    [ErrorMessage]  NVARCHAR (4000) NULL,
    [StackTrace]    NVARCHAR (MAX)  NULL,
    [UserName]      NVARCHAR (50)   NULL,
    CONSTRAINT [PK_dbo.Logs] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

