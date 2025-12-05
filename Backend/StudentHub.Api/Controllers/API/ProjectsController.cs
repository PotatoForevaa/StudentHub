using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.DTOs.Responses;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    /// <summary>
    /// Projects management endpoints for creating, retrieving, and managing projects.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Get a specific project by ID.
        /// </summary>
        /// <param name="id">Project ID (GUID)</param>
        /// <returns>Project details</returns>
        /// <response code="200">Project retrieved successfully</response>
        /// <response code="404">Project not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProject([FromRoute] Guid id)
        {
            var projectResult = await _projectService.GetByIdAsync(id);
            return projectResult.ToActionResult();
        }

        /// <summary>
        /// Get all projects.
        /// </summary>
        /// <returns>List of all projects</returns>
        /// <response code="200">Projects retrieved successfully</response>
        /// <response code="401">Unauthorized - authentication required</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ProjectDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectService.GetAllAsync();
            var response = new ApiResponse<List<ProjectDto>> { IsSuccess = true, Data = projects };
            return Ok(response);
        }

        /// <summary>
        /// Create a new project.
        /// </summary>
        /// <param name="request">Project creation details (name, description, files, external URL)</param>
        /// <returns>Created project with ID</returns>
        /// <response code="200">Project created successfully</response>
        /// <response code="400">Invalid project data (validation error)</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new CreateProjectCommand(
                Name: request.Name,
                Description: request.Description,
                AuthorId: userId,
                Files: request.Files?.Select(f => f.OpenReadStream()).ToList(),
                Url: request.ExternalUrl
                );

            var projectResult = await _projectService.CreateAsync(command);
            return projectResult.ToActionResult();
        }

        /// <summary>
        /// Get list of images for a specific project.
        /// </summary>
        /// <param name="id">Project ID (GUID)</param>
        /// <returns>List of image paths for the project</returns>
        /// <response code="200">Image list retrieved successfully</response>
        /// <response code="404">Project not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpPost("{id}/GetImageList")]
        [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetImageList(Guid id)
        {
            var result = await _projectService.GetImageListByIdAsync(id);
            return result.ToActionResult();
        }

        /// <summary>
        /// Add or update a score for a project (1-5).
        /// </summary>
        /// <param name="id">Project ID (GUID)</param>
        /// <param name="request">Score value (1-5)</param>
        /// <returns>New average score for the project</returns>
        /// <response code="200">Score added/updated successfully (returns new average)</response>
        /// <response code="400">Validation error (invalid score)</response>
        /// <response code="404">Project not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        [Authorize]
        [HttpPost("{id}/Score")]
        [ProducesResponseType(typeof(ApiResponse<double>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddScore([FromRoute] Guid id, [FromBody] ProjectScoreRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new AddProjectScoreCommand(id, userId, request.Score);
            var result = await _projectService.AddScoreAsync(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get a specific image from a project.
        /// </summary>
        /// <param name="id">Project ID (GUID)</param>
        /// <param name="path">Image path/name</param>
        /// <returns>Image file</returns>
        /// <response code="200">Image retrieved successfully</response>
        /// <response code="404">Image not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        [Authorize]
        [HttpGet("{id}/{path}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [Produces("image/jpeg")]
        public async Task<IActionResult> GetImage(Guid id, string path)
        {
            var image = await _projectService.GetImageAsync(path);
            if (!image.IsSuccess) return image.ToActionResult();
            return File(image.Value, "image/jpeg");
        }

        /// <summary>
        /// Delete a project by ID.
        /// </summary>
        /// <param name="id">Project ID (GUID)</param>
        /// <returns>Deletion result</returns>
        /// <response code="200">Project deleted successfully</response>
        /// <response code="404">Project not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
        {
            var deleteResult = await _projectService.DeleteAsync(id);
            return deleteResult.ToActionResult();
        }
    }
}
