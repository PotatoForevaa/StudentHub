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

        // --- Categories ---

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _projectUseCase.GetAllCategoriesAsync();
            return result.ToActionResult();
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var result = await _projectUseCase.CreateCategoryAsync(request.Name);
            return result.ToCreatedActionResult($"api/admin/categories/{result.Value?.Id}");
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var result = await _projectUseCase.DeleteCategoryAsync(id);
            return result.ToActionResult();
        }

        // --- Tags management ---

        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            var result = await _projectUseCase.GetAllTagsAsync();
            return result.ToActionResult();
        }

        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {
            var result = await _projectUseCase.CreateTagAsync(request.Name);
            return result.ToCreatedActionResult($"api/admin/tags/{result.Value?.Id}");
        }

        [HttpDelete("tags/{id}")]
        public async Task<IActionResult> DeleteTag([FromRoute] Guid id)
        {
            var result = await _projectUseCase.DeleteTagAsync(id);
            return result.ToActionResult();
        }

        // --- Criteria management ---

        [HttpGet("criteria")]
        public async Task<IActionResult> GetAllCriteria([FromQuery] Guid? categoryId)
        {
            if (categoryId.HasValue)
            {
                var result = await _projectUseCase.GetCriteriaByCategoryIdAsync(categoryId.Value);
                return result.ToActionResult();
            }
            
            return Ok();
        }

        [HttpPost("criteria")]
        public async Task<IActionResult> CreateCriterion([FromBody] CreateCriterionRequest request)
        {
            var result = await _projectUseCase.CreateCriterionAsync(request.Name, request.CategoryId);
            return result.ToCreatedActionResult($"api/admin/criteria/{result.Value?.Id}");
        }

        [HttpDelete("criteria/{id}")]
        public async Task<IActionResult> DeleteCriterion([FromRoute] Guid id)
        {
            var result = await _projectUseCase.DeleteCriterionAsync(id);
            return result.ToActionResult();
        }
    }
}