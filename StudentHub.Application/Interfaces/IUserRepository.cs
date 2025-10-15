using StudentHub.Application.DTOs.Responses;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<Result<string, string>> AddAsync(User user, string password);
        Task<Result<string, string>> CreateRole(string roleName);
        Task<Result<string, string>> AddToRoleAsync(string username, string role);
        Task<Result<string, string>> CheckPasswordAsync(string username, string password);
    }
}