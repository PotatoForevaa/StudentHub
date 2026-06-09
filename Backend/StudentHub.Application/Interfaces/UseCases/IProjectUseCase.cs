using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces.UseCases
{
    public interface IProjectUseCase
    {
        Task<Result<ProjectDto?>> GetByIdAsync(Guid id, Guid? currentUserId = null);
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
        Task<Result<List<ProjectCommentDto>>> GetCommentsByProjectIdAsync(Guid projectId, Guid? currentUserId = null);
        Task<Result<PaginatedResponse<ProjectCommentDto>>> GetCommentsByProjectIdAsync(Guid projectId, int page, int pageSize, Guid? currentUserId = null);
        Task<Result<List<ProjectCommentDto>>> GetCommentsByAuthorIdAsync(Guid authorId);
        Task<Result> ReportCommentAsync(Guid commentId, Guid reporterId);
        Task<Result<PaginatedResponse<ProjectCommentDto>>> GetModerationCommentsAsync(string queue, int page, int pageSize);
        Task<Result<ProjectCommentDto>> ApproveCommentAsync(Guid commentId);
        Task<Result<ProjectCommentDto>> MarkCommentToxicAsync(Guid commentId);
        Task<Result<ProjectCommentDto>> AppealCommentAsync(Guid commentId, Guid userId, string? message);
        Task<Result<ProjectCommentDto>> ResolveAppealAsync(Guid commentId, bool approved);
        Task<Result<List<ActivityDto>>> GetUserActivityAsync(Guid userId);
        Task<Result<List<LeaderboardUserDto>>> GetLeaderboardAsync(string type, string period, int page, int pageSize);

        // Categories
        Task<Result<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<Result<CategoryDto>> CreateCategoryAsync(string name);
        Task<Result> DeleteCategoryAsync(Guid id);

        // Tags
        Task<Result<List<TagDto>>> GetAllTagsAsync();
        Task<Result<TagDto>> CreateTagAsync(string name);
        Task<Result> DeleteTagAsync(Guid id);

        // Criteria
        Task<Result<List<CriterionDto>>> GetCriteriaByCategoryIdAsync(Guid categoryId);
        Task<Result<CriterionDto>> CreateCriterionAsync(string name, Guid categoryId);
        Task<Result> DeleteCriterionAsync(Guid id);

        // Criterion Scores
        Task<Result> SubmitCriterionScoresAsync(Guid projectId, Guid teacherId, List<(Guid CriterionId, int Score, string? Comment)> scores);
        Task<Result<List<CriterionScoreDto>>> GetCriterionScoresAsync(Guid projectId);

        // Project filtering
        Task<Result<PaginatedResponse<ProjectDto>>> GetFilteredProjectsAsync(string? search, Guid? categoryId, Guid? tagId, int page, int pageSize);
    }
}