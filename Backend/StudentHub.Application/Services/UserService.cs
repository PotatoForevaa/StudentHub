using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.Services;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        public UserService(IUserRepository userRepository, IFileStorageService fileStorageService)
        {
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result> CheckPasswordAsync(string username, string password)
        {

            var a = await _userRepository.CheckPasswordAsync(username, password);
            return a;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(u => new UserDto(u.Id, u.Username, u.FullName)).ToList();
            return userDtos;
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
            var result =  await _fileStorageService.SaveFileAsync(picture, fileName);
            return result;
        }

        public async Task<Result<Stream>> GetProfilePicture(string path)
        {
            var result = await _fileStorageService.GetFileAsync(path);
            return result;
        }
    }
}
