using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;
using StudentHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace StudentHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(ILogger<UserRepository> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            var appUser = await _userManager.FindByNameAsync(login);
            return appUser == null ? null : Map(appUser);
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            var appUser = await _userManager.FindByIdAsync(id.ToString());
            return appUser == null ? null : Map(appUser);
        }

        public async Task<bool> AddAsync(User user, string password)
        {
            var appUser = new AppUser
            {
                UserName = user.UserName,
                FullName = user.FullName
            };

            var result = await _userManager.CreateAsync(appUser, password);
            if (!result.Succeeded)
                _logger.LogWarning($"Failed to create user: {user.Username}, {result.Errors.Select(e => e.Description)}");
            else
                _logger.LogInformation($"User created: {user.Username}");

            return result.Succeeded;
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var appUser = await _userManager.FindByNameAsync(user.Username);
            return appUser != null && await _userManager.CheckPasswordAsync(appUser, password);
        }

        private User Map(AppUser appUser) => new User
        {
            Id = appUser.Id,
            UserName = appUser.UserName!,
            FullName = appUser.FullName
        };
    }
}
