using Microsoft.EntityFrameworkCore;
using TicDrive.Models;

namespace TicDrive.Context
{
    public class TicDriveDbContext : DbContext
    {
        public TicDriveDbContext(DbContextOptions<TicDriveDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
