using DataDomain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessengerInfrastructure
{
	public class MessengerDbContext : IdentityDbContext<User>
	{
		public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
