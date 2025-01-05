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

            modelBuilder.Entity<CarModel>()
                .HasOne(model => model.CarMake)
                .WithMany()
                .HasForeignKey(model => model.CarMakeId)
                .IsRequired();
            }

        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CarMake> CarMakes { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Car> Cars { get; set; }
    }
}
