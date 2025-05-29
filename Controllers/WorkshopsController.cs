using Microsoft.AspNetCore.Mvc;
using TicDrive.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicDrive.Dto.UserDto;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkshopsController : ControllerBase
    {
        private readonly IWorkshopsService _workshopsService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public WorkshopsController(IWorkshopsService workshopsService, IAuthService authService, IMapper mapper)
        {
            _workshopsService = workshopsService;
            _authService = authService;
            _mapper = mapper;
        }

        public class GetWorkshopsQueries
        {
            public int Skip { get; set; } = 0;
            public int Take { get; set; } = 10;
            public int ServiceId { get; set; } = 0;
            public string? Filter { get; set; } = string.Empty;
            public string Order { get; set; } = "asc";
            public string? WorkshopId { get; set; } = null;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetWorkshops([FromQuery] GetWorkshopsQueries query)
        {
            int skip = query.Skip;
            int take = query.Take;
            int serviceId = query.ServiceId;

            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            try
            {
                var workshops = await _workshopsService.GetWorkshops(serviceId, userId, query.WorkshopId, null, null, false, filter: query.Filter);

                switch (query.Order?.ToLower())
                {
                    case "desc":
                        workshops = workshops.OrderByDescending(w => w.Name);
                        break;
                    case "asc":
                    default:
                        workshops = workshops.OrderBy(w => w.Name);
                        break;
                }

                var pagedWorkshops = workshops.Skip(skip).Take(take).ToList();

                if(query?.WorkshopId != null)
                {
                    return Ok(new { workshop = pagedWorkshops.FirstOrDefault(), Count = pagedWorkshops.Count() > 0 ? 1 : 0 });
                } else
                {
                    return Ok(new { workshops = pagedWorkshops, Count = workshops.Count() });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        public class GetNearbyWorkshopsQuery
        {
            public decimal? Latitude { get; set; }
            public decimal? Longitude { get; set; }
            public int? KmRange { get; set; }
            public int? ServiceId { get; set; }
            public int Skip { get; set; } = 0;
            public int Take { get; set; } = 10;
            public string? Filter { get; set; } = string.Empty;
            public string Order { get; set; } = "asc";
            public string? WorkshopId { get; set; } = null;
        }

        [HttpGet]
        [Route("nearby")]
        public async Task<IActionResult> GetNearbyWorkshops([FromQuery] GetNearbyWorkshopsQuery query)
        {
            if (query == null || query.Latitude == null || query.Longitude == null)
            {
                return BadRequest("Latitude and longitude are required.");
            }

            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            var nearbyWorkshops = await _workshopsService.GetWorkshops(
                query.ServiceId,
                userId,
                query.WorkshopId,
                (decimal)query.Latitude,
                (decimal)query.Longitude,
                false,
                query.Filter,
                query.KmRange
            );

            switch (query.Order?.ToLower())
            {
                case "desc":
                    nearbyWorkshops = nearbyWorkshops.OrderByDescending(w => w.Name);
                    break;
                case "asc":
                default:
                    nearbyWorkshops = nearbyWorkshops.OrderBy(w => w.Name);
                    break;
            }

            var pagedNearbyWorkshops = nearbyWorkshops.Skip(query.Skip).Take(query.Take).ToList();

            if(query?.WorkshopId != null)
            {
                return Ok(new { nearbyWorkshop = pagedNearbyWorkshops.FirstOrDefault(), Count = nearbyWorkshops.Count() > 0 ? 1 : 0 });
            }
            else
            {
                return Ok(new { nearbyWorkshops = pagedNearbyWorkshops, Count = nearbyWorkshops.Count() });
            }
        }

        [HttpGet]
        [Route("specializations")]
        public IActionResult GetSpecializations()
        {
            return Ok(_workshopsService.GetSpecializations());
        }
    }
}
