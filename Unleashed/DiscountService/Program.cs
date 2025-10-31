using DiscountService.Data;
using DiscountService.Profiles;
using DiscountService.Repositories;
using DiscountService.Repositories.Interfaces;
using DiscountService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery();

builder.Services.AddDbContext<DiscountDbContext>(options =>
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
}).AddServiceDiscovery();

builder.Services.AddHttpClient("notificationservice", client =>
{
    client.BaseAddress = new Uri("http://notificationservice");
}).AddServiceDiscovery();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(DiscountProfile).Assembly);
});
// Add services to the container.
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IUserDiscountRepository, UserDiscountRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IDiscountStatusRepository, DiscountStatusRepository>();
builder.Services.AddScoped<IDiscountTypeRepository, DiscountTypeRepository>();
builder.Services.AddScoped<IDiscountService, DiscountService.Services.DiscountService>();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
