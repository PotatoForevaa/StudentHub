using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost([FromRoute] Guid id)
        {
            var postResult = await _postService.GetByIdAsync(id);
            return postResult.ToActionResult();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postService.GetAllAsync();
            return Ok(posts);
        }


        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreatePost(CreatePostRequest createPostRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var post = new CreatePostCommand(createPostRequest.Title, createPostRequest.Description, userId);

            var result = await _postService.CreateAsync(post);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPut("Update")]
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

        [Authorize]
        [HttpDelete("Delete/{id}")]
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
