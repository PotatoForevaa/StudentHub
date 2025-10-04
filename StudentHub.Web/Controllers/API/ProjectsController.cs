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

        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IFileStorageService _fileStorageService;
        public ProjectsController(IUserRepository userRepository, IProjectRepository projectRepository, IFileStorageService fileStorageService)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _fileStorageService = fileStorageService;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProject(CreateProjectRequest createProjectRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound("User not found");

            var project = new Project()
            {
                Name = createProjectRequest.Name,
                Description = createProjectRequest.Description,
                ExternalUrl = createProjectRequest.ExternalUrl
            };

            foreach (string base64 in createProjectRequest.Base64Images)
            {
                var path = await _fileStorageService.SaveImageAsync(base64);

                var image = new Image()
                {
                    Path = path
                };
                project.Images.Add(image);
            }

            await _projectRepository.AddAsync(project);
            return Created();
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProject(UpdateProjectRequest updateProjectRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound("User not found");

            var project = await _projectRepository.GetByIdAsync(updateProjectRequest.ProjectId);
            if (user.Id != project.AuthorId)
                return Unauthorized();

            project.Name = updateProjectRequest.Name;
            project.Description = updateProjectRequest.Description;

            var newImages = new List<Image>();
            foreach (var base64 in updateProjectRequest.Base64Images)
            {
                var path = await _fileStorageService.SaveImageAsync(base64);
                newImages.Add(new Image { Path = path });
            }
            project.Images = newImages;

            await _projectRepository.UpdateAsync(project);
            return Ok();
        }
    }
}
