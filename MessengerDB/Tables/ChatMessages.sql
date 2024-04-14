CREATE TABLE [dbo].[ChatMessages]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ChatId] UNIQUEIDENTIFIER NOT NULL,
    [SenderId] NVARCHAR(450) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL,
    [Timestamp] DATETIME NOT NULL,
    [IsGroupChat] BIT NOT NULL DEFAULT 0,
    [GroupChatId] UNIQUEIDENTIFIER NULL,
    [UserChatId] UNIQUEIDENTIFIER NULL,

    CONSTRAINT FK_ChatMessages_UserChats 
        FOREIGN KEY (UserChatId) 
        REFERENCES UserChats(ChatId) 
        ON DELETE CASCADE,

    CONSTRAINT FK_ChatMessages_GroupChats 
        FOREIGN KEY (GroupChatId) 
        REFERENCES GroupChats(Id) 
        ON DELETE CASCADE
);
