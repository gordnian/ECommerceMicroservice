using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;
using Order = ECommerce.Api.Orders.Models.Order;

namespace ECommerce.Api.Orders.Providers;

public class OrdersProvider : IOrdersProvider
{
    private readonly OrdersDbContext _dbContext;
    private readonly ILogger<OrdersProvider> _logger;
    private readonly IMapper _mapper;

    public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
        SeedData();
    }

    private void SeedData()
    {
        if (!_dbContext.Orders.Any())
        {
            _dbContext.Orders.Add(new Db.Order()
            {
                Id = 1,
                CustomerId = 1,
                OrderDate = DateTime.Now,
                Items = new List<OrderItem>()
                {
                    new OrderItem(){OrderId = 1, ProductId = 1, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 1, ProductId = 2, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 1, ProductId = 3, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 2, ProductId = 2, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 3, ProductId = 3, Quantity = 1, UnitPrice = 100},
                },
                Total = 100
            });
            _dbContext.Orders.Add(new Db.Order()
            {
                Id = 2,
                CustomerId = 1,
                OrderDate = DateTime.Now.AddDays(-1),
                Items = new List<OrderItem>()
                {
                    new OrderItem(){OrderId = 1, ProductId = 1, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 1, ProductId = 2, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 1, ProductId = 3, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 2, ProductId = 2, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 3, ProductId = 3, Quantity = 1, UnitPrice = 100},
                },
                Total = 100
            });
            _dbContext.Orders.Add(new Db.Order()
            {
                Id = 3,
                CustomerId = 2,
                OrderDate = DateTime.Now,
                Items = new List<OrderItem>()
                {
                    new OrderItem(){OrderId = 1, ProductId = 1, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 2, ProductId = 2, Quantity = 10, UnitPrice = 10},
                    new OrderItem(){OrderId = 3, ProductId = 3, Quantity = 1, UnitPrice = 100},
                },
                Total = 100
            });
            _dbContext.SaveChanges();
        }
    }

    public async Task<(bool IsSuccess, IEnumerable<Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
    {
        try
        {
            _logger.LogInformation("Querying orders");
            var orders = await _dbContext.Orders.Where(o => o.CustomerId == customerId).Include(o => o.Items)
                .ToListAsync();
            if (orders.Any())
            {
                _logger.LogInformation("Orders found");
                var result = _mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                return (true, result, null)!;
            }

            return (false, null, "Not found")!;
        }
        catch (Exception e)
        {
            _logger.LogError("Error getting orders for customers {Error}", e.ToString());
            return (false, null, e.Message)!;
        }
    }
}