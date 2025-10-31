using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Profiles;
using OrderService.Repositories;
using OrderService.Repositories.Interfaces;
using OrderService.Services;
using OrderService.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProductService.Clients.IClients;
using OrderService.Clients;
using OrderService.Clients.IClients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(builder.Configuration["FrontEnd"])
                  .AllowAnyMethod()
                  .WithHeaders("Content-Type", "Authorization");
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 1. Validates that the token was signed by the key we specified.
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        // 2. Set these to 'false' because don't have issuer or audience.
        ValidateIssuer = false,
        ValidateAudience = false,

        // 3. Validates that the token has not expired.
        ValidateLifetime = true,

        // 4. Sets a grace period for token expiration to account for clock differences.
        // TimeSpan.Zero means no grace period.
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddHttpClient("authservice", client =>
{
    client.BaseAddress = new Uri("http://authservice");
}).AddServiceDiscovery();

string? productApiUrl = builder.Configuration["ServiceUrls:ProductApiBase"];
if (string.IsNullOrEmpty(productApiUrl))
{
    throw new ArgumentNullException("ServiceUrls:ProductApiBase is not configured.");
}

builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(productApiUrl);
}).AddServiceDiscovery();

string? inventoryApiUrl = builder.Configuration["ServiceUrls:InventoryApiBase"];
if (string.IsNullOrEmpty(inventoryApiUrl))
{
    throw new ArgumentNullException("ServiceUrls:InventoryApiBase is not configured.");
}


builder.Services.AddHttpClient<IInventoryApiClient, InventoryApiClient>(client =>
{
    client.BaseAddress = new Uri(inventoryApiUrl);
}).AddServiceDiscovery();

builder.Services.AddHttpClient("discountservice", client =>
{
    client.BaseAddress = new Uri("http://discountservice");
}).AddServiceDiscovery();

builder.Services.AddHttpClient("notificationservice", client =>
{
    client.BaseAddress = new Uri("http://notificationservice");
}).AddServiceDiscovery();

builder.Services.AddHttpClient("cartservice", client =>
{
    client.BaseAddress = new Uri("http://cartservice");
}).AddServiceDiscovery();





// Add services to the container.
builder.Services.AddScoped<IOrderStatusRepo, OrderStatusRepo>();
builder.Services.AddScoped<IPaymenMethodRepo, PaymentMethodRepo>();
builder.Services.AddScoped<IShippingRepo, ShippingRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderServices>();
builder.Services.AddScoped<IOrderVariationRepo, OrderVariationRepo>();
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
