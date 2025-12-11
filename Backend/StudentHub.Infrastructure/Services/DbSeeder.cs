using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentHub.Application.Entities;
using StudentHub.Infrastructure.Data;
using StudentHub.Infrastructure.Identity;

namespace StudentHub.Infrastructure.Services
{
    public static class DbSeeder
    {
        public static async Task SeedAdmin(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateAsyncScope();

            var appDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var username = config["ADMIN:USERNAME"] ?? "Admin";
            var password = config["ADMIN:PASSWORD"];
            var name = config["ADMIN:FULLNAME"] ?? "Admin";

            const string roleName = "Admin";

            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

            var admin = await userManager.FindByNameAsync(username);
            if (admin == null)
            {
                if (password == null) throw new Exception("Admin password not configured");
                admin = new AppUser { UserName = username };
                await userManager.CreateAsync(admin, password);
                await userManager.AddToRoleAsync(admin, roleName);
            }

            if (!await appDb.Users.AnyAsync(u => u.Id == admin.Id))
            {
                var user = new User
                {
                    Id = admin.Id,
                    FullName = name,
                    Username = username
                };
                await appDb.AddAsync(user);
                await appDb.SaveChangesAsync();
            }
        }
    }
}
