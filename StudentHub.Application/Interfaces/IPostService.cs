using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces
{
    public interface IPostService
    {
        Task<PostDto> GetPostAsync(Guid id);
        Task<List<PostDto>> GetPostDtosAsync(int page, int pagesize);
        Task CreatePostAsync(PostDto post);
        Task UpdatePostAsync(PostDto post);
        Task DeletePostAsync(Guid id);
    }
}
