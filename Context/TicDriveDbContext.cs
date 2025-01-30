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

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Longitude)
                    .HasPrecision(18, 6);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Latitude)
                    .HasPrecision(18, 6);
            });
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CarMake> CarMakes { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OfferedServices> OfferedServices { get; set;}
        public DbSet<FavoriteWorkshop> FavoriteWorkshops { get; set ; }
    }
}
