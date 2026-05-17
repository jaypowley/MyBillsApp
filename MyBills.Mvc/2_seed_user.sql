USE [MyBills_Core]
GO

DECLARE @newUserId INT;

INSERT INTO [dbo].[Users]
           ([Username]
           ,[Email]
           ,[PasswordHash]
           ,[CreatedDate]
           ,[UpdatedDate])
     VALUES
           ('test@example.com'
           ,'test@example.com'
           ,'Rfc2898DeriveBytes$50000$qVB1DnBAYYvmKnD8H398kw==$qkHY1NLrTEjKUpBgXlfIFMjnAhjNClx3Y3iZFghEn7Q=' 
           ,GetDate()
           ,GetDate())

SET @newUserId = SCOPE_IDENTITY();

INSERT INTO [dbo].[UserDetails]
           ([FirstName]
           ,[ProfilePicture]
           ,[UserId])
     VALUES
           ('Test'
           ,NULL
           ,@newUserId)
GO
