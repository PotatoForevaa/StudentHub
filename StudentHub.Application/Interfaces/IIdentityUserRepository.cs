using StudentHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Application.Interfaces
{
    public interface IIdentityUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByLoginAsync(string login);
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(User user, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
    }
}
