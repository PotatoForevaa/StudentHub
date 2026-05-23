using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.Extensions;
using StudentHub.Application.Interfaces.UseCases;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserUseCase _userUseCase;
        private readonly IProjectUseCase _projectUseCase;

        public AdminController(IUserUseCase userUseCase, IProjectUseCase projectUseCase)
        {
            _userUseCase = userUseCase;
            _projectUseCase = projectUseCase;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery] string? role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userUseCase.SearchAsync(search, role, page, pageSize);
            return result.ToActionResult();
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest request)
        {
            var result = await _userUseCase.ReplaceAssignableRoleAsync(id, request.Role);
            return result.ToActionResult();
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var result = await _userUseCase.DeleteAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _projectUseCase.SearchProjectsAsync(search, page, pageSize);
            return result.ToActionResult();
        }

        [HttpDelete("projects/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
        {
            var result = await _projectUseCase.DeleteAsync(id);
            return result.ToActionResult();
        }
    }
}
