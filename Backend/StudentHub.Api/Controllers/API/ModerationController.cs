using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.Extensions;
using StudentHub.Application.Interfaces.UseCases;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/moderation")]
    [ApiController]
    [Authorize(Roles = "Admin,Teacher")]
    public class ModerationController : ControllerBase
    {
        private readonly IProjectUseCase _projectUseCase;
        private readonly IUserUseCase _userUseCase;

        public ModerationController(IProjectUseCase projectUseCase, IUserUseCase userUseCase)
        {
            _projectUseCase = projectUseCase;
            _userUseCase = userUseCase;
        }

        // --- Comment Moderation ---

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments([FromQuery] string queue = "reported", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _projectUseCase.GetModerationCommentsAsync(queue, page, pageSize);
            return result.ToActionResult();
        }

        [HttpGet("comments/appeals")]
        public async Task<IActionResult> GetAppeals([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _projectUseCase.GetModerationCommentsAsync("appeals", page, pageSize);
            return result.ToActionResult();
        }

        [HttpPut("comments/{id}/approve")]
        public async Task<IActionResult> ApproveComment([FromRoute] Guid id)
        {
            var result = await _projectUseCase.ApproveCommentAsync(id);
            return result.ToActionResult();
        }

        [HttpPut("comments/{id}/toxic")]
        public async Task<IActionResult> MarkCommentToxic([FromRoute] Guid id)
        {
            var result = await _projectUseCase.MarkCommentToxicAsync(id);
            return result.ToActionResult();
        }

        // --- Appeal endpoints ---

        [HttpPost("comments/{id}/appeal")]
        public async Task<IActionResult> SubmitAppeal([FromRoute] Guid id, [FromBody] AppealCommentRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _projectUseCase.AppealCommentAsync(id, userId, request?.Message);
            return result.ToActionResult();
        }

        [HttpPut("comments/{id}/appeal")]
        public async Task<IActionResult> ResolveAppeal([FromRoute] Guid id, [FromBody] ResolveAppealRequest request)
        {
            var result = await _projectUseCase.ResolveAppealAsync(id, request.Approved);
            return result.ToActionResult();
        }

        // --- Mute endpoints ---

        [HttpPost("users/{userId}/mute")]
        public async Task<IActionResult> MuteUser([FromRoute] Guid userId, [FromBody] MuteUserRequest request)
        {
            var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _userUseCase.MuteUserAsync(userId, actorId, request.Duration, request.Reason);
            return result.ToActionResult();
        }

        [HttpDelete("users/{userId}/mute")]
        public async Task<IActionResult> UnmuteUser([FromRoute] Guid userId)
        {
            var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _userUseCase.UnmuteUserAsync(userId, actorId);
            return result.ToActionResult();
        }

        [HttpGet("users/{userId}/mute")]
        public async Task<IActionResult> GetMuteStatus([FromRoute] Guid userId)
        {
            var result = await _userUseCase.GetMuteInfoAsync(userId);
            return result.ToActionResult();
        }

        [HttpGet("mutes")]
        public async Task<IActionResult> GetActiveMutes([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _userUseCase.GetAllActiveMutesAsync(page, pageSize);
            return result.ToActionResult();
        }
    }
}
