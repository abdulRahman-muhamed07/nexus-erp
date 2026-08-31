using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Domain.Entities;
using XeoTechErp.Api.Domain.Enums;
using XeoTechErp.Api.Data;

namespace XeoTechErp.Api.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(XeoTechDbContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var user = new User
            {
                Email = "admin@nexuserp.io",
                DisplayName = "System Administrator",
                Role = Role.Administrator
            };

            user.PasswordHash = hasher.HashPassword(user, "admin123");
            db.Users.Add(user);
        }

        if (!await db.AppConfig.AnyAsync())
            db.AppConfig.Add(new AppConfig());

        await db.SaveChangesAsync();
    }
}