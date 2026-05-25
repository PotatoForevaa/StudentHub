using StudentHub.Application.DTOs;
using StudentHub.Application.Entities;

namespace StudentHub.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(int page = 0, int pageSize = 0);
        Task<(List<User> Users, int TotalCount)> SearchAsync(string? search, string? role, int page, int pageSize);
        Task<Result<User?>> GetByUsernameAsync(string username);
        Task<Result<User?>> GetByIdAsync(Guid id);
        Task<Result<User?>> AddAsync(User user, string password);
        Task UpdateAsync(User user);
        Task<Result> DeleteAsync(Guid id);
        Task<Result> CreateRole(string roleName);
        Task<Result> AddToRoleAsync(string username, string role);
        Task<Result> ReplaceAssignableRoleAsync(Guid userId, string role);
        Task<List<string>> GetRolesAsync(Guid userId);
        Task<Result> CheckPasswordAsync(string username, string password);
        Task<Result<User?>> GetByExternalIdAsync(string externalId);
        Task<Result<User?>> AddOAuth2UserAsync(string externalId, string username, string fullName);

        // Mute operations
        Task<Result<UserMute>> MuteUserAsync(Guid userId, Guid mutedByUserId, TimeSpan duration, string? reason);
        Task<Result> UnmuteUserAsync(Guid userId);
        Task<Result<UserMute?>> GetActiveMuteAsync(Guid userId);
        Task<Result<List<UserMute>>> GetAllActiveMutesAsync(int page = 1, int pageSize = 20);
        Task<int> CountActiveMutesAsync();
    }
}
