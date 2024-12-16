using Microsoft.AspNetCore.Mvc;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;

        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        [HttpGet]
        public IActionResult GetServices()
        {
            return Ok(_servicesService.GetServices());
        }
    }
}
