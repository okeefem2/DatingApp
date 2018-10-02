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

    }
}