using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace TicDrive.utils.files
{
    public static class ImagesUtils
    {
        public static Uri GenerateSasUri(BlobClient blobClient, TimeSpan validFor)
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

        public static string GetContentTypeFromExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
