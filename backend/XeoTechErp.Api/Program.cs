using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.Services;
using XeoTechErp.Api.Infrastructure;
using XeoTechErp.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<XeoTechDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=xeotech-erp.db"));
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var key = builder.Configuration["Jwt:Key"] ?? "dev-only-change-this-secret-to-a-long-random-value";
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters { ValidateIssuerSigningKey=true, IssuerSigningKey=new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)), ValidateIssuer=false, ValidateAudience=false, ValidateLifetime=true, ClockSkew=TimeSpan.FromMinutes(1) };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddScoped<IInventoryService,InventoryService>();
builder.Services.AddScoped<IDashboardService,DashboardService>();
var app=builder.Build();
if(app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
using(var scope=app.Services.CreateScope()){var db=scope.ServiceProvider.GetRequiredService<XeoTechDbContext>();db.Database.Migrate();await DatabaseSeeder.SeedAsync(db);}
app.Run();
public partial class Program;
