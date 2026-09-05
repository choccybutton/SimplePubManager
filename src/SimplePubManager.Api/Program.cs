using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using SimplePubManager.Infrastructure.Data;
using SimplePubManager.Infrastructure.Data.Repositories;
using SimplePubManager.Infrastructure.Services;
using SimplePubManager.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure services
// Add DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<OrganizationRepository>();
builder.Services.AddScoped<AreaRepository>();
builder.Services.AddScoped<ShiftRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<HolidayRepository>();
builder.Services.AddScoped<TaskRepository>();
builder.Services.AddScoped<DeviceRepository>();
builder.Services.AddScoped<RecurringTaskTemplateRepository>();
builder.Services.AddScoped<BillRepository>();

// Register Services
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DeviceAuthService>();
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddScoped<PaymentCalculationService>();
builder.Services.AddScoped<RecurringTaskService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(SimplePubManager.Infrastructure.Data.AppDbContext).Assembly);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT SecretKey is not configured or is too short (minimum 32 characters)");
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SimplePubManager",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "SimplePubManager-Users",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure CORS
var corsSettings = builder.Configuration.GetSection("Cors");
var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
// Middleware order is critical

// 1. Exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. Tenant resolution middleware (resolves organization from subdomain)
app.UseMiddleware<TenantResolutionMiddleware>();

// 3. CORS
app.UseCors("AllowFrontend");

// 4. Routing (must come before authentication and authorization)
app.UseRouting();

// 5. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6. Swagger (development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 7. Endpoints
app.MapControllers();

// 8. Health check endpoint
app.MapGet("/health", () => Results.Ok("API is running"))
    .WithName("Health")
    .AllowAnonymous();

app.Run();

public partial class Program { }
