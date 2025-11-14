using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
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
            if (!projectResult.IsSuccess) return projectResult.ToActionResult();

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

            var createProject = new CreateProjectCommand(createProjectRequest.Name, createProjectRequest.Description, userId, filePaths, createProjectRequest.ExternalUrl);

            var createResult = await _projectService.CreateAsync(createProject);
            return createResult.ToActionResult();
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProject(UpdateProjectRequest updateProjectRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var filePaths = new List<string>();
            if (updateProjectRequest.Base64Images?.Count > 0)
                filePaths = await _fileStorageService.SaveImagesAsync(updateProjectRequest.Base64Images);

            var createProject = new CreateProjectCommand(updateProjectRequest.Name, updateProjectRequest.Description, userId, filePaths, updateProjectRequest.ExternalUrl);

            var createResult = await _projectService.UpdateAsync(createProject);
            return createResult.ToActionResult();
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
        {
            var deleteResult = await _projectService.DeleteAsync(id);
            return deleteResult.ToActionResult();     
        }
    }
}
