using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.Interfaces.UseCases;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectUseCase _projectUseCase;
        public ProjectsController(IProjectUseCase projectUseCase)
        {
            _projectUseCase = projectUseCase;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] Guid id)
        {
            var projectResult = await _projectUseCase.GetByIdAsync(id);
            return projectResult.ToActionResult();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var result = await _projectUseCase.GetAllAsync();
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetProjectsByAuthor([FromRoute] Guid authorId)
        {
            var result = await _projectUseCase.GetProjectsByAuthorIdAsync(authorId);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new CreateProjectCommand(
                Name: request.Name,
                Description: request.Description,
                AuthorId: userId,
                Files: request.Files?.Select(f => f.OpenReadStream()).ToList(),
                Url: request.ExternalUrl
                );

            var projectResult = await _projectUseCase.CreateAsync(command);
            return projectResult.ToCreatedActionResult($"api/projects/{projectResult.Value!.Id}");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject([FromRoute] Guid id, [FromBody] UpdateProjectRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new UpdateProjectCommand(
                ProjectId: id,
                AuthorId: userId,
                Name: request.Name,
                Description: request.Description,
                ExternalUrl: request.ExternalUrl,
                Base64Images: request.Base64Images
            );

            var result = await _projectUseCase.UpdateAsync(command);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetImages([FromRoute] Guid id)
        {
            var result = await _projectUseCase.GetImageListByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost("{id}/score")]
        public async Task<IActionResult> AddScore([FromRoute] Guid id, [FromBody] ProjectScoreRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new AddProjectScoreCommand(id, userId, request.Score);
            var result = await _projectUseCase.AddScoreAsync(command);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}/images/{filename}")]
        public async Task<IActionResult> GetImage([FromRoute] Guid id, [FromRoute] string filename)
        {
            var image = await _projectUseCase.GetImageAsync(filename);
            if (!image.IsSuccess) return image.ToActionResult();
            return File(image.Value, "image/jpeg");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
        {
            var deleteResult = await _projectUseCase.DeleteAsync(id);
            return deleteResult.ToActionResult();
        }

        [Authorize]
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment([FromRoute] Guid id, [FromBody] CreateProjectCommentRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new CreateProjectCommentCommand(id, userId, request.Content);
            var result = await _projectUseCase.AddCommentAsync(command);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments([FromRoute] Guid id)
        {
            var result = await _projectUseCase.GetCommentsByProjectIdAsync(id);
            return result.ToActionResult();
        }
    }
}
