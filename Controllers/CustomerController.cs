using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicDrive.Dto.FavoriteWorkshopDto;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CustomerController(IAuthService authService, ICustomerService customerService, IMapper mapper)
        {
            _authService = authService;
            _customerService = customerService;
            _mapper = mapper;
        }
        //[Route("workshops/favorite")]
        //[Authorize]
        //public async Task<IActionResult> GetFavoriteWorkshops()
        //{
        //    var userClaims = _authService.GetUserClaims(this);
        //    var userId = _authService.GetUserId(userClaims);

        //    var favoriteWorkshops = await _customerService.GetFavoriteWorkshops(userId);
        //    return Ok(_mapper.Map<List<FullFavoriteWorkshopDto>>(favoriteWorkshops));
        //}
    }
}
