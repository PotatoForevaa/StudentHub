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
            var userDtos = users.Select(u => new UserDto(u.Id, u.Username, u.FullName)).ToList();
            return Result<List<UserDto>>.Success(userDtos);
        }

        public async Task<Result<UserDto?>> GetByIdAsync(Guid id)
        {
            var userResult = await _userRepository.GetByIdAsync(id);
            if (!userResult.IsSuccess) return Result<UserDto?>.Failure(userResult.Errors, userResult.ErrorType);

            var user = userResult.Value;
            var userDto = new UserDto(user.Id, user.Username, user.FullName);

            return Result<UserDto?>.Success(userDto);
        }

        public async Task<Result<UserDto?>> GetByUsernameAsync(string username)
        {
            var userResult = await _userRepository.GetByUsernameAsync(username);
            if (!userResult.IsSuccess) return Result<UserDto?>.Failure(userResult.Errors, userResult.ErrorType);

            var user = userResult.Value;
            var userDto = new UserDto(user.Id, user.Username, user.FullName);

            return Result<UserDto?>.Success(userDto);
        }

        public async Task<Result<UserDto?>> LoginAsync(string username, string password)
        {
            var userResult = await _userRepository.GetByUsernameAsync(username);
            if (!userResult.IsSuccess) return Result<UserDto?>.Failure(userResult.Errors, userResult.ErrorType);

            var passwordResult = await _userRepository.CheckPasswordAsync(username, password);
            if (!passwordResult.IsSuccess) return Result<UserDto?>.Failure(passwordResult.Errors, passwordResult.ErrorType);

            var user = userResult.Value;
            var userDto = new UserDto(user.Id, user.Username, user.FullName);

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
            var userDto = new UserDto(user.Id, user.Username, user.FullName);

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
    }
}
