using DataDomain.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MessengerInfrastructure
{
	public class MessengerDbContext : DbContext
	{
		public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
			: base(options)
		{
		}

		// DbSet properties for your entities
		public DbSet<User> Users { get; set; }
		// Add DbSet properties for other entities as needed

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure entity mappings and relationships here
			// Example:
			// modelBuilder.Entity<User>().ToTable("Users");
		}
	}
}
