using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        // Constructor that just passes the options up to the class this one is inheriting from (super(options))
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}

        public DbSet<Value> Values { get; set; } // This property will be converted to a table
        public DbSet<User> Users { get; set; } // This property will be converted to a table
        public DbSet<Photo> Photos { get; set; } // This property will be converted to a table
        public DbSet<Like> Likes { get; set; } // This property will be converted to a table

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>().HasKey(l => new {l.LikerId, l.LikeeId}); // Create compound key here 
            builder.Entity<Like>().HasOne(l => l.Liker).WithMany(u => u.Likees).HasForeignKey(l => l.LikerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>().HasOne(l => l.Likee).WithMany(u => u.Likers).HasForeignKey(l => l.LikeeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}