using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicDrive.Dto.FavoriteWorkshopDto;
using TicDrive.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWorkshopsService _workshopsService;
        private readonly IMapper _mapper;
        public CustomerController(IAuthService authService, IWorkshopsService workshopsService, IMapper mapper)
        {
            _authService = authService;
            _workshopsService = workshopsService;
            _mapper = mapper;
        }
        public class GetFavoriteWorkshopsQueries
        {
            public int Skip { get; set; } = 0;
            public int Take { get; set; } = 10;
            public string? Filter { get; set; } = string.Empty;
            public string Order { get; set; } = "asc";
        }

        [Route("workshops/favorite")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFavoriteWorkshops([FromQuery] GetFavoriteWorkshopsQueries query)
        {
            int skip = query.Skip;
            int take = query.Take;

            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            var favoriteWorkshops = await _workshopsService.GetWorkshops(skip, take, customerId: userId, favorite: true, filter: query.Filter);

            switch (query.Order?.ToLower())
            {
                case "desc":
                    favoriteWorkshops = favoriteWorkshops.OrderByDescending(w => w.Name);
                    break;
                case "asc":
                default:
                    favoriteWorkshops = favoriteWorkshops.OrderBy(w => w.Name);
                    break;
            }
            return Ok(new { workshops = favoriteWorkshops, count = favoriteWorkshops.Count() });

        }

        [Route("workshops/favorite")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikeWorkshop([FromQuery] string workshopId)
        {
            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);
            await _workshopsService.LikeWorkshop(userId, workshopId);

            return NoContent();

        }
    }
}
