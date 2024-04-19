using System;
using System.Threading.Tasks;
using DataDomain.Users;
using Microsoft.EntityFrameworkCore;

namespace DataDomain.Repositories
{
    public interface IUserCommandRepository : ICommandRepository<User>
	{
	}
}

