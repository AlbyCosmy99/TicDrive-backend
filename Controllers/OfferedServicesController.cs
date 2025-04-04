using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicDrive.Dto.OfferedServicesDto;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferedServicesController : ControllerBase
    {
        private readonly IOfferedServicesService _offeredServicesService;
        private readonly IMapper _mapper;

        public OfferedServicesController(IOfferedServicesService offeredServicesService, IMapper mapper)
        {
            _offeredServicesService = offeredServicesService;
            _mapper = mapper;
        }
        public class GetOfferedServicesQueries
        {
            public required string WorkshopId { get; set; }
            public int? ServiceId { get; set; }
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetOfferedServices([FromQuery] GetOfferedServicesQueries query)
        {
            if (query == null || string.IsNullOrEmpty(query.WorkshopId))
            {
                return BadRequest("WorkshopId is required as query parameters.");
            }

            return Ok(_mapper.Map<List<FullOfferedServicesDto>>(_offeredServicesService.GetOfferedServices(query.WorkshopId, query.ServiceId)));
        }
    }
}
