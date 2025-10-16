using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> RegisterAsync(RegisterUserCommand request);
        Task<Result> CheckPasswordAsync(string username, string password);
        Task<Result<UserDto?>> GetByUsernameAsync(string username);
        Task<Result<UserDto?>> GetByIdAsync(Guid id);
        Task<List<UserDto>> GetAllAsync();
    }
}
