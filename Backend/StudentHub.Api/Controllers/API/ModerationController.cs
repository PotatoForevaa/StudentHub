using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.Extensions;
using StudentHub.Application.Interfaces.UseCases;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/moderation")]
    [ApiController]
    [Authorize(Roles = "Admin,Moderator")]
    public class ModerationController : ControllerBase
    {
        private readonly IProjectUseCase _projectUseCase;

        public ModerationController(IProjectUseCase projectUseCase)
        {
            _projectUseCase = projectUseCase;
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments([FromQuery] string queue = "reported", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _projectUseCase.GetModerationCommentsAsync(queue, page, pageSize);
            return result.ToActionResult();
        }

        [HttpPut("comments/{id}/approve")]
        public async Task<IActionResult> ApproveComment([FromRoute] Guid id)
        {
            var result = await _projectUseCase.ApproveCommentAsync(id);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("comments/{id}/toxic")]
        public async Task<IActionResult> MarkCommentToxic([FromRoute] Guid id)
        {
            var result = await _projectUseCase.MarkCommentToxicAsync(id);
            return result.ToActionResult();
        }
    }
}
