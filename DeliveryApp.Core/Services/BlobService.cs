using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace DeliveryApp.Core.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly string _blobContainerName = "images";
        private readonly ILogger<BlobService> _logger;

        public BlobService(string connectionString, ILogger<BlobService> logger)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            _containerClient.CreateIfNotExists();
            _logger = logger;
        }

        public async Task<string> AddBlob(string blobName, IEnumerable<byte> data)
        {
            try
            {
                string uniqueName = $"{Guid.NewGuid()}_{blobName}";
                var blob = _containerClient.GetBlobClient(uniqueName);
                using var ms = new MemoryStream(data.ToArray());
                await blob.UploadAsync(ms, overwrite: true);
                return blob.Uri.ToString(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while uploading blob {blobName}.");
                throw;
            }
        }

    }
}
