using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Guid id);
        Task<Project?> GetByIdAsync(Guid id);
        Task<List<Project>> GetAllAsync();
    }
}
