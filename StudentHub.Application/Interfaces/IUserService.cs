using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterUserRequest request);
        Task<bool> CheckPasswordAsync(string login, string password);
        Task<UserDto?> GetByLoginAsync(string login);
    }
}
