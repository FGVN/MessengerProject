CREATE TABLE [dbo].[UserChats]
(
    [ChatId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [ContactUserId] NVARCHAR(450) NOT NULL,
    CONSTRAINT FK_UserChats_AspNetUsers FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_UserChats_ContactAspNetUsers FOREIGN KEY (ContactUserId) REFERENCES AspNetUsers(Id)
);
