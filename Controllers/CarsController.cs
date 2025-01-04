using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicDrive.Dto.CarDto.CarMakeDto;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : Controller
    {
        private readonly ICarsService _carsService;
        private readonly IMapper _mapper;

        public CarsController(ICarsService carsService, IMapper mapper)
        {
            _carsService = carsService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("makes")]
        public IActionResult GetMakes()
        {
            return Ok(_mapper.Map<List<FullCarMakeDto>>(_carsService.GetMakes()));
        }
    }
}
