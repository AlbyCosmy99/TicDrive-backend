using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicDrive.Context;

namespace TicDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly TicDriveDbContext _dbContext;

        public LanguagesController(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("")]
        public IActionResult GetLanguages()
        {
            return Ok(_dbContext.Languages.ToList());
        } 
    }
}
