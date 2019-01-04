using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>> // Type config is needed because int is being used for IDs in this app... yuck, just use the default with string
    {
        // Constructor that just passes the options up to the class this one is inheriting from (super(options))
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}

        public DbSet<Value> Values { get; set; } // This property will be converted to a table
        public DbSet<Photo> Photos { get; set; } // This property will be converted to a table
        public DbSet<Like> Likes { get; set; } // This property will be converted to a table
        public DbSet<Message> Messages { get; set; } // This property will be converted to a table

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId});
            builder.Entity<UserRole>().HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired();
            builder.Entity<UserRole>().HasOne(ur => ur.User).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired();

            builder.Entity<Like>().HasKey(l => new {l.LikerId, l.LikeeId}); // Create compound key here 
            builder.Entity<Like>().HasOne(l => l.Liker).WithMany(u => u.Likees).HasForeignKey(l => l.LikerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>().HasOne(l => l.Likee).WithMany(u => u.Likers).HasForeignKey(l => l.LikeeId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>().HasOne(m => m.Sender).WithMany(u => u.MessagesSent).HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Message>().HasOne(m => m.Recipient).WithMany(u => u.MessagesReceived).HasForeignKey(m => m.RecipientId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}