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
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] Guid id)
        {
            var projectResult = await _projectService.GetByIdAsync(id);
            return projectResult.ToActionResult();
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

        [Authorize]
        [HttpPost("{id}/GetImageList")]
        public async Task<IActionResult> GetImageList(Guid id)
        {
            var result = await _projectService.GetImageListByIdAsync(id);
            return result.ToActionResult();
        }

        //[Authorize]
        //[HttpPost("{id}/{path}")]
        //public async Task<IActionResult> GetImage(Guid id, string path)
        //{
        //    var image = 
        //}

        //[Authorize]
        //[HttpPut("Update")]
        //public async Task<IActionResult> UpdateProject(UpdateProjectRequest updateProjectRequest)
        //{
        //    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        //    var filePaths = new List<string>();
        //    if (updateProjectRequest.Base64Images?.Count > 0)
        //        filePaths = await _fileStorageService.SaveImagesAsync(updateProjectRequest.Base64Images);

        //    var createProject = new CreateProjectCommand(updateProjectRequest.Name, updateProjectRequest.Description, userId, filePaths, updateProjectRequest.ExternalUrl);

        //    var createResult = await _projectService.UpdateAsync(createProject);
        //    return createResult.ToActionResult();
        //}

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
        {
            var deleteResult = await _projectService.DeleteAsync(id);
            return deleteResult.ToActionResult();     
        }
    }
}
