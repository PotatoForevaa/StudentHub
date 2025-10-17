using Microsoft.AspNetCore.Mvc;
using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces.Services;
using StudentHub.Web.DTOs.Requests;
using StudentHub.Web.Extensions;

namespace StudentHub.Web.Controllers.API
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
            var registerDto = new RegisterUserCommand
            {
                FullName = registerRequest.FullName,
                Password = registerRequest.Password,
                Username = registerRequest.Username
            };

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
            if (!userResult.IsSuccess) return passwordResult.ToActionResult();
            
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