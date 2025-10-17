using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces.Services;
using StudentHub.Web.DTOs.Requests;
using System.Security.Claims;

namespace StudentHub.Web.Controllers.API
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
            var post = await _postService.GetByIdAsync(id);
            if (post == null) return NotFound($"Post {id} not found");
            return Ok(post);
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

            var post = new CreatePostCommand
            {
                Description = createPostRequest.Description,
                Title = createPostRequest.Title,
                AuthorId = userId,
            };

            await _postService.CreateAsync(post);
            return CreatedAtAction(nameof(GetPost), post);
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdatePostRequest updatePostRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var postResult = await _postService.GetByIdAsync(updatePostRequest.PostId);
            var post = postResult.Value;

            if (post == null) return NotFound("Post not found");
            if (userId != post.Author) return Forbid();

            var updatedPost = new CreatePostCommand
            {
                AuthorId = userId,
                Description = updatePostRequest.Description,
                Title = updatePostRequest.Title,
            };

            await _postService.UpdateAsync(updatedPost);
            return Ok();
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var postResult = await _postService.GetByIdAsync(id);
            var post = postResult.Value;

            if (post == null) return NotFound("Post not found");
            if (userId != post.Author) return Forbid();

            await _postService.DeleteAsync(id);
            return Ok();
        }
    }
}
