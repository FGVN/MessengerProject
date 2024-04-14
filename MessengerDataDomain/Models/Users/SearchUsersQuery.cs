using System;
using DataAccess.Models.Users;
using MediatR;

public class SearchUsersQuery : SearchQuery<UserMenuItemDTO>, IRequest<IEnumerable<object>>
{
}
