using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Entities;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.UseCases;

namespace StudentHub.Application.UseCases
{
    public class PostUseCase : IPostUseCase
    {
        private readonly IPostRepository _postRepository;
        public PostUseCase(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Result<PostDto?>> CreateAsync(CreatePostCommand createPostCommand)
        {
            var post = new Post
            {
                AuthorId = createPostCommand.AuthorId,
                Description = createPostCommand.Description,
                Title = createPostCommand.Title
            };

            var postResult = await _postRepository.AddAsync(post);
            if (!postResult.IsSuccess) return Result<PostDto?>.Failure(postResult.Errors, postResult.ErrorType);

            post = postResult.Value;
            var postDto = new PostDto(post.Id, post.Title, post.Description, post.AuthorId, post.CreatedAt);

            return Result<PostDto?>.Success(postDto);
        }

        public async Task<Result> DeleteAsync(Guid id, Guid userId)
        {
            var postResult = await _postRepository.GetByIdAsync(id);
            if (!postResult.IsSuccess) return Result.Failure(postResult.Errors, postResult.ErrorType);

            var post = postResult.Value;
            if (post.AuthorId != userId) return Result.Failure(new List<Error> { new Error { Message = "Forbidden", Field = "Access" } }, ErrorType.Unauthorized);

            return await _postRepository.DeleteAsync(id);
        }

        public async Task<Result<List<PostDto>>> GetAllAsync(int page = 0, int pagesize = 10)
        {
            var postListResult = await _postRepository.GetAllAsync(page, pagesize);
            if (!postListResult.IsSuccess) return Result<List<PostDto>>.Failure(postListResult.Errors, postListResult.ErrorType);

            var postList = postListResult.Value;
            var postDtos = postList
                .Select(p => new PostDto(p.Id, p.Title, p.Description, p.AuthorId, p.CreatedAt))
                .ToList();
            return Result<List<PostDto>>.Success(postDtos);
        }

        public async Task<Result<PostDto?>> GetByIdAsync(Guid id)
        {
            var postResult = await _postRepository.GetByIdAsync(id);
            if (!postResult.IsSuccess) return Result<PostDto?>.Failure(postResult.Errors, postResult.ErrorType);

            var post = postResult.Value;
            var postDto = new PostDto(post.Id, post.Title, post.Description, post.AuthorId, post.CreatedAt);

            return Result<PostDto?>.Success(postDto);
        }

        public async Task<Result<PostDto?>> UpdateAsync(UpdatePostCommand updatePostCommand, Guid userId)
        {
            var postResult = await _postRepository.GetByIdAsync(updatePostCommand.Id);
            if (!postResult.IsSuccess) return Result<PostDto?>.Failure(postResult.Errors, postResult.ErrorType);

            var post = postResult.Value;
            if (post.AuthorId != userId) return Result<PostDto?>.Failure(new List<Error> { new Error { Message = "Forbidden", Field = "Access" } }, ErrorType.Unauthorized);

            var updatePost = new Post
            {
                Id = updatePostCommand.Id,
                AuthorId = userId,
                Description = updatePostCommand.Description,
                Title = updatePostCommand.Title,
            };

            var updateResult = await _postRepository.UpdateAsync(updatePost);
            if (!updateResult.IsSuccess) return Result<PostDto?>.Failure(updateResult.Errors, updateResult.ErrorType);

            post = updateResult.Value;
            var postDto = new PostDto(post.Id, post.Title, post.Description, post.AuthorId, post.CreatedAt);
            return Result<PostDto?>.Success(postDto);
        }
    }
}
