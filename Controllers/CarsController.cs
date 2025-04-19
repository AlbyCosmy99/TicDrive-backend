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
        public IActionResult GetModels(int id)
        {
            return Ok(_mapper.Map<List<FullCarModelDto>>(_carsService.GetCarModelsByMakeId(id)));
        }

        public class AddCarQuery
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Plate { get; set; }
            public string? Make { get; set; }
            public string? Model { get; set; }
            public int? Year { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public FuelType? FuelType { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public TransmissionType? TransmissionType { get; set; }
            public string? EngineDisplacement { get; set; }
            public int? Mileage { get; set; }
            public int? CV { get; set; }
        }
            
        [HttpPut]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> UpdateCustomerCar([FromBody] AddCarQuery query)
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
                await _carsService.UpdateCustomerCar(query, userId);
  
                return NoContent();
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> PostCustomerCar([FromBody] AddCarQuery query)
        {
            if (query == null)
            {
                return BadRequest("query params are required.");
            }

            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            if (userId == null)
            {
                return Unauthorized("User is not authorize to register a car.");
            }

            try
            {
                var carHasBeenRegistered = await _carsService.PostCustomerCar(query, userId);

                if (carHasBeenRegistered == true)
                {
                    return Created();
                }

                return NoContent(); //car already registered for this user
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("customer-cars")]
        [Authorize]
        public IActionResult GetCustomerCars()
        {
            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            if (userId == null) return Unauthorized("User id is null.");

            return Ok(_carsService.GetCustomerCars(userId));
        }

        [HttpDelete]
        [Route("customer-cars/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCustomerCar([FromRoute] int id)
        {
            await _carsService.DeleteCustomerCar(id);
            return NoContent();
        }
    }
}
