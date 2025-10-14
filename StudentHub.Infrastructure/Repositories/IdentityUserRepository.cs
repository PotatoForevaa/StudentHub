using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;
using StudentHub.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Infrastructure.Repositories
{
    public class IdentityUserRepository : IIdentityUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public IdentityUserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<AppUser>> GetAllAsync() => await _userManager.Users.ToListAsync();

        public Task<bool> AddAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        

        public Task<User?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByLoginAsync(string login)
        {
            throw new NotImplementedException();
        }
    }
}
