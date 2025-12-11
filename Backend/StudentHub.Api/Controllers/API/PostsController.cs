using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Requests;
using StudentHub.Api.Extensions;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.Interfaces.UseCases;
using System.Security.Claims;

namespace StudentHub.Api.Controllers.API
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostUseCase _postUseCase;
        public PostsController(IPostUseCase postUseCase)
        {
            _postUseCase = postUseCase;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost([FromRoute] Guid id)
        {
            var postResult = await _postUseCase.GetByIdAsync(id);
            return postResult.ToActionResult();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var result = await _postUseCase.GetAllAsync();
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostRequest createPostRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var post = new CreatePostCommand(createPostRequest.Title, createPostRequest.Description, userId);

            var result = await _postUseCase.CreateAsync(post);
            return result.ToCreatedActionResult($"api/posts/{result.Value!.Id}");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePostRequest updatePostRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updatePost = new UpdatePostCommand(id, updatePostRequest.Title, updatePostRequest.Description, userId);

            var updateResult = await _postUseCase.UpdateAsync(updatePost, userId);
            return updateResult.ToActionResult();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _postUseCase.DeleteAsync(id, userId);
            return result.ToActionResult();
        }
    }
}
