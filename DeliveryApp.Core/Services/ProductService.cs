using Azure.Data.Tables;
using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.Extensions.Logging;

namespace DeliveryApp.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly TableClient _tableClient;
        private readonly string _tableName = "products";
        private readonly BlobService _blobService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(string connectionString, ILogger<ProductService> logger, ILogger<BlobService> blobLogger)
        {
            _tableServiceClient = new TableServiceClient(connectionString);
            _tableClient = _tableServiceClient.GetTableClient(_tableName);
            _tableClient.CreateIfNotExists();
            _blobService = new BlobService(connectionString, blobLogger);
            _logger = logger;
        }

        public async Task AddProduct(Product product, IEnumerable<byte> bytes)
        {
            try
            {
                var imageUrl = await _blobService.AddBlob(product.Image, bytes);
                product.Url = imageUrl; 
                await _tableClient.AddEntityAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a product.");
                throw;
            }
        }

        public IEnumerable<Product> GetProducts()
        {
            try
            {
                var products = _tableClient.Query<Product>(x => x.PartitionKey.Equals(nameof(Product))).ToList();

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products.");
                throw;
            }
        }

        public Task OrderProduct(string rowKey, string email)
        {
            throw new NotImplementedException();
        }
    }
}
