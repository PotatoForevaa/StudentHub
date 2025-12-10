using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Task<Result<Project?>> AddAsync(Project project);
        Task<Result> DeleteAsync(Guid id);
        Task<List<Project>> GetAllAsync(int page = 0, int pageSize = 0);
        Task<List<Project>> GetProjectsByAuthorIdAsync(Guid authorId);
        Task<Result<Project?>> GetByIdAsync(Guid id);
        Task<Result<Project?>> UpdateAsync(Project project);
        Task<Result<List<string>>> GetImageListByIdAsync(Guid id);
        Task<Result<double>> AddRatingAsync(ProjectRating rating);
        Task<Result<double>> GetAverageRatingAsync(Guid projectId);
        Task<Result<ProjectComment>> AddCommentAsync(ProjectComment comment);
        Task<Result<List<ProjectComment>>> GetCommentsByProjectIdAsync(Guid projectId);
        Task<Result<List<ProjectComment>>> GetCommentsByAuthorIdAsync(Guid authorId);
        Task<int?> GetUserScoreForProjectAsync(Guid userId, Guid projectId);
        Result<byte[]> GetImageAsync(string path);
    }
}
