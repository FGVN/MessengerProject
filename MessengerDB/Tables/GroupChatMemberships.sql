CREATE TABLE [dbo].[GroupChatMemberships]
(
    [GroupId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    CONSTRAINT PK_GroupChatMemberships PRIMARY KEY (GroupId, UserId),
    CONSTRAINT FK_GroupChatMemberships_GroupChats FOREIGN KEY (GroupId) REFERENCES GroupChats(Id) ON DELETE CASCADE,
    CONSTRAINT FK_GroupChatMemberships_AspNetUsers FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
)
