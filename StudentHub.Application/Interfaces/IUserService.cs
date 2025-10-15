using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<string, string>> RegisterAsync(RegisterUserRequest request);
        Task<Result<string, string>> CheckPasswordAsync(string username, string password);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<List<UserDto>> GetAllAsync();
    }
}
