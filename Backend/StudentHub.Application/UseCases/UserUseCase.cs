using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Entities;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.UseCases;

namespace StudentHub.Application.UseCases
{
    public class UserUseCase : IUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        public UserUseCase(IUserRepository userRepository, IFileStorageService fileStorageService)
        {
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result> CheckPasswordAsync(string username, string password)
        {
            return await _userRepository.CheckPasswordAsync(username, password);
        }

        public async Task<Result<List<UserDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(new UserDto(user.Id, user.Username, user.FullName, await _userRepository.GetRolesAsync(user.Id)));
            }
            return Result<List<UserDto>>.Success(userDtos);
        }

        public async Task<Result<PaginatedResponse<UserDto>>> SearchAsync(string? search, string? role, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (users, totalCount) = await _userRepository.SearchAsync(search, role, page, pageSize);
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(new UserDto(user.Id, user.Username, user.FullName, await _userRepository.GetRolesAsync(user.Id)));
            }

            return Result<PaginatedResponse<UserDto>>.Success(new PaginatedResponse<UserDto>(
                userDtos,
                page,
                pageSize,
                totalCount,
                (int)Math.Ceiling(totalCount / (double)pageSize)));
        }

        public async Task<Result<UserDto?>> GetByIdAsync(Guid id)
        {
            var userResult = await _userRepository.GetByIdAsync(id);
            if (!userResult.IsSuccess) return Result<UserDto?>.Failure(userResult.Errors, userResult.ErrorType);

            var user = userResult.Value;
            var userDto = new UserDto(user.Id, user.Username, user.FullName, await _userRepository.GetRolesAsync(user.Id));

            return Result<UserDto?>.Success(userDto);
        }

        public async Task<Result<UserDto?>> GetByUsernameAsync(string username)
        {
            var userResult = await _userRepository.GetByUsernameAsync(username);
            if (!userResult.IsSuccess) return Result<UserDto?>.Failure(userResult.Errors, userResult.ErrorType);

            var user = userResult.Value;
            var userDto = new UserDto(user.Id, user.Username, user.FullName, await _userRepository.GetRolesAsync(user.Id));

            return Result<UserDto?>.Success(userDto);
        }

        public async Task<Result<UserDto?>> LoginAsync(string username, string password)
        {
            var userResult = await _userRepository.GetByUsernameAsync(username);
            if (!userResult.IsSuccess) return Result<UserDto?>.Failure(userResult.Errors, userResult.ErrorType);

            var passwordResult = await _userRepository.CheckPasswordAsync(username, password);
            if (!passwordResult.IsSuccess) return Result<UserDto?>.Failure(passwordResult.Errors, passwordResult.ErrorType);

            var user = userResult.Value;
            var userDto = new UserDto(user.Id, user.Username, user.FullName, await _userRepository.GetRolesAsync(user.Id));

            return Result<UserDto?>.Success(userDto);
        }

        public async Task<Result<UserDto?>> RegisterAsync(RegisterUserCommand request)
        {
            var user = new User
            {
                FullName = request.FullName,
                Username = request.Username
            };
            var userResult = await _userRepository.AddAsync(user, request.Password);
            if (!userResult.IsSuccess) return Result<UserDto?>.Failure(userResult.Errors, userResult.ErrorType);

            user = userResult.Value;
            var userDto = new UserDto(user.Id, user.Username, user.FullName, await _userRepository.GetRolesAsync(user.Id));

            return Result<UserDto?>.Success(userDto);
        }

        public async Task<Result<UserInfoDto?>> GetInfoById(Guid id)
        {
            var userResult = await _userRepository
                .GetByIdAsync(id);

            if (!userResult.IsSuccess) return Result<UserInfoDto?>.Failure(userResult.Errors, userResult.ErrorType);
            var user = userResult.Value;

            var userInfo = new UserInfoDto(
                FullName: user.FullName,
                Username: user.Username
            );

            return Result<UserInfoDto?>.Success(userInfo);
        }

        public async Task<Result> AddProfilePicture(Stream picture, string fileName)
        {
            var result = await _fileStorageService.SaveFileAsync(picture, fileName);
            return result;
        }

        public async Task<Result<Stream>> GetProfilePictureById(Guid id)
        {
            var userResult = await _userRepository.GetByIdAsync(id);
            if (!userResult.IsSuccess) return Result<Stream>.Failure(userResult.Errors);
            var user = userResult.Value;

            var result = await _fileStorageService.GetFileAsync(user.ProfilePicturePath);
            return result;
        }

        public async Task<Result<Stream>> GetProfilePictureByUsername(string username)
        {
            var userResult = await _userRepository.GetByUsernameAsync(username);
            if (!userResult.IsSuccess) return Result<Stream>.Failure(userResult.Errors);
            var user = userResult.Value;

            var result = await _fileStorageService.GetFileAsync(user.ProfilePicturePath);
            return result;
        }

        public async Task<Result> AddProfilePicture(Guid id, Stream picture)
        {
            var userResult = await _userRepository.GetByIdAsync(id);
            if (!userResult.IsSuccess) return userResult;
            var user = userResult.Value;

            var pathResult = await _fileStorageService.SaveFileAsync(picture, id.ToString());
            if (!pathResult.IsSuccess) return pathResult;
            user.ProfilePicturePath = pathResult.Value;

            await _userRepository.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<Result> ReplaceAssignableRoleAsync(Guid id, string role)
        {
            return await _userRepository.ReplaceAssignableRoleAsync(id, role);
        }

        public async Task<Result<UserDto?>> LoginWithOAuth2Async(string externalId, string username, string fullName)
        {
            // Try to find existing user by externalId
            var userResult = await _userRepository.GetByExternalIdAsync(externalId);
            if (userResult.IsSuccess)
            {
                var user = userResult.Value!;
                var userDto = new UserDto(user.Id, user.Username, user.FullName, await _userRepository.GetRolesAsync(user.Id));
                return Result<UserDto?>.Success(userDto);
            }

            // If user not found by externalId, try to find by username
            var existingUserResult = await _userRepository.GetByUsernameAsync(username);
            if (existingUserResult.IsSuccess)
            {
                // User exists with this username, update with externalId
                var user = existingUserResult.Value!;
                user.ExternalId = externalId;
                await _userRepository.UpdateAsync(user);
                var userDto = new UserDto(user.Id, user.Username, user.FullName, await _userRepository.GetRolesAsync(user.Id));
                return Result<UserDto?>.Success(userDto);
            }

            // Create new user with OAuth2 data
            var newUserResult = await _userRepository.AddOAuth2UserAsync(externalId, username, fullName);
            if (!newUserResult.IsSuccess) return Result<UserDto?>.Failure(newUserResult.Errors, newUserResult.ErrorType);

            var newUser = newUserResult.Value!;
            var newUserDto = new UserDto(newUser.Id, newUser.Username, newUser.FullName, await _userRepository.GetRolesAsync(newUser.Id));
            return Result<UserDto?>.Success(newUserDto);
        }

        public async Task<Result> MuteUserAsync(Guid targetUserId, Guid actorId, string duration, string? reason)
        {
            var userResult = await _userRepository.GetByIdAsync(targetUserId);
            if (!userResult.IsSuccess) return Result.Failure(userResult.Errors, userResult.ErrorType);

            var durationSpan = duration switch
            {
                "1h" => TimeSpan.FromHours(1),
                "6h" => TimeSpan.FromHours(6),
                "24h" => TimeSpan.FromHours(24),
                "3d" => TimeSpan.FromDays(3),
                "7d" => TimeSpan.FromDays(7),
                "30d" => TimeSpan.FromDays(30),
                _ => TimeSpan.FromHours(1)
            };

            return await _userRepository.MuteUserAsync(targetUserId, actorId, durationSpan, reason);
        }

        public async Task<Result> UnmuteUserAsync(Guid targetUserId, Guid actorId)
        {
            return await _userRepository.UnmuteUserAsync(targetUserId);
        }

        public async Task<Result<MuteInfoDto?>> GetMuteInfoAsync(Guid userId)
        {
            var muteResult = await _userRepository.GetActiveMuteAsync(userId);
            if (!muteResult.IsSuccess || muteResult.Value == null)
                return Result<MuteInfoDto?>.Success(new MuteInfoDto(false, null, null, null, null));

            var mute = muteResult.Value;
            return Result<MuteInfoDto?>.Success(new MuteInfoDto(
                true,
                mute.MutedUntil,
                mute.Reason,
                mute.MutedByUser?.Username,
                mute.CreatedAt
            ));
        }

        public async Task<Result<List<MuteInfoDto>>> GetAllActiveMutesAsync(int page = 1, int pageSize = 20)
        {
            var result = await _userRepository.GetAllActiveMutesAsync(page, pageSize);
            if (!result.IsSuccess) return Result<List<MuteInfoDto>>.Failure(result.Errors, result.ErrorType);

            var mutes = result.Value.Select(m => new MuteInfoDto(
                true,
                m.MutedUntil,
                m.Reason,
                m.MutedByUser?.Username,
                m.CreatedAt
            )).ToList();

            return Result<List<MuteInfoDto>>.Success(mutes);
        }
    }
}