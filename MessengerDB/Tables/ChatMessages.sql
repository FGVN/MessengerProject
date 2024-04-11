CREATE TABLE [dbo].[ChatMessages]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ChatId] UNIQUEIDENTIFIER NOT NULL,
    [SenderId] NVARCHAR(450) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL,
    [Timestamp] DATETIME NOT NULL,
    CONSTRAINT FK_ChatMessages_UserChats FOREIGN KEY (ChatId) REFERENCES UserChats(ChatId),
    CONSTRAINT FK_ChatMessages_AspNetUsers FOREIGN KEY (SenderId) REFERENCES AspNetUsers(Id)
)

