using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        public PostService(IPostRepository postRepository)
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
            if (!postResult.IsSuccess) return Result<PostDto?>.Failure(postResult.Error, postResult.ErrorType);

            post = postResult.Value;
            var postDto = new PostDto
            {
                Id = post.Id,
                CreatedAt = post.CreatedAt,
                Title = post.Title,
                Description = post.Description,
                Author = post.AuthorId
            };

            return Result<PostDto?>.Success(postDto);
        }

        public async Task<Result> DeleteAsync(Guid id) => await _postRepository.DeleteAsync(id);

        public async Task<List<PostDto>> GetAllAsync(int page = 0, int pagesize = 10)
        {
            var postList = await _postRepository.GetAllAsync(page, pagesize);
            var postDtos = postList.Select(p => new PostDto
            {
                Id = p.Id,
                CreatedAt = p.CreatedAt,
                Title = p.Title,
                Description = p.Description,
                Author = p.AuthorId
            }).ToList();
            return postDtos;
        }

        public async Task<Result<PostDto?>> GetByIdAsync(Guid id)
        {
            var postResult = await _postRepository.GetByIdAsync(id);
            if (!postResult.IsSuccess) return Result<PostDto?>.Failure(postResult.Error, postResult.ErrorType);

            var post = postResult.Value;
            var postDto = new PostDto
            {
                Author = post.AuthorId,
                CreatedAt = post.CreatedAt,
                Title = post.Title,
                Description = post.Description,
                Id = post.Id,
            };

            return Result<PostDto?>.Success(postDto);
        }

        public async Task<Result<PostDto?>> UpdateAsync(CreatePostCommand createPostCommand)
        {
            var post = new Post
            {
                AuthorId = createPostCommand.AuthorId,
                Description = createPostCommand.Description,
                Title = createPostCommand.Title,
            };

            var postResult = await _postRepository.UpdateAsync(post);
            if (!postResult.IsSuccess) return Result<PostDto?>.Failure(postResult.Error, postResult.ErrorType);

            post = postResult.Value;
            var postDto = new PostDto
            {
                Author = post.AuthorId,
                CreatedAt = post.CreatedAt,
                Title = post.Title,
                Description = post.Description,
                Id = post.Id
            };
            return Result<PostDto?>.Success(postDto);
        }
    }
}
