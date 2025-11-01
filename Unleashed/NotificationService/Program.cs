using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Clients;
using NotificationService.Clients.IClients;
using NotificationService.Data;
using NotificationService.Profiles;
using NotificationService.Repositories;
using NotificationService.Repositories.IRepositories;
using NotificationService.Services;
using NotificationService.Services.IServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery();

// --- Database Context ---
builder.Services.AddDbContext<NotificationDbContext>(options =>
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
// Add AutoMapper, scanning the current assembly for all profiles (like MappingProfile)
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<NotificationProfile>();
    cfg.AddProfile<NotificationUserProfile>();
});

// Add custom services and repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotiService>();

builder.Services.AddScoped<INotificationUserRepository, NotificationUserRepository>();
builder.Services.AddScoped<INotificationUserService, NotiUserService>();


builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>("authservice", client =>
{
    client.BaseAddress = new Uri("http://authservice");
}).AddServiceDiscovery();

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

