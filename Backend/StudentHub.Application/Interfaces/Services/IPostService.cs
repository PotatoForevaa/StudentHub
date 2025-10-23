using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<Result<PostDto?>> GetByIdAsync(Guid id);
        Task<List<PostDto>> GetAllAsync(int page = 0, int pagesize = 0);
        Task<Result<PostDto?>> CreateAsync(CreatePostCommand createPostCommand);
        Task<Result<PostDto?>> UpdateAsync(UpdatePostCommand updatePostCommand);
        Task<Result> DeleteAsync(Guid id);
    }
}
