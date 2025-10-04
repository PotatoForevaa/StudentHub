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
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        public PostsController(IUserRepository userRepository, IPostRepository postRepository)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreatePost(CreatePostRequest createPostRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound("User not found");

            var post = new Post()
            {
                AuthorId = user.Id,
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound("User not found");

            var post = await _postRepository.GetByIdAsync(updatePostRequest.PostId);
            if (user.Id != post.AuthorId)
                return Unauthorized();

            post.Description = updatePostRequest.Description;
            post.Title = updatePostRequest.Title;

            await _postRepository.UpdateAsync(post);
            return Ok();
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound("User not found");

            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound("Post not found");

            if (user.Id != post.AuthorId)
                return Unauthorized();

            await _postRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
