using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=xeotech-erp.db";

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<XeoTechDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Apply the schema and seed the admin account.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<XeoTechDbContext>();
    db.Database.Migrate();
    if (!db.Users.Any())
    {
        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            Email = "admin@nexuserp.io",
            DisplayName = "Alex Morgan",
            Role = Role.Administrator
        };
        admin.PasswordHash = hasher.HashPassword(admin, "admin123");
        db.Users.Add(admin);
        db.AppConfig.Add(new AppConfig { TaxRate = 8m, ShippingFee = 25m, FreeShipOver = 1000m });
        db.SaveChanges();
    }
}

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;