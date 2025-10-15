using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentHub.Application.DTOs.Responses;
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

        public async Task<Result<string, string>> AddAsync(User user, string password)
        {
            await _db.Users.AddAsync(user);

            var appUser = new AppUser { Id = user.Id, UserName = user.Username };
            var result = await _userManager.CreateAsync(appUser, password);

            if (result.Succeeded)
            {
                await _db.SaveChangesAsync();
                return Result<string, string>.Success($"User {user.Username} created");
            }

            return Result<string, string>.Failure(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        public async Task<Result<string, string>> CheckPasswordAsync(string userName, string password)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null)
                return Result<string, string>.Failure($"User {userName} not found", ErrorType.NotFound);

            var result = await _userManager.CheckPasswordAsync(appUser, password);
            return result ?
                 Result<string, string>.Success("Password verified") :
                 Result<string, string>.Failure("Wrong password", ErrorType.Unauthorized);
        }

        public async Task<Result<string, string>> AddToRoleAsync(string username, string role)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return Result<string, string>.Failure($"User {username} not found", ErrorType.NotFound);

            var result = await _userManager.AddToRoleAsync(appUser, role);
            return result.Succeeded ? 
                Result<string, string>.Success($"User {username} added to role {role}") :
                Result<string, string>.Failure(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        public async Task<Result<string, string>> CreateRole(string roleName)
        {
            var role = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };

            var result = await _roleManager.CreateAsync(role);
            return result.Succeeded ?
                Result<string, string>.Success(role.Id.ToString()) : 
                Result<string, string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
