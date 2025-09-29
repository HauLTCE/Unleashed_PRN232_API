using Microsoft.EntityFrameworkCore;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.MapControllers();

app.Run();
