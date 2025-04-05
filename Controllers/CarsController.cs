using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using TicDrive.Dto.CarDto.CarMakeDto;
using TicDrive.Dto.CarDto.CarModelDto;
using TicDrive.Enums;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarsService _carsService;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public CarsController(ICarsService carsService, IMapper mapper, IAuthService authService)
        {
            _carsService = carsService;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet]
        [Route("makes")]
        public IActionResult GetMakes()
        {
            return Ok(_mapper.Map<List<FullCarMakeDto>>(_carsService.GetMakes()));
        }

        [HttpGet]
        [Route("models/{id}")]
        public IActionResult GetMakes(int id)
        {
            return Ok(_mapper.Map<List<FullCarModelDto>>(_carsService.GetCarModelsByMakeId(id)));
        }

        public class AddCarQuery
        {
            public string? Name { get; set; }
            public string? Plate { get; set; }
            public required string Make { get; set; }
            public required string Model { get; set; }
            public int? Year { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public FuelType? FuelType { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public TransmissionType? TransmissionType { get; set; }
            public string? EngineDisplacement { get; set; }
            public int? Km { get; set; }
        }
            
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> PostCar([FromBody] AddCarQuery query)
        {
            if (query == null)
            {
                return BadRequest("query params are required.");
            }

            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            if(userId == null)
            {
                return Unauthorized("User is not authorize to register a car.");
            }

            try
            {
                await _carsService.PostCar(query, userId);
                return NoContent();
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
        }
    }
}
