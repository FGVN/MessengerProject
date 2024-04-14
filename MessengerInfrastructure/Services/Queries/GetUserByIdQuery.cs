using DataAccess.Models.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerInfrastructure.Services
{
    public class GetUserByIdQuery : IRequest<UserMenuItemDTO>
    {
        public string UserId { get; }

        public GetUserByIdQuery(string userId)
        {
            UserId = userId;
        }
    }

}
