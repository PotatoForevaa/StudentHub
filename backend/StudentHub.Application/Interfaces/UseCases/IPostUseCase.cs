using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces.UseCases
{
    public interface IPostUseCase
    {
        Task<Result<PostDto?>> GetByIdAsync(Guid id);
        Task<Result<List<PostDto>>> GetAllAsync(int page = 0, int pagesize = 0);
        Task<Result<PostDto?>> CreateAsync(CreatePostCommand createPostCommand);
        Task<Result<PostDto?>> UpdateAsync(UpdatePostCommand updatePostCommand, Guid userId);
        Task<Result> DeleteAsync(Guid id, Guid userId);
    }
}
