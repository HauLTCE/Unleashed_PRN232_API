using CartService.Data;
using CartService.Profiles;
using CartService.Repositories;
using CartService.Repositories.Interfaces;
using CartService.Services;
using CartService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery();

builder.Services.AddDbContext<CartDbContext>(options =>
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

builder.Services.AddHttpClient("productservice", client =>
{
    client.BaseAddress = new Uri("http://productservice");
}).AddServiceDiscovery();

builder.Services.AddHttpClient("inventoryservice", client =>
{
    client.BaseAddress = new Uri("http://inventoryservice");
}).AddServiceDiscovery();

builder.Services.AddHttpClient("discountservice", client =>
{
    client.BaseAddress = new Uri("http://discountservice");
}).AddServiceDiscovery();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(CartProfile).Assembly);
});

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService.Services.CartService>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["services:authservice:http:0"];
        options.Audience = "cart_service_api";
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

builder.Services.AddAuthorization();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();