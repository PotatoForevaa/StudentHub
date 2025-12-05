using StudentHub.Application.DTOs;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Task<Result<Project?>> AddAsync(Project project);
        Task<Result<Project?>> UpdateAsync(Project project);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<Project?>> GetByIdAsync(Guid id);
        Task<List<Project>> GetAllAsync(int page = 0, int pageSize = 0);
        Task<Result<List<string>>> GetImageListByIdAsync(Guid id);
        Task<Result<double>> AddRatingAsync(ProjectRating rating);
    }
}
