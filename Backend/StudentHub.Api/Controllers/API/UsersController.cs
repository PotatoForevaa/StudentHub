using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.Extensions;
using StudentHub.Api.Extensions.Attributes;
using StudentHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("ProfilePicture/{username}")]
        public async Task<IActionResult> GetProfilePicture([FromRoute] string username)
        {
            var pictureResult = await _userService.GetProfilePictureByUsername(username);
            if (!pictureResult.IsSuccess) return pictureResult.ToActionResult();
            return File(pictureResult.Value!, "image/jpeg");
        }

        [Authorize]
        [HttpPost("ProfilePicture")]
        public async Task<IActionResult> SetProfilePicture([FileExtensions("png,jpg")][FileSize(5 * 1024 * 1024)] IFormFile file)
        {
            var pictureResult = await _userService.AddProfilePicture(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!), file.OpenReadStream());
            return pictureResult.ToActionResult();
        }
    }
}
