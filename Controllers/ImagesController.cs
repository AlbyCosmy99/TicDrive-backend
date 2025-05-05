using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicDrive.Context;
using TicDrive.Models;
using TicDrive.Services;

namespace TicDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly TicDriveDbContext _dbContext;
        private const string CONTAINER_NAME = "user-images";

        public ImagesController(BlobServiceClient blobServiceClient, TicDriveDbContext dbContext, IAuthService authService)
        {
            _blobServiceClient = blobServiceClient;
            _dbContext = dbContext;
            _authService = authService;
        }

        [HttpPost("{userId}/multiple")]
        public async Task<IActionResult> UploadImages(string userId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Files are required.");

            var containerClient = _blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);
            await containerClient.CreateIfNotExistsAsync();

            foreach (var file in files)
            {
                var blobName = $"{userId}/{Guid.NewGuid()}_{file.FileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);

                _dbContext.UserImages.Add(new UserImage
                {
                    UserId = userId,
                    Url = blobName
                });
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "All images uploaded successfully" });
        }


        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetUserImages()
        {
            var userClaims = _authService.GetUserClaims(this);
            var userId = _authService.GetUserId(userClaims);

            var userImages = await _dbContext.UserImages
                .Where(img => img.UserId == userId)
                .OrderByDescending(img => img.Id)
                .ToListAsync();

            if (!userImages.Any())
                return NotFound();

            var containerClient = _blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);

            var result = new List<object>();

            foreach (var image in userImages)
            {
                var blobClient = containerClient.GetBlobClient(image.Url);

                if (await blobClient.ExistsAsync())
                {
                    var sasUri = GenerateSasUri(blobClient, TimeSpan.FromMinutes(15));

                    result.Add(new
                    {
                        image.Id,
                        image.UserId,
                        Url = sasUri.ToString()
                    });
                }
            }

            return Ok(result);
        }

        private Uri GenerateSasUri(BlobClient blobClient, TimeSpan validFor)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(validFor)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder);
        }

        private string? GetContentTypeFromExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => null
            };
        }
    }
}
