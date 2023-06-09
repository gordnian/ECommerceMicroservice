using ECommerce.Api.Search.Interfaces;
using ECommerce.Api.Search.Services;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("OrdersService", conf =>
{
    conf.BaseAddress = new Uri(builder.Configuration["Services:Orders"]!);
});

builder.Services.AddHttpClient("ProductsService", conf =>
{
    conf.BaseAddress = new Uri(builder.Configuration["Services:Products"]!);
})
    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(500)));

builder.Services.AddHttpClient("CustomersService", conf =>
{
    conf.BaseAddress = new Uri(builder.Configuration["Services:Customers"]!);  
})
    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(500)));

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ICustomersService, CustomersService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();