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
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var projectResult = await _projectUseCase.GetByIdAsync(id, userId);
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
        [HttpGet("search")]
        public async Task<IActionResult> SearchProjects(
            [FromQuery] string? search,
            [FromQuery] Guid? categoryId,
            [FromQuery] Guid? tagId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _projectUseCase.GetFilteredProjectsAsync(search, categoryId, tagId, page, pageSize);
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
                Url: request.ExternalUrl,
                CategoryId: request.CategoryId,
                TagIds: request.TagIds
                );

            var projectResult = await _projectUseCase.CreateAsync(command);
            return projectResult.ToCreatedActionResult($"api/projects/{projectResult.Value!.Id}");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject([FromRoute] Guid id, [FromForm] UpdateProjectRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new UpdateProjectCommand(
                ProjectId: id,
                AuthorId: userId,
                Name: request.Name,
                Description: request.Description,
                ExternalUrl: request.ExternalUrl,
                Files: request.Files?.Select(f => f.OpenReadStream()).ToList(),
                CategoryId: request.CategoryId,
                TagIds: request.TagIds
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
        [HttpPost("{projectId}/comments/{commentId}/report")]
        public async Task<IActionResult> ReportComment([FromRoute] Guid projectId, [FromRoute] Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _projectUseCase.ReportCommentAsync(commentId, userId);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments([FromRoute] Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _projectUseCase.GetCommentsByProjectIdAsync(id, page, pageSize, userId);
            return result.ToActionResult();
        }

        // --- Categories ---

        [Authorize]
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _projectUseCase.GetAllCategoriesAsync();
            return result.ToActionResult();
        }

        // --- Tags ---

        [Authorize]
        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            var result = await _projectUseCase.GetAllTagsAsync();
            return result.ToActionResult();
        }

        // --- Criteria ---

        [Authorize]
        [HttpGet("categories/{categoryId}/criteria")]
        public async Task<IActionResult> GetCriteriaByCategory([FromRoute] Guid categoryId)
        {
            var result = await _projectUseCase.GetCriteriaByCategoryIdAsync(categoryId);
            return result.ToActionResult();
        }

        // --- Criterion Scores ---

        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost("{id}/scores")]
        public async Task<IActionResult> SubmitScores([FromRoute] Guid id, [FromBody] SubmitCriterionScoresRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var scores = request.Scores.Select(s => (s.CriterionId, s.Score, s.Comment)).ToList();
            var result = await _projectUseCase.SubmitCriterionScoresAsync(id, userId, scores);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}/scores")]
        public async Task<IActionResult> GetScores([FromRoute] Guid id)
        {
            var result = await _projectUseCase.GetCriterionScoresAsync(id);
            return result.ToActionResult();
        }
    }
}