using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DataAccess.Models.Users;
using DataDomain.Users;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace MessengerInfrastructure.Services.QueryHandlers
{
    public class UserMenuQueryHandler
    {
        private readonly UserManager<User> _userManager;

        public UserMenuQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserMenuItemDTO>> GetUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return users.Select(x => new UserMenuItemDTO { Username = x.UserName, Email = x.Email });
        }

        public async Task<IEnumerable<UserMenuItemDTO>> SearchUsersAsync(SearchUsersQuery query)
        {
            var validator = new SearchUsersQueryValidator(); // FluentValidation validator instance
            validator.ValidateAndThrow(query); // Validate query parameters

            var users = _userManager.Users.Where(x => x.UserName.Contains(query.Query)).ToList();

            // Sorting
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var propertyInfo = typeof(User).GetProperty(query.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    users = query.SortDirection.ToLower() == "asc" ?
                        users.OrderBy(u => propertyInfo.GetValue(u)).ToList() :
                        users.OrderByDescending(u => propertyInfo.GetValue(u)).ToList();
                }
                else
                {
                    throw new ArgumentException($"Invalid SortBy option: {query.SortBy}");
                }
            }

            // Pagination
            var from = query.From;
            var to = query.To < users.Count ? query.To : users.Count;
            return users.GetRange(from, to - from).Select(x => new UserMenuItemDTO { Username = x.UserName, Email = x.Email });
        }
    }
}
