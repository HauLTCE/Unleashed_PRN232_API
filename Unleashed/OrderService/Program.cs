using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Profiles;
using OrderService.Repositories;
using OrderService.Repositories.Interfaces;
using OrderService.Services;
using OrderService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddHttpClient("authservice", client =>
{
    client.BaseAddress = new Uri("http://authservice");
});

builder.Services.AddHttpClient("productservice", client =>
{
    client.BaseAddress = new Uri("http://productservice");
});

builder.Services.AddHttpClient("inventoryservice", client =>
{
    client.BaseAddress = new Uri("http://inventoryservice");
});

builder.Services.AddHttpClient("discountservice", client =>
{
    client.BaseAddress = new Uri("http://discountservice");
});

builder.Services.AddHttpClient("notificationservice", client =>
{
    client.BaseAddress = new Uri("http://notificationservice");
});

builder.Services.AddHttpClient("cartservice", client =>
{
    client.BaseAddress = new Uri("http://cartservice");
});





// Add services to the container.
builder.Services.AddScoped<IOrderStatusRepo, OrderStatusRepo>();
builder.Services.AddScoped<IPaymenMethodRepo, PaymentMethodRepo>();
builder.Services.AddScoped<IShippingRepo, ShippingRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderServices>();
builder.Services.AddScoped<IOrderVariationSingleRepo, OrderVariationSingleRepo>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(OrderProfile).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
