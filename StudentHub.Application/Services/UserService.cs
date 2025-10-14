using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        public UserService(IUserRepository userRepository, IIdentityUserRepository identityUserRepository)
        {
            _userRepository = userRepository;
            _identityUserRepository = identityUserRepository;
        }

        public async Task<bool> CheckPasswordAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return await _identityUserRepository.CheckPasswordAsync(user, password);
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(u => new UserDto { FullName = u.FullName, Id = u.Id, Username = u.Username }).ToList();
            return userDtos;
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username
            };

            return userDto;
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username
            };

            return userDto;
        }

        public Task<bool> RegisterAsync(RegisterUserRequest request)
        {
            var user = new User
            {
                FullName = request.FullName,
                Username = request.Username
            };

            return _identityUserRepository.AddAsync(user, request.Password);
        }
    }
}
