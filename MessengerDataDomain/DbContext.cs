using DataDomain.Users;
using MessengerDataAccess.Models.Chats;
using MessengerDataAccess.Models.Messages;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessengerInfrastructure
{
    public class MessengerDbContext : IdentityDbContext<User>
    {
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserChat> UserChats { get; set; }

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
