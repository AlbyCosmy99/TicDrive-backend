using Microsoft.EntityFrameworkCore;
using TicDrive.Models;

namespace TicDrive.Context
{
    public class TicDriveDbContext : DbContext
    {
        public TicDriveDbContext(DbContextOptions<TicDriveDbContext> options) : base(options) { }
        public DbSet<Service> Services { get; set; }
    }
}
