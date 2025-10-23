using StudentHub.Application.DTOs;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task<Result<Post?>> AddAsync(Post post);
        Task<Result<Post?>> UpdateAsync(Post post);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<Post?>> GetByIdAsync(Guid id);
        Task<List<Post>> GetAllAsync(int page, int pagesize);
    }
}
