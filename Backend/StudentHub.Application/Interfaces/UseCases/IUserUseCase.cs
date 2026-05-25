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
        Task<Result<PaginatedResponse<UserDto>>> SearchAsync(string? search, string? role, int page, int pageSize);
        Task<Result<UserInfoDto?>> GetInfoById(Guid id);
        Task<Result<Stream?>> GetProfilePictureById(Guid id);
        Task<Result<Stream?>> GetProfilePictureByUsername(string username);
        Task<Result> AddProfilePicture(Guid id, Stream picture);
        Task<Result> DeleteAsync(Guid id);
        Task<Result> ReplaceAssignableRoleAsync(Guid id, string role);
        Task<Result<UserDto?>> LoginWithOAuth2Async(string externalId, string username, string fullName);

        // Mute operations
        Task<Result> MuteUserAsync(Guid targetUserId, Guid actorId, string duration, string? reason);
        Task<Result> UnmuteUserAsync(Guid targetUserId, Guid actorId);
        Task<Result<MuteInfoDto?>> GetMuteInfoAsync(Guid userId);
        Task<Result<List<MuteInfoDto>>> GetAllActiveMutesAsync(int page = 1, int pageSize = 20);
    }
}
