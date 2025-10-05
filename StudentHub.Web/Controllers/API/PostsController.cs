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
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        public PostsController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost([FromRoute] Guid id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return NotFound("Post not found");
            return Ok(post);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postRepository.GetAllAsync();
            return Ok(posts);
        }   


        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreatePost(CreatePostRequest createPostRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var post = new Post()
            {
                AuthorId = userId,
                Description = createPostRequest.Description,
                Title = createPostRequest.Title
            };

            await _postRepository.AddAsync(post);
            return Created();
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdatePostRequest updatePostRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var post = await _postRepository.GetByIdAsync(updatePostRequest.PostId);

            if (post == null) return NotFound("Post not found");
            if (userId != post.AuthorId) return Forbid();

            post.Description = updatePostRequest.Description;
            post.Title = updatePostRequest.Title;

            await _postRepository.UpdateAsync(post);
            return Ok();
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null) return NotFound("Post not found");
            if (userId != post.AuthorId) return Forbid();

            await _postRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
