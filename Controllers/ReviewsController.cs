using Microsoft.AspNetCore.Mvc;
using TicDrive.Services;
using System.Threading.Tasks;
using AutoMapper;
using TicDrive.Dto.ReviewDto;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;
        private readonly IMapper _mapper;

        public ReviewsController(IReviewsService reviewsService, IMapper mapper)
        {
            _reviewsService = reviewsService;
            _mapper = mapper;
        }

        public class GetReviewsQuery
        {
            public int Skip { get; set; } = 0;
            public int Take { get; set; } = 10;
        }

        [HttpGet("{workshopId}")]
        public async Task<IActionResult> GetReviewsByWorkshopId(string workshopId, [FromQuery] GetReviewsQuery query)
        {
            if (string.IsNullOrWhiteSpace(workshopId))
            {
                return BadRequest("WorkshopId is required.");
            }

            var reviews = await _reviewsService.GetAllReviewsByWorkshopId(workshopId, query.Skip, query.Take);
            return Ok(_mapper.Map<List<FullReviewDto>>(reviews));
        }
    }
}
