using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Guid id);
        Task<Post?> GetByIdAsync(Guid id);
        Task<List<Post>> GetAllAsync();
    }
}
