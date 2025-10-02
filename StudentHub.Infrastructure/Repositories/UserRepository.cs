using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;
using StudentHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace StudentHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
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

        public async Task AddAsync(User user, string password)
        {
            var appUser = new AppUser
            {
                UserName = user.UserName,
                FullName = user.FullName
            };

            await _userManager.CreateAsync(appUser, password);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var appUser = await _userManager.FindByNameAsync(user.UserName);
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
