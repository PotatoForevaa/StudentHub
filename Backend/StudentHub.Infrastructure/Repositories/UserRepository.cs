using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentHub.Application.DTOs;
using StudentHub.Application.Interfaces.Repositories;
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

        public async Task<List<User>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            if (page == 0 && pageSize == 0) return await _db.Users.ToListAsync();
            return await _db.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Result<User?>> GetByUsernameAsync(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username.ToUpper() == username.ToUpper());
            if (user == null) return Result<User?>.Failure($"Пользователь {username} не найден", "username", ErrorType.NotFound);
            return Result<User?>.Success(user);
        }


        public async Task<Result<User?>> GetByIdAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return Result<User?>.Failure($"Пользователь {id} не найден", "id", ErrorType.NotFound);
            return Result<User?>.Success(user);
        }


        public async Task<Result<User?>> AddAsync(User user, string password)
        {
            await _db.Users.AddAsync(user);

            var appUser = new AppUser { Id = user.Id, UserName = user.Username };
            var result = await _userManager.CreateAsync(appUser, password);

            if (result.Succeeded)
            {
                await _db.SaveChangesAsync();
                return Result<User>.Success(user);
            }

            return Result<User?>.Failure(result.Errors.Select(e => new Error
            {
                Field = "Password",
                Message = e.Description
            }).ToList());
        }

        public async Task<Result> CheckPasswordAsync(string userName, string password)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null)
                return Result.Failure($"Пользователь {userName} не найден", "username", ErrorType.NotFound);

            var result = await _userManager.CheckPasswordAsync(appUser, password);
            return result ?
                 Result.Success() :
                 Result.Failure("Неверный пароль", "password", ErrorType.Unauthorized);
        }

        public async Task<Result> AddToRoleAsync(string username, string role)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return Result.Failure($"Пользователь  {username}  не найден", "username", ErrorType.NotFound);

            var result = await _userManager.AddToRoleAsync(appUser, role);
            return result.Succeeded ?
                Result.Success() :
                Result.Failure(result.Errors.Select(e => new Error
                {
                    Message = e.Description
                }).ToList());
        }

        public async Task<Result> CreateRole(string roleName)
        {
            var role = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };

            var result = await _roleManager.CreateAsync(role);
            return result.Succeeded ?
                Result.Success() :
                Result.Failure(result.Errors.Select(e => new Error
                {
                    Message = e.Description
                }).ToList());
        }

        public async Task UpdateAsync(User user)
        {
            _db.Update<User>(user);
            await _db.SaveChangesAsync();
        }
    }
}
