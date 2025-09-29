using Microsoft.EntityFrameworkCore;
using ReviewService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("authservice", client =>
{
    client.BaseAddress = new Uri("http://authservice");
});

builder.Services.AddHttpClient("productservice", client =>
{
    client.BaseAddress = new Uri("http://productservice");
});

builder.Services.AddHttpClient("orderservice", client =>
{
    client.BaseAddress = new Uri("http://orderservice");
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
