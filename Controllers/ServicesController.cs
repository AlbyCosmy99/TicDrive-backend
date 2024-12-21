using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicDrive.Dto.ServiceDto;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;
        private readonly IMapper _mapper;

        public ServicesController(IServicesService servicesService, IMapper mapper)
        {
            _servicesService = servicesService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetServices()
        {
            return Ok(_mapper.Map<List<FullServiceDto>>(_servicesService.GetServices()));
        }
    }
}
