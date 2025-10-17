using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.Interfaces.Services;
using StudentHub.Domain.Entities;
using StudentHub.Web.DTOs.Requests;
using System.Security.Claims;

namespace StudentHub.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {

        private readonly IProjectService _projectService;
        private readonly IFileStorageService _fileStorageService;
        public ProjectsController(IProjectService projectService, IFileStorageService fileStorageService)
        {
            _projectService = projectService;
            _fileStorageService = fileStorageService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] Guid id)
        {
            var projectResult = await _projectService.GetByIdAsync(id);
            if (!projectResult.IsSuccess)
            {
                switch (projectResult.ErrorType)
                {
                    case ErrorType.NotFound:
                        return NotFound(projectResult.Error);
                    default:
                        return StatusCode(500);
                }
            }

            var project = projectResult.Value;
            return Ok(project);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectService.GetAllAsync();
            return Ok(projects);
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProject(CreateProjectRequest createProjectRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var filePaths = new List<string>();
            if (createProjectRequest.Base64Images?.Count > 0)
                filePaths = await _fileStorageService.SaveImagesAsync(createProjectRequest.Base64Images);

            var createProject = new CreateProjectCommand
            {
                Name = createProjectRequest.Name,
                Description = createProjectRequest.Description,
                AuthorId = userId,
                FilePaths = filePaths,
                Url = createProjectRequest.ExternalUrl
            };

            var createResult = await _projectService.CreateAsync(createProject);
            if (!createResult.IsSuccess)
            {
                switch (createResult.ErrorType)
                {
                    case ErrorType.NotFound:
                        return NotFound(createResult.Error);
                    default:
                        return StatusCode(500);
                }
            }

            return CreatedAtAction(nameof(GetProject), new { id = createResult.Value.Id }, createResult.Value);
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProject(UpdateProjectRequest updateProjectRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var filePaths = new List<string>();
            if (updateProjectRequest.Base64Images?.Count > 0)
                filePaths = await _fileStorageService.SaveImagesAsync(updateProjectRequest.Base64Images);

            var createProject = new CreateProjectCommand
            {
                Name = updateProjectRequest.Name,
                Description = updateProjectRequest.Description,
                AuthorId = userId,
                FilePaths = filePaths,
                Url = updateProjectRequest.ExternalUrl
            };

            var createResult = await _projectService.UpdateAsync(createProject);
            if (!createResult.IsSuccess)
            {
                switch (createResult.ErrorType)
                {
                    case ErrorType.NotFound:
                        return NotFound(createResult.Error);
                    case ErrorType.Unauthorized:
                        return Unauthorized(createResult.Error);
                    default:
                        return StatusCode(500);
                }
            }
            return Ok();
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
        {
            var deleteResult = await _projectService.DeleteAsync(id);
            if (!deleteResult.IsSuccess)
            {
                switch (deleteResult.ErrorType)
                {
                    case ErrorType.NotFound:
                        return NotFound(deleteResult.Error);
                    case ErrorType.Unauthorized:
                        return Unauthorized(deleteResult.Error);
                    default:
                        return StatusCode(500);
                }
            }
            return Ok();
        }
    }
}
