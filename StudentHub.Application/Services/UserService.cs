using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CheckPasswordAsync(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            return await _userRepository.CheckPasswordAsync(user, password);
        }

        public async Task<UserDto?> GetByLoginAsync(string login)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null) return null;

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Login = user.Username
            };

            return userDto;
        }

        public Task RegisterAsync(RegisterUserRequest request)
        {
            var user = new User
            {
                FullName = request.FullName,
                Username = request.Username
            };

            return _userRepository.AddAsync(user, request.Password);
        }
    }
}
