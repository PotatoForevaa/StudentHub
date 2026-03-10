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

        [HttpGet("oauth2/challenge")]
        public async Task<IActionResult> OAuth2Challenge(string redirectUri = null)
        {
            await _authService.ChallengeOAuth2Async(redirectUri);
            return Ok(new ApiResponse { IsSuccess = true });
        }

        [HttpGet("oauth2/callback")]
        public async Task<IActionResult> OAuth2Callback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new ApiResponse { IsSuccess = false, Errors = new List<ApiError> { new ApiError { Message = "Authorization code is required" } } });
            }

            var externalId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("preferred_username");
            var fullName = User.FindFirstValue(ClaimTypes.GivenName) ?? User.FindFirstValue("name");

            if (string.IsNullOrEmpty(externalId) || string.IsNullOrEmpty(username))
            {
                return BadRequest(new ApiResponse { IsSuccess = false, Errors = new List<ApiError> { new ApiError { Message = "Invalid OAuth2 response" } } });
            }

            var result = await _userUseCase.LoginWithOAuth2Async(externalId, username, fullName);
            if (result.IsSuccess)
            {
                var user = result.Value!;
                await _authService.SignInWithOAuth2Async(user.Id.ToString(), user.Username, user.FullName);
                return Ok(new ApiResponse<StudentHub.Application.DTOs.Responses.UserDto> { IsSuccess = true, Data = user });
            }
            else
            {
                return Unauthorized(new ApiResponse<StudentHub.Application.DTOs.Responses.UserDto> { IsSuccess = false, Errors = result.Errors.Select(e => new ApiError { Message = e.Message, Field = e.Field }).ToList(), ErrorType = result.ErrorType.ToString() });
            }
        }

        [HttpPost("oauth2/logout")]
        public async Task<IActionResult> OAuth2Logout()
        {
            await _authService.SignOutAsync();
            return Ok(new ApiResponse { IsSuccess = true });
        }
    }
}
