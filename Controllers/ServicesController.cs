using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using TicDrive.Dto.ServiceDto;
using TicDrive.Enums;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController(IServicesService servicesService, IMapper mapper) : ControllerBase
    {
        private readonly IServicesService _servicesService = servicesService;
        private readonly IMapper _mapper = mapper;

        public class GetServicesQueries
        {
            public int? Skip { get; set; } = 0;
            public int? Take { get; set; } = 10;
            public string? Filter { get; set; } = string.Empty;
            public string? LanguageCode { get; set; }
            public int? FatherId { get; set; }
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetServices(string? workshopId, [FromQuery] GetServicesQueries query)
        {
            var languageCode = "en";
            if(query != null && query.LanguageCode != null)
            {
                languageCode = query.LanguageCode;
            }

            try
            {
                var services = _servicesService.GetServices(workshopId, query.Filter, languageCode, query.FatherId);
                var paginatedServices = services.Skip(query.Skip ?? 0).Take(query.Take ?? 10).ToList();

                return Ok(paginatedServices);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is OperationCanceledException)
            {
                return BadRequest(new { Message = "An error occurred.", Details = ex.Message });
            }
        }

    }
}
