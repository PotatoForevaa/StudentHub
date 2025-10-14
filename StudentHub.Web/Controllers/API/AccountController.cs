using Microsoft.AspNetCore.Mvc;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces;
using StudentHub.Web.DTOs.Requests;

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
            var registerDto = new RegisterUserRequest
            {
                FullName = registerRequest.FullName,
                Password = registerRequest.Password,
                Username = registerRequest.Username
            };

            var result = await _userService.RegisterAsync(registerDto);
            if (result) return Created();
            return BadRequest("Something went wrong");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userService.GetByUsernameAsync(loginRequest.Username);
            if (user == null) return NotFound("User not found");

            var passwordResult = await _userService.CheckPasswordAsync(loginRequest.Username, loginRequest.Password);
            if (passwordResult == false) return Unauthorized("Wrong password");

            await _authService.SignInAsync(user.Id, user.Username);

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