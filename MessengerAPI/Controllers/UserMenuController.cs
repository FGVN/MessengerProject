using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessengerInfrastructure.Services;
using DataAccess.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //move
    public class QueryController : ControllerBase
    {
        private readonly UserMenuQueryHandler _userMenuQueryHandler;

        public QueryController(UserMenuQueryHandler userMenuQueryHandler)
        {
            _userMenuQueryHandler = userMenuQueryHandler;
        }

        [HttpGet("users")]
        public async Task<IEnumerable<UserMenuItemDTO>> GetUsers()
        {
            var userId = User.FindFirst("nameid")?.Value;

            return await _userMenuQueryHandler.GetAllAsync();
        }

        [HttpPost("users/search")]
        public async Task<IEnumerable<object>> SearchUsers(SearchUsersQuery query)
        {
            var userId = User.FindFirst("nameid")?.Value;

            return await _userMenuQueryHandler.SearchAsync(query);
        }
    }
}