using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;
using StudentHub.Infrastructure.Data;

namespace StudentHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly AppDbContext _db;

        public UserRepository(ILogger<UserRepository> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<List<User>> GetAllAsync() => 
            await _db.Users.ToListAsync();

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _db.Users.FindAsync(id);

        public async Task<bool> AddAsync(User user)
        {
            if (await _db.Users.AnyAsync(u => u.Username == user.Username)) return false;
            await _db.Users.AddAsync(user);
            return true;           
        }
    }
}
