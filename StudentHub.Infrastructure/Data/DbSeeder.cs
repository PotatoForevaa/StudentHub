using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StudentHub.Infrastructure.Identity;

namespace StudentHub.Infrastructure.Data
{
    public class DbSeeder
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public DbSeeder(IConfiguration configuration, RoleManager<IdentityRole<Guid>> roleManager, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedAdminAsync()
        {
            string username = _configuration["AdminUser:Username"] ?? "Admin";
            string password = _configuration["AdminUser:Password"] ?? throw new Exception("Admin password not configured!");
            string fullName = _configuration["AdminUser:FullName"] ?? "Admin";

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));

            var admin = await _userManager.FindByNameAsync(username);
            if (admin != null)
            {
                var adminUser = new AppUser
                {
                    UserName = username,
                    FullName = fullName
                };

                await _userManager.CreateAsync(adminUser, password);
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

        }
    }
}
