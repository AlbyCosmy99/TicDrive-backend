using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly TicDriveDbContext _dbContext;
        private readonly IAuthService _authService;

        public StatisticsController(TicDriveDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }

        [HttpGet("dashboard")]
        [Authorize]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userClaims = _authService.GetUserClaims(this);
            var workshopId = _authService.GetUserId(userClaims);
            var userType = _authService.GetUserType(userClaims);

            if (userType != Enums.UserType.Workshop)
            {
                return Unauthorized("Only workshops can access statistics.");
            }

            var bookingsQuery = _dbContext.Bookings.AsQueryable();
            var reviewsQuery = _dbContext.Reviews.AsQueryable();

            if (!string.IsNullOrEmpty(workshopId))
            {
                bookingsQuery = bookingsQuery.Where(b => b.WorkshopId == workshopId);
                reviewsQuery = reviewsQuery.Where(r => r.WorkshopId == workshopId);
            }

            var uniqueClients = await bookingsQuery
                .Select(b => b.CustomerId)
                .Distinct()
                .CountAsync();

            var totalBookings = await bookingsQuery.CountAsync();

            var totalEarnings = await bookingsQuery
                .Where(b => b.Status == BookingType.Completed)
                .SumAsync(b => (decimal?)b.FinalPrice ?? 0);

            var totalReviews = await reviewsQuery.CountAsync();
            var averageRating = await reviewsQuery
                .Select(r => (double?)r.Stars)
                .AverageAsync() ?? 0.0;

            return Ok(new
            {
                uniqueClients,
                totalBookings,
                totalEarnings = Math.Round(totalEarnings, 2),
                totalReviews,
                averageRating = Math.Round(averageRating, 1)
            });
        }
    }
}
