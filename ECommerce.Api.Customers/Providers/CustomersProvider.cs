using AutoMapper;
using ECommerce.Api.Customers.Db;
using ECommerce.Api.Customers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Customer = ECommerce.Api.Customers.Models.Customer;

namespace ECommerce.Api.Customers.Providers;

public class CustomersProvider : ICustomersProvider
{
    private readonly CustomerDbContext _dbContext;
    private readonly ILogger<CustomersProvider> _logger;
    private readonly IMapper _mapper;

    public CustomersProvider(CustomerDbContext dbContext, ILogger<CustomersProvider> logger, IMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
        SeedData();
    }

    private void SeedData()
    {
        if (!_dbContext.Customers.Any())
        {
            _dbContext.Customers.Add(new Db.Customer() { Id = 1, Name = "Jessica Smith", Address = "20 Elm St." });
            _dbContext.Customers.Add(new Db.Customer() { Id = 2, Name = "JohnSmith", Address = "30 Main St." });
            _dbContext.Customers.Add(new Db.Customer() { Id = 3, Name = "William Johnson", Address = "100 10th St." });
            _dbContext.SaveChanges();
        }
    }

    public async Task<(bool IsSuccess, IEnumerable<Customer> Customers, string ErrorMessage)> GetCustomersAsync()
    {
        try
        {
            _logger.LogInformation("Querying customers");
            var customers = await _dbContext.Customers.ToListAsync();
            if (customers.Any())
            {
                _logger.LogInformation("{Count} customer(s) found", customers.Count.ToString());
                var result = _mapper.Map<IEnumerable<Db.Customer>, IEnumerable<Models.Customer>>(customers);
                return (true, result, null)!;
            }

            return (false, null, "Not found")!;
        }
        catch (Exception e)
        {
            _logger.LogError("Error getting customers {Error}", e.ToString());
            return (false, null, e.Message)!;
        }
    }

    public async Task<(bool IsSuccess, Customer Customer, string ErrorMessage)> GetCustomerAsync(int id)
    {
        try
        {
            _logger.LogInformation("Querying customers");
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer != null)
            {
                _logger.LogInformation("Customer found");
                var result = _mapper.Map<Db.Customer, Models.Customer>(customer);
                return (true, result, null)!;
            }

            return (false, null, "Not found")!;
        }
        catch (Exception e)
        {
            _logger.LogError("Error getting customers {Error}", e.ToString());
            return (false, null, e.Message)!;
        }
    }
}