using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(User user, string password);
        Task<bool> CreateRole(string roleName);
        Task<bool> AddToRoleAsync(string username, string role);
        Task<bool> CheckPasswordAsync(string username, string password);
    }
}