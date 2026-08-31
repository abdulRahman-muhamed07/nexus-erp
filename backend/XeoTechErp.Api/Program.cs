using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using XeoTechErp.Application;
using XeoTechErp.Infrastructure;
using XeoTechErp.Api.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("A real Jwt:Key must be provided through configuration or environment variables.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "XeoTechErp.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "XeoTechErp.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("login", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Administrator"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
