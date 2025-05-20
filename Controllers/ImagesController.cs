using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Dto.UserImageDto;
using TicDrive.Models;
using TicDrive.Services;
using TicDrive.utils.files;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private const string CONTAINER_NAME = "user-images";

        private readonly IAuthService _authService;
        private readonly IImagesService _imagesService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly TicDriveDbContext _dbContext;
        private readonly IMapper _mapper;


        public ImagesController(
            BlobServiceClient blobServiceClient, 
            TicDriveDbContext dbContext, 
            IAuthService authService, 
            IImagesService imagesService, 
            IMapper mapper)
        {
            _blobServiceClient = blobServiceClient;
            _dbContext = dbContext;
            _authService = authService;
            _imagesService = imagesService;
            _mapper = mapper;
        }

        [HttpPost("{userId}/multiple")]
        public async Task<IActionResult> UploadImages(
          string userId,
          List<IFormFile> files,
          [FromForm] int mainImageIndex = 0)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Files are required.");

            var containerClient = _blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);
            await containerClient.CreateIfNotExistsAsync();

            var existingImages = _dbContext.UserImages.Where(ui => ui.UserId == userId);
            foreach (var img in existingImages)
                img.IsMainImage = false;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var blobName = $"{userId}/{Guid.NewGuid()}_{file.FileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);

                _dbContext.UserImages.Add(new UserImage
                {
                    UserId = userId,
                    Filename = blobName,
                    IsMainImage = (i == mainImageIndex)
                });
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "All images uploaded successfully" });
        }

        public class GetUserImagesQuery
        {
            public int Take { get; set; }
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetUserImages([FromQuery] GetUserImagesQuery query)
        {
            var take = 5;

            if(query?.Take != null)
            {
                take = query.Take;
            }
            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            if (userId == null)
            {
                return BadRequest("User info not found. Payload broken.");
            }

            var images = await _imagesService.GetUserImagesAsync(userId, take);

            return Ok(_mapper.Map<List<FullUserImageDto>>(images));
        }
    }
}
