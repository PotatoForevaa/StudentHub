using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.DTOs.Responses;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    /// <summary>
    /// Posts management endpoints for creating, retrieving, updating, and deleting posts.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Get a specific post by ID.
        /// </summary>
        /// <param name="id">Post ID (GUID)</param>
        /// <returns>Post details</returns>
        /// <response code="200">Post retrieved successfully</response>
        /// <response code="404">Post not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost([FromRoute] Guid id)
        {
            var postResult = await _postService.GetByIdAsync(id);
            return postResult.ToActionResult();
        }

        /// <summary>
        /// Get all posts.
        /// </summary>
        /// <returns>List of all posts</returns>
        /// <response code="200">Posts retrieved successfully</response>
        /// <response code="401">Unauthorized - authentication required</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PostDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postService.GetAllAsync();
            var response = new ApiResponse<List<PostDto>> { IsSuccess = true, Data = posts };
            return Ok(response);
        }

        /// <summary>
        /// Create a new post.
        /// </summary>
        /// <param name="createPostRequest">Post creation details (title, description)</param>
        /// <returns>Created post with ID</returns>
        /// <response code="200">Post created successfully</response>
        /// <response code="400">Invalid post data (validation error)</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePost(CreatePostRequest createPostRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var post = new CreatePostCommand(createPostRequest.Title, createPostRequest.Description, userId);

            var result = await _postService.CreateAsync(post);
            return result.ToActionResult();
        }

        /// <summary>
        /// Update an existing post.
        /// </summary>
        /// <param name="updatePostRequest">Post update details (post ID, title, description)</param>
        /// <returns>Updated post</returns>
        /// <response code="200">Post updated successfully</response>
        /// <response code="400">Invalid post data (validation error)</response>
        /// <response code="403">Forbidden - only post author can update</response>
        /// <response code="404">Post not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpPut("Update")]
        [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(UpdatePostRequest updatePostRequest)
        {
            var postResult = await _postService.GetByIdAsync(updatePostRequest.PostId);
            if (!postResult.IsSuccess) return postResult.ToActionResult();

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (postResult.Value.AuthorId != userId) return Forbid();

            var post = postResult.Value;

            var updatePost = new UpdatePostCommand(post.Id, updatePostRequest.Title, updatePostRequest.Description, userId);

            var updateResult = await _postService.UpdateAsync(updatePost);
            return updateResult.ToActionResult();
        }

        /// <summary>
        /// Delete a post by ID.
        /// </summary>
        /// <param name="id">Post ID (GUID)</param>
        /// <returns>Deletion result</returns>
        /// <response code="200">Post deleted successfully</response>
        /// <response code="403">Forbidden - only post author can delete</response>
        /// <response code="404">Post not found</response>
        /// <response code="401">Unauthorized - authentication required</response>
        /// <response code="500">Server error</response>
        [Authorize]
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var postResult = await _postService.GetByIdAsync(id);
            if (!postResult.IsSuccess) return postResult.ToActionResult();

            var post = postResult.Value;

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (post.AuthorId != userId) return Forbid();

            await _postService.DeleteAsync(id);
            return Ok();
        }
    }
}
