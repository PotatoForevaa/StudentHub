using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;
using StudentHub.Web.DTOs.Requests;
using System.Security.Claims;

namespace StudentHub.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {

        private readonly IProjectRepository _projectRepository;
        private readonly IFileStorageService _fileStorageService;
        public ProjectsController(IProjectRepository projectRepository, IFileStorageService fileStorageService)
        {
            _projectRepository = projectRepository;
            _fileStorageService = fileStorageService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] Guid id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null) return NotFound("Project not found");
            return Ok(project);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectRepository.GetAllAsync();
            return Ok(projects);
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProject(CreateProjectRequest createProjectRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var project = new Project()
            {
                Name = createProjectRequest.Name,
                Description = createProjectRequest.Description,
                ExternalUrl = createProjectRequest.ExternalUrl,
                AuthorId = userId,
            };

            if (createProjectRequest.Base64Images?.Count > 0)
                foreach (string base64 in createProjectRequest.Base64Images!)
                {
                    var path = await _fileStorageService.SaveImageAsync(base64);
                    project.Images.Add(new Image { Path = path });
                }

            await _projectRepository.AddAsync(project);
            return Created();
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProject(UpdateProjectRequest updateProjectRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var project = await _projectRepository.GetByIdAsync(updateProjectRequest.ProjectId);

            if (project == null) return NotFound("Project not found");
            if (userId != project.AuthorId) return Forbid();

            project.Name = updateProjectRequest.Name;
            project.Description = updateProjectRequest.Description;

            var newImages = new List<Image>();

            if (updateProjectRequest.Base64Images?.Count > 0)
                foreach (var base64 in updateProjectRequest.Base64Images)
                {
                    var path = await _fileStorageService.SaveImageAsync(base64);
                    newImages.Add(new Image { Path = path });
                }
            project.Images = newImages;

            await _projectRepository.UpdateAsync(project);
            return Ok();
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var project = await _projectRepository.GetByIdAsync(id);

            if (project == null) return NotFound("Project not found");
            if (userId != project.AuthorId) return Forbid();

            await _projectRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
