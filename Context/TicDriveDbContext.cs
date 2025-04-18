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

            //Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            //Reviews
            modelBuilder.Entity<Review>()
                .HasOne(model => model.Customer)
                .WithMany()
                .HasForeignKey(model => model.CustomerId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<Review>()
             .HasOne(model => model.Workshop)
             .WithMany()
             .HasForeignKey(model => model.WorkshopId)
             .OnDelete(DeleteBehavior.NoAction)
             .IsRequired();

            //FavoriteWorkshops
            modelBuilder.Entity<FavoriteWorkshop>()
                .HasOne(model => model.Workshop)
                .WithMany()
                .HasForeignKey(model => model.WorkshopId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<FavoriteWorkshop>()
                .HasOne(model => model.Customer)
                .WithMany()
                .HasForeignKey(model => model.CustomerId)
                .OnDelete(DeleteBehavior.NoAction)
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

            modelBuilder
            .Entity<Car>() 
            .Property(e => e.TransmissionType)
            .HasConversion<string>();

            modelBuilder
           .Entity<Car>()
           .Property(e => e.FuelType)
           .HasConversion<string>();
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CarMake> CarMakes { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarModelVersion> CarModelVersions { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OfferedServices> OfferedServices { get; set;}
        public DbSet<FavoriteWorkshop> FavoriteWorkshops { get; set ; }
        public DbSet<CustomerCar> CustomerCars { get; set; }
    }
}
