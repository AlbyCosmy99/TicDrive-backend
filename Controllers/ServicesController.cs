using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicDrive.Dto.ServiceDto;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController(IServicesService servicesService, IMapper mapper) : ControllerBase
    {
        private readonly IServicesService _servicesService = servicesService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetServices(string? workshopId)
        {
            try
            {
                var services = await _servicesService.GetServices(workshopId);
                return Ok(_mapper.Map<List<FullServiceDto>>(services));
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is OperationCanceledException)
            {
                return BadRequest(new { Message = "An error occurred.", Details = ex.Message });
            }
        }
    }
}
