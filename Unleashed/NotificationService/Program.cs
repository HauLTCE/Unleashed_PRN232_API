using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Data;
using NotificationService.Profiles;
using NotificationService.Repositories;
using NotificationService.Repositories.IRepositories;
using NotificationService.Services;
using NotificationService.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// --- Database Context ---
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// --- Dependency Injection ---

// Add AutoMapper, scanning the current assembly for profiles
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<NotificationProfile>();
});

// Add custom services and repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotiService>();


// --- API Services ---
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- HTTP Request Pipeline Configuration ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
