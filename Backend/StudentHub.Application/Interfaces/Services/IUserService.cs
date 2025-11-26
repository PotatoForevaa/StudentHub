using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserDto>> RegisterAsync(RegisterUserCommand request);
        Task<Result> CheckPasswordAsync(string username, string password);
        Task<Result<UserDto?>> GetByUsernameAsync(string username);
        Task<Result<UserDto?>> GetByIdAsync(Guid id);
        Task<List<UserDto>> GetAllAsync();
        Task<Result<UserInfoDto?>> GetInfoById(Guid id);
        Task<Result<Stream?>> GetProfilePictureById(Guid id);
        Task<Result<Stream?>> GetProfilePictureByUsername(string username);
        Task<Result> AddProfilePicture(Guid id, Stream picture);
    }
}
