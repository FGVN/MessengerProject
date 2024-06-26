﻿using DataAccess;
using MediatR;
using DataAccess.Models;
using MessengerInfrastructure.Commands;

namespace MessengerInfrastructure.CommandHandlers;

public class JoinGroupChatCommandHandler : IRequestHandler<JoinGroupChatCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public JoinGroupChatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(JoinGroupChatCommand request, CancellationToken cancellationToken)
    {
        var membership = new GroupChatMembership
        {
            GroupId = request.GroupChatId,
            UserId = request.UserId
        };

        var membershipRepository = _unitOfWork.GetRepository<GroupChatMembership>();
        await membershipRepository.AddAsync(membership);
        await _unitOfWork.SaveChangesAsync();
    }
}
