using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;
using StudentHub.Web.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace StudentHub.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        public AccountController(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                FullName = registrationRequest.FullName,
                UserName = registrationRequest.Username
            };

            await _userRepository.AddAsync(user, registrationRequest.Password);
            return Created();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepository.GetByLoginAsync(loginRequest.Login);
            if (user == null)
                return NotFound("User not found");

            var passwordResult = await _userRepository.CheckPasswordAsync(user, loginRequest.Password);
            if (passwordResult == false)
                return Unauthorized("Wrong password");

            await _authService.SignInAsync(user);

            return Ok();
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return Ok();
        }
    }
}