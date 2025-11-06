using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces.Services;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        public AccountController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var registerDto = new RegisterUserCommand(registerRequest.Username, registerRequest.Password, registerRequest.FullName);
            var result = await _userService.RegisterAsync(registerDto);
            if (result.IsSuccess) return Created();

            return BadRequest(result.Error);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var passwordResult = await _userService.CheckPasswordAsync(loginRequest.Username, loginRequest.Password);
            if (!passwordResult.IsSuccess) return passwordResult.ToActionResult();

            var userResult = await _userService.GetByUsernameAsync(loginRequest.Username);
            if (!userResult.IsSuccess) return userResult.ToActionResult();

            var user = userResult.Value;
            var userId = user.Id;

            await _authService.SignInAsync(user.Id, loginRequest.Password);

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