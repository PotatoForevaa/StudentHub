using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.DTOs.Responses;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    /// <summary>
    /// Account management endpoints for authentication and user account operations.
    /// </summary>
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

        /// <summary>
        /// Register a new user account.
        /// </summary>
        /// <param name="registerRequest">User registration details (username, password, full name)</param>
        /// <returns>Registration result with user data</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Invalid registration data (validation error)</response>
        /// <response code="409">Conflict - username already exists</response>
        /// <response code="500">Server error</response>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(ApiResponse<StudentHub.Application.DTOs.Responses.UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var registerDto = new RegisterUserCommand(registerRequest.Username, registerRequest.Password, registerRequest.FullName);
            var result = await _userService.RegisterAsync(registerDto);

            return result.ToActionResult();
        }

        /// <summary>
        /// Login with username and password.
        /// </summary>
        /// <param name="loginRequest">Login credentials (username, password)</param>
        /// <returns>Login result with user data and authentication cookie</returns>
        /// <response code="200">Login successful</response>
        /// <response code="400">Invalid credentials (validation error)</response>
        /// <response code="401">Unauthorized - invalid password or user not found</response>
        /// <response code="500">Server error</response>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<StudentHub.Application.DTOs.Responses.UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var userResult = await _userService.GetByUsernameAsync(loginRequest.Username);
            if (!userResult.IsSuccess) return userResult.ToActionResult();

            var passwordResult = await _userService.CheckPasswordAsync(loginRequest.Username, loginRequest.Password);
            if (!passwordResult.IsSuccess) return passwordResult.ToActionResult();

            var user = userResult.Value;
            var userId = user.Id;

            await _authService.SignInAsync(user.Id, loginRequest.Password);

            var response = new ApiResponse<StudentHub.Application.DTOs.Responses.UserDto>
            {
                IsSuccess = true,
                Data = user
            };

            return Ok(response);
        }

        /// <summary>
        /// Logout the current user.
        /// </summary>
        /// <returns>Logout result</returns>
        /// <response code="200">Logout successful</response>
        [HttpPost("Logout")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return Ok(new ApiResponse { IsSuccess = true });
        }

        /// <summary>
        /// Get the current authenticated user's account information.
        /// </summary>
        /// <returns>User account information</returns>
        /// <response code="200">User information retrieved</response>
        /// <response code="401">Unauthorized - authentication required</response>
        [Authorize]
        [HttpGet("Me")]
        [ProducesResponseType(typeof(ApiResponse<StudentHub.Application.DTOs.Responses.UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAccountInfo()
        {
            var userResult = await _userService.GetInfoById(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
            return userResult.ToActionResult();
        }
    }
}
