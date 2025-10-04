using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByLoginAsync(string login);
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(User user, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
    }

}