using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DeliveryAppProject.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly INavigationService _navigationService;
        private readonly IProductService _productService;
        private readonly ILogger<OrdersModel> _logger;

        public OrdersModel(INavigationService navigationService, IProductService productService, ILogger<OrdersModel> logger)
        {
            _navigationService = navigationService;
            _productService = productService;
            _logger = logger;
        }

        public List<OrderDTO> Orders { get; set; }

        public async Task OnGet()
        {
            try
            {
                var products = _productService.GetProducts();
                var ordersJson = await _navigationService.ReceiveMassagesAsync();
                var orders = ordersJson.Select(x => JsonSerializer.Deserialize<Order>(x)!).ToList();

                Orders = orders.Select(order =>
                {
                    var product = products.First(p => p.RowKey == order.ProductRowKey);
                    return new OrderDTO
                    {
                        Name = product.Name,
                        Email = order.Email,
                        Price = product.Price,
                        Image = product.Url 
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders or products.");
            }
        }
    }
}
