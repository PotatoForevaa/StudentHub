using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.Extensions;
using StudentHub.Api.Extensions.Attributes;
using StudentHub.Application.Interfaces.UseCases;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserUseCase _userUseCase;
        private readonly IProjectUseCase _projectUseCase;
        public UsersController(IUserUseCase userUseCase, IProjectUseCase projectUseCase)
        {
            _userUseCase = userUseCase;
            _projectUseCase = projectUseCase;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var result = await _userUseCase.GetByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetUserByUsername([FromRoute] string username)
        {
            var result = await _userUseCase.GetByUsernameAsync(username);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userUseCase.GetAllAsync();
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("by-username/{username}/profile-picture")]
        public async Task<IActionResult> GetProfilePicture([FromRoute] string username)
        {
            var pictureResult = await _userUseCase.GetProfilePictureByUsername(username);
            if (!pictureResult.IsSuccess) return pictureResult.ToActionResult();
            return File(pictureResult.Value!, "image/jpeg");
        }

        [Authorize]
        [HttpPost("profile-picture")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SetProfilePicture([FileExtensions("png,jpg")][FileSize(5 * 1024 * 1024)] IFormFile file)
        {
            var pictureResult = await _userUseCase.AddProfilePicture(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!), file.OpenReadStream());
            return pictureResult.ToActionResult();
        }

        [Authorize]
        [HttpGet("{userId}/activity")]
        public async Task<IActionResult> GetUserActivity([FromRoute] Guid userId)
        {
            var result = await _projectUseCase.GetUserActivityAsync(userId);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{userId}/comments")]
        public async Task<IActionResult> GetUserComments([FromRoute] Guid userId)
        {
            var result = await _projectUseCase.GetCommentsByAuthorIdAsync(userId);
            return result.ToActionResult();
        }
    }
}
