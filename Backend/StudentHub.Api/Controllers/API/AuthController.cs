using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.DTOs.Responses;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces.UseCases;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserUseCase _userUseCase;
        public AuthController(IAuthService authService, IUserUseCase userUseCase)
        {
            _authService = authService;
            _userUseCase = userUseCase;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var registerDto = new RegisterUserCommand(registerRequest.Username, registerRequest.Password, registerRequest.FullName);
            var result = await _userUseCase.RegisterAsync(registerDto);
            return result.ToActionResult();
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<StudentHub.Application.DTOs.Responses.UserDto>>> Login(LoginRequest loginRequest)
        {
            var loginResult = await _userUseCase.LoginAsync(loginRequest.Username, loginRequest.Password);
            if (loginResult.IsSuccess)
            {
                var user = loginResult.Value!;

                await _authService.SignInAsync(user.Id, loginRequest.Password);

                return Ok(new ApiResponse<StudentHub.Application.DTOs.Responses.UserDto> { IsSuccess = true, Data = user });
            }
            else
            {
                return Unauthorized(new ApiResponse<StudentHub.Application.DTOs.Responses.UserDto> { IsSuccess = false, Errors = loginResult.Errors.Select(e => new ApiError { Message = e.Message, Field = e.Field }).ToList(), ErrorType = loginResult.ErrorType.ToString() });
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            await _authService.SignOutAsync();
            return Ok(new ApiResponse { IsSuccess = true });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetAccountInfo()
        {
            var userResult = await _userUseCase.GetByIdAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
            return userResult.ToActionResult();
        }
    }
}
