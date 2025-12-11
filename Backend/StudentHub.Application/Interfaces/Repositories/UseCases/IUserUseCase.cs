using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces.UseCases
{
    public interface IUserUseCase
    {
        Task<Result<UserDto>> RegisterAsync(RegisterUserCommand request);
        Task<Result<UserDto?>> LoginAsync(string username, string password);
        Task<Result> CheckPasswordAsync(string username, string password);
        Task<Result<UserDto?>> GetByUsernameAsync(string username);
        Task<Result<UserDto?>> GetByIdAsync(Guid id);
        Task<Result<List<UserDto>>> GetAllAsync();
        Task<Result<UserInfoDto?>> GetInfoById(Guid id);
        Task<Result<Stream?>> GetProfilePictureById(Guid id);
        Task<Result<Stream?>> GetProfilePictureByUsername(string username);
        Task<Result> AddProfilePicture(Guid id, Stream picture);
    }
}
