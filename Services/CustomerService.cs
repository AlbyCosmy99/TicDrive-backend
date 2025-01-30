using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface ICustomerService
    {
        Task<List<FavoriteWorkshop>> GetFavoriteWorkshops(string? userId);
    }
    public class CustomerService : ICustomerService
    {
        private readonly TicDriveDbContext _dbContext;
        public CustomerService(TicDriveDbContext context)
        {
            _dbContext = context;
        }

        public async Task<List<FavoriteWorkshop>> GetFavoriteWorkshops(string? userId)
        {
            return await _dbContext.FavoriteWorkshops
                .Where(favoriteWorkshop => favoriteWorkshop.Customer.Id == userId)
                .ToListAsync();
        }
    }
}
