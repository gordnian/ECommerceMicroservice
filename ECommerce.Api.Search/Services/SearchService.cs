using ECommerce.Api.Search.Interfaces;

namespace ECommerce.Api.Search.Services;

public class SearchService : ISearchService
{
    private readonly IOrderService _orderService;
    private readonly IProductsService _productsService;
    private readonly ICustomersService _customersService;

    public SearchService(IOrderService orderService, IProductsService productsService, ICustomersService customersService)
    {
        _orderService = orderService;
        _productsService = productsService;
        _customersService = customersService;
    }
    public async Task<(bool IsSuccess, dynamic SearchResult)> SearchAsync(int customerId)
    {
        var customersResult = await _customersService.GetCustomerAsync(customerId);
        var ordersResult = await _orderService.GetOrdersAsync(customerId);
        var productsResult = await _productsService.GetProductsAsync();
        if (ordersResult.IsSuccess)
        {
            foreach (var order in ordersResult.Orders)
            {
                foreach (var item in order.Items)
                {
                    item.ProductName = productsResult.IsSuccess
                        ? productsResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name
                        : "Product information is not available";
                }
            }
            var result = new
            {
                Customer = customersResult.IsSuccess ? customersResult.Customer : new {Name = "Customer information is not available"},
                Orders = ordersResult.Orders
            };
            return (true, result);
        }

        return (false, null)!;
    }
}