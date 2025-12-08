using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces.Services
{
    public interface IProjectService
    {
        Task<Result<ProjectDto?>> GetByIdAsync(Guid id);
        Task<List<ProjectDto>> GetAllAsync(int page = 0, int pageSize = 0);
        Task<Result<ProjectDto?>> CreateAsync(CreateProjectCommand createProjectCommand);
        Task<Result<ProjectDto?>> UpdateAsync(CreateProjectCommand updateProjectCommand);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<Stream?>> GetImageAsync(string path);
        Task<Result<List<String>>> GetImageListByIdAsync(Guid id);
        Task<Result<double>> AddScoreAsync(AddProjectScoreCommand command);
        Task<Result<ProjectCommentDto>> AddCommentAsync(CreateProjectCommentCommand command);
        Task<Result<List<ProjectCommentDto>>> GetCommentsByProjectIdAsync(Guid projectId);
    }
}
