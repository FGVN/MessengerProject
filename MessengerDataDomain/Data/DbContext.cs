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
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupChatMembership> GroupChatMemberships { get; set; }
        public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GroupChatMembership>()
                .HasKey(m => new { m.GroupId, m.UserId });

            modelBuilder.Entity<GroupChatMembership>()
                .HasOne(m => m.GroupChat)
                .WithMany(g => g.Members)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupChatMembership>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
