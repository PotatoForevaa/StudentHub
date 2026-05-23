using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces.UseCases
{
    public interface IProjectUseCase
    {
        Task<Result<ProjectDto?>> GetByIdAsync(Guid id);
        Task<Result<List<ProjectDto>>> GetAllAsync(int page = 0, int pageSize = 0);
        Task<Result<PaginatedResponse<AdminProjectDto>>> SearchProjectsAsync(string? search, int page, int pageSize);
        Task<Result<List<ProjectDto>>> GetProjectsByAuthorIdAsync(Guid authorId);
        Task<Result<ProjectDto?>> CreateAsync(CreateProjectCommand createProjectCommand);
        Task<Result<ProjectDto?>> UpdateAsync(UpdateProjectCommand updateProjectCommand);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<Stream?>> GetImageAsync(string path);
        Task<Result<List<String>>> GetImageListByIdAsync(Guid id);
        Task<Result<double>> AddScoreAsync(AddProjectScoreCommand command);
        Task<Result<double>> GetAverageRatingAsync(Guid projectId);
        Task<Result<ProjectCommentDto>> AddCommentAsync(CreateProjectCommentCommand command);
        Task<Result<List<ProjectCommentDto>>> GetCommentsByProjectIdAsync(Guid projectId);
        Task<Result<PaginatedResponse<ProjectCommentDto>>> GetCommentsByProjectIdAsync(Guid projectId, int page, int pageSize);
        Task<Result<List<ProjectCommentDto>>> GetCommentsByAuthorIdAsync(Guid authorId);
        Task<Result<PaginatedResponse<ProjectCommentDto>>> GetModerationCommentsAsync(string queue, int page, int pageSize);
        Task<Result<ProjectCommentDto>> ApproveCommentAsync(Guid commentId);
        Task<Result<ProjectCommentDto>> MarkCommentToxicAsync(Guid commentId);
        Task<Result<List<ActivityDto>>> GetUserActivityAsync(Guid userId);
        Task<Result<List<LeaderboardUserDto>>> GetLeaderboardAsync(string type, string period, int page, int pageSize);
    }
}
