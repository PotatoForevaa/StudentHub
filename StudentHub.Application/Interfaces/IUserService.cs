using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterUserRequest request);
        Task<bool> CheckPasswordAsync(string username, string password);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<List<UserDto>> GetAllAsync();
    }
}
