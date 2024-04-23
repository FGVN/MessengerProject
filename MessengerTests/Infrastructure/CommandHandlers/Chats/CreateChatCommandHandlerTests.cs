﻿using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using MessengerInfrastructure.Commands;
using MessengerInfrastructure.QueryHandlers;
using System.Linq.Expressions;

namespace MessengerInfrastructure.Tests;

public class CreateChatCommandHandlerTests
{
    [Fact]
    public async Task Handle_SenderAndContactUserExist_CreatesNewChat()
    {
        // Arrange
        var senderId = Guid.NewGuid().ToString();
        var contactUsername = "testUser";
        var user = new User { Id = senderId };
        var contactUser = new User { Id = Guid.NewGuid().ToString(), UserName = contactUsername };

        var userRepositoryMock = new Mock<IRepository<User>>(); 
        userRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
                  .ReturnsAsync(new List<User> { user }.AsQueryable());


        userRepositoryMock.Setup(repo => repo.GetAllAsync(u => u.UserName == contactUsername))
                          .ReturnsAsync(new List<User> { contactUser }.AsQueryable());

        var userChatRepositoryMock = new Mock<IRepository<UserChat>>();
        userChatRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<UserChat, bool>>>()))
                      .ReturnsAsync(Enumerable.Empty<UserChat>().AsQueryable());


        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<User>()).Returns(userRepositoryMock.Object);
        unitOfWorkMock.Setup(uow => uow.GetRepository<UserChat>()).Returns(userChatRepositoryMock.Object);

        var handler = new CreateChatCommandHandler(unitOfWorkMock.Object);
        var command = new CreateChatCommand (senderId, contactUsername);

        // Act
        var chatId = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<Guid>(chatId);
    }

    [Fact]
    public async Task Handle_SenderNotFound_ThrowsException()
    {
        // Arrange
        var senderId = Guid.NewGuid().ToString();
        var contactUsername = "testUser";

        var userRepositoryMock = new Mock<IRepository<User>>();
        userRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
                          .ReturnsAsync(Enumerable.Empty<User>().AsQueryable());

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<User>()).Returns(userRepositoryMock.Object);

        var handler = new CreateChatCommandHandler(unitOfWorkMock.Object);
        var command = new CreateChatCommand (senderId, contactUsername);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ContactUserNotFound_ThrowsException()
    {
        // Arrange
        var senderId = Guid.NewGuid().ToString();
        var contactUsername = "testUser";
        var user = new User { Id = senderId };

        var userRepositoryMock = new Mock<IRepository<User>>();
        userRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
                          .ReturnsAsync(new List<User> { user }.AsQueryable());

        userRepositoryMock.Setup(repo => repo.GetAllAsync(u => u.UserName == contactUsername))
                          .ReturnsAsync(Enumerable.Empty<User>().AsQueryable());

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<User>()).Returns(userRepositoryMock.Object);

        var handler = new CreateChatCommandHandler(unitOfWorkMock.Object);
        var command = new CreateChatCommand (senderId, contactUsername );

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ChatAlreadyExists_ThrowsException()
    {
        // Arrange
        var senderId = Guid.NewGuid().ToString();
        var contactUsername = "testUser";
        var user = new User { Id = senderId };
        var contactUser = new User { Id = Guid.NewGuid().ToString(), UserName = contactUsername };
        var existingChat = new UserChat { UserId = senderId, ContactUserId = contactUser.Id };

        var userRepositoryMock = new Mock<IRepository<User>>();
        userRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
                          .ReturnsAsync(new List<User> { user }.AsQueryable());

        userRepositoryMock.Setup(repo => repo.GetAllAsync(u => u.UserName == contactUsername))
                          .ReturnsAsync(new List<User> { contactUser }.AsQueryable());

        var userChatRepositoryMock = new Mock<IRepository<UserChat>>();
        userChatRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<UserChat, bool>>>()))
                              .ReturnsAsync(new List<UserChat> { existingChat }.AsQueryable());

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<User>()).Returns(userRepositoryMock.Object);
        unitOfWorkMock.Setup(uow => uow.GetRepository<UserChat>()).Returns(userChatRepositoryMock.Object);

        var handler = new CreateChatCommandHandler(unitOfWorkMock.Object);
        var command = new CreateChatCommand (senderId, contactUsername);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }
}
