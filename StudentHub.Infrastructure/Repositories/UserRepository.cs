using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;
using StudentHub.Infrastructure.Data;
using StudentHub.Infrastructure.Identity;

namespace StudentHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserRepository(ILogger<UserRepository> logger, AppDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<User>> GetAllAsync() => 
            await _db.Users.ToListAsync();

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _db.Users.FindAsync(id);

        public async Task<bool> AddAsync(User user, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Username == user.Username)) 
                return false;

            await _db.Users.AddAsync(user);

            var appUser = new AppUser { Id = user.Id, UserName = user.Username };
            var result = await _userManager.CreateAsync(appUser, password);

            if (result.Succeeded)
                return true;

            return false;           
        }

        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null) return false;
            return await _userManager.CheckPasswordAsync(appUser, password);
        }

        public async Task<bool> AddToRoleAsync(string username, string role)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return false;
            
            return (await _userManager.AddToRoleAsync(appUser, role)).Succeeded;
        }

        public async Task<bool> CreateRole(string roleName)
        {
            if (await _roleManager.Roles.AnyAsync(r => r.Name == roleName)) return false;

            var role = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName
            };

            return (await _roleManager.CreateAsync(role)).Succeeded;            
        }
    }
}
