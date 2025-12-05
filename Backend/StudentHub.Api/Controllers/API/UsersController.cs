using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Responses;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Api.Extensions;
using StudentHub.Api.Extensions.Attributes;
using StudentHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    /// <summary>
    /// Users management endpoints for retrieving user information and managing profiles.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get a specific user by ID (Admin only).
        /// </summary>
        /// <param name="id">User ID (GUID)</param>
        /// <returns>User details</returns>
        /// <response code="200">User retrieved successfully</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get all users (Admin only).
        /// </summary>
        /// <returns>List of all users</returns>
        /// <response code="200">Users retrieved successfully</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="401">Unauthorized - authentication required</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            var response = new ApiResponse<List<UserDto>> { IsSuccess = true, Data = users };
            return Ok(response);
        }

        /// <summary>
        /// Get a user's profile picture by username.
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Profile picture image file</returns>
        /// <response code="200">Profile picture retrieved successfully</response>
        /// <response code="404">Profile picture not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        [Authorize]
        [HttpGet("ProfilePicture/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [Produces("image/jpeg")]
        public async Task<IActionResult> GetProfilePicture([FromRoute] string username)
        {
            var pictureResult = await _userService.GetProfilePictureByUsername(username);
            if (!pictureResult.IsSuccess) return pictureResult.ToActionResult();
            return File(pictureResult.Value!, "image/jpeg");
        }

        /// <summary>
        /// Upload or update user's profile picture.
        /// </summary>
        /// <param name="file">Profile picture file (PNG or JPG, max 5MB)</param>
        /// <returns>Upload result</returns>
        /// <response code="200">Profile picture updated successfully</response>
        /// <response code="400">Invalid file format or size (validation error)</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpPost("ProfilePicture")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SetProfilePicture([FileExtensions("png,jpg")][FileSize(5 * 1024 * 1024)] IFormFile file)
        {
            var pictureResult = await _userService.AddProfilePicture(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!), file.OpenReadStream());
            return pictureResult.ToActionResult();
        }
    }
}
