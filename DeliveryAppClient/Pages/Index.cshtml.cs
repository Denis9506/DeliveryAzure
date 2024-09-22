using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DeliveryAppClient.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly INavigationService _navigationService;
        private readonly IProductService _productService;

        public IndexModel(ILogger<IndexModel> logger, INavigationService navigationService, IProductService productService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _productService = productService;
            Products = new List<Product>(_productService.GetProducts());
        }

        [BindProperty]
        public string Message { get; set; } = string.Empty;

        public List<Product> Products { get; set; }

        public async Task OnPostAsync(string ProductRowKey, string Email)
        {
            try
            {
                Order order = new Order
                {
                    ProductRowKey = ProductRowKey,
                    Email = Email
                };

                await _navigationService.SendMessageAsync(JsonSerializer.Serialize(order));
                _logger.LogInformation("Order sent successfully for ProductRowKey: {ProductRowKey}, Email: {Email}", ProductRowKey, Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending order for ProductRowKey: {ProductRowKey}, Email: {Email}", ProductRowKey, Email);
            }
        }
    }
}
