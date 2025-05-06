using Azure.Storage.Blobs;
using TicDrive.utils.files;
using TicDrive.Context;
using TicDrive.Dto.UserImageDto;
using TicDrive.Models;


namespace TicDrive.Services
{
    public interface IImagesService
    {
        Task<IEnumerable<UserImage>> GetUserImagesAsync(string userId, int take);
    }

    public class ImagesService : IImagesService
    {
        private const string CONTAINER_NAME = "user-images";
        private readonly BlobServiceClient _blobServiceClient;
        private readonly TicDriveDbContext _context;

        public ImagesService(BlobServiceClient blobServiceClient, TicDriveDbContext context)
        {
            _blobServiceClient = blobServiceClient;
            _context = context;
        }

        public async Task<IEnumerable<UserImage>> GetUserImagesAsync(string userId, int take)
        {
            var userImages = _context.UserImages
                .Where(img => img.UserId == userId)
                .OrderByDescending(img => img.IsMainImage)
                .Take(take)
                .ToList();

            var containerClient = _blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);

            var images = new List<UserImage>();

            foreach (var image in userImages)
            {
                var blobClient = containerClient.GetBlobClient(image.Filename);

                if (await blobClient.ExistsAsync())
                {
                    var sasUri = ImagesUtils.GenerateSasUri(blobClient, TimeSpan.FromMinutes(15));

                    images.Add(new UserImage
                    {
                        Id = image.Id,
                        UserId = image.UserId,
                        Filename = sasUri.ToString(),
                        IsMainImage = image.IsMainImage
                    });
                }
            }

            return images;
        }
    }
}
