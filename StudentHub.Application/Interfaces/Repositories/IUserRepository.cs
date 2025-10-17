using StudentHub.Application.DTOs;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(int page = 0, int pageSize = 0);
        Task<Result<User?>> GetByUsernameAsync(string username);
        Task<Result<User?>> GetByIdAsync(Guid id);
        Task<Result<User?>> AddAsync(User user, string password);
        Task<Result> CreateRole(string roleName);
        Task<Result> AddToRoleAsync(string username, string role);
        Task<Result> CheckPasswordAsync(string username, string password);
    }
}