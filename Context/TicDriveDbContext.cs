using Microsoft.EntityFrameworkCore;
using TicDrive.Models;
using TicDrive.Models.DateTime;
using TicDrive.Models.Legal;
using TicDrive.Models.Log;
using TicDrive.Models.Workshops;

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

            //Car
            modelBuilder
                .Entity<Car>() 
                .Property(e => e.TransmissionType)
                .HasConversion<string>();

            modelBuilder
               .Entity<Car>()
               .Property(e => e.FuelType)
               .HasConversion<string>();

            //LegalDeclaration
            modelBuilder
                .Entity<LegalDeclaration>()
                .Property(ld => ld.Type)
                .HasConversion<string>();

            modelBuilder
                .Entity<LegalDeclaration>()
                .Property(ld => ld.Context)
                .HasConversion<string>();
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceTranslation> ServicesTranslations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CarMake> CarMakes { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarModelVersion> CarModelVersions { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OfferedServices> OfferedServices { get; set;}
        public DbSet<FavoriteWorkshop> FavoriteWorkshops { get; set ; }
        public DbSet<CustomerCar> CustomerCars { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<WorkshopDetails> WorkshopsDetails { get; set; }
        public DbSet<WorkshopNonWorkingDay> WorkshopsNonWorkingDays { get; set; }
        public DbSet<WorkshopSchedule> WorkshopsSchedules { get; set; }
        public DbSet<WorkshopSpecialization>  WorkshopsSpecializations { get; set; }
        public DbSet<LegalDeclaration> LegalDeclarations { get; set; } 
        public DbSet<UserConsent> UserConsents { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<UserSpokenLanguage> SpokenLanguages { get; set; }
        public DbSet<DayTranslation> DaysTranslations { get; set; }

        //logs
        public DbSet<LoginLog> LoginLogs { get; set; }
    }
}
