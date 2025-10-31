using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.Repositories;
using ReviewService.Repositories.Interfaces;
using ReviewService.Services;
using ReviewService.Services.Interfaces;
using ReviewService.Middleware;
using ReviewService.Clients.Interfaces;
using ReviewService.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery();

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

builder.Services.AddDbContext<ReviewDbContext>(options =>
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

builder.Services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://authservice");
}).AddServiceDiscovery();

builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://productservice");
}).AddServiceDiscovery();

builder.Services.AddHttpClient("orderservice", client =>
{
    client.BaseAddress = new Uri("http://orderservice");
}).AddServiceDiscovery();

builder.Services.AddHttpClient("notificationservice", client =>
{
    client.BaseAddress = new Uri("http://notificationservice");
}).AddServiceDiscovery();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewServicee>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();