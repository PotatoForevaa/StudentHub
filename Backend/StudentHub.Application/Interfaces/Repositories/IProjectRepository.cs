using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Entities;

namespace StudentHub.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Task<Result<Project?>> AddAsync(Project project);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<List<Project>>> GetAllAsync(int page = 0, int pageSize = 0);
        Task<(List<Project> Projects, int TotalCount)> SearchProjectsAsync(string? search, int page, int pageSize);
        Task<Result<List<Project>>> GetProjectsByAuthorIdAsync(Guid authorId);
        Task<Result<Project?>> GetByIdAsync(Guid id);
        Task<Result<Project?>> UpdateAsync(Project project);
        Task<Result<List<string>>> GetImageListByIdAsync(Guid id);
        Task<Result<double>> AddRatingAsync(Rating rating);
        Task<Result<double>> GetAverageRatingAsync(Guid projectId);
        Task<Result<Comment>> AddCommentAsync(Comment comment);
        Task<Result<List<Comment>>> GetCommentsByProjectIdAsync(Guid projectId, int page = 0, int pageSize = 0, bool onlyApproved = true);
        Task<int> CountCommentsByProjectIdAsync(Guid projectId, bool onlyApproved = true);
        Task<Result<List<Comment>>> GetCommentsByAuthorIdAsync(Guid authorId, int page = 0, int pageSize = 0, bool onlyApproved = true);
        Task<Result<List<Comment>>> GetCommentsByModerationStatusAsync(CommentModerationStatus status, CommentModerationOrigin? origin = null, int page = 0, int pageSize = 0);
        Task<int> CountCommentsByModerationStatusAsync(CommentModerationStatus status, CommentModerationOrigin? origin = null);
        Task<Result<List<Comment>>> GetReportedCommentsAsync(int page = 0, int pageSize = 0);
        Task<int> CountReportedCommentsAsync();
        Task<Result<Comment>> GetCommentByIdAsync(Guid commentId);
        Task<Result> UpdateCommentAsync(Comment comment);
        Task<Result<CommentReport>> AddCommentReportAsync(CommentReport report);
        Task<Result<List<Rating>>> GetRatingsByAuthorIdAsync(Guid authorId);
        Task<int?> GetUserScoreForProjectAsync(Guid userId, Guid projectId);
        Task<Result<List<Comment>>> GetCommentsWithAppealPendingAsync(int page = 1, int pageSize = 20);
        Task<int> CountCommentsWithAppealPendingAsync();
        Result<byte[]> GetImageAsync(string path);
        Task<Result<List<LeaderboardUserDto>>> GetActivityLeaderboardAsync(string period, int page, int pageSize);
        Task<Result<List<LeaderboardUserDto>>> GetRatingLeaderboardAsync(string period, int page, int pageSize);

        // Categories
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Task<Category> CreateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Guid id);

        // Tags
        Task<List<Tag>> GetAllTagsAsync();
        Task<Tag?> GetTagByIdAsync(Guid id);
        Task<Tag> CreateTagAsync(Tag tag);
        Task DeleteTagAsync(Guid id);

        // Criteria
        Task<List<Criterion>> GetCriteriaByCategoryIdAsync(Guid categoryId);
        Task<Criterion?> GetCriterionByIdAsync(Guid id);
        Task<Criterion> CreateCriterionAsync(Criterion criterion);
        Task DeleteCriterionAsync(Guid id);

        // Criterion Scores
        Task<CriterionScore> AddCriterionScoreAsync(CriterionScore score);
        Task<List<CriterionScore>> GetCriterionScoresByProjectIdAsync(Guid projectId);
        Task<List<CriterionScore>> GetCriterionScoresByProjectAndTeacherAsync(Guid projectId, Guid teacherId);

        // Project filtering
        Task<(List<Project> Projects, int TotalCount)> GetFilteredProjectsAsync(string? search, Guid? categoryId, Guid? tagId, int page, int pageSize);

        // Project categories/tags management
        Task AddProjectCategoriesAsync(Guid projectId, List<Guid> categoryIds);
        Task AddProjectTagsAsync(Guid projectId, List<Guid> tagIds);
        Task ClearProjectCategoriesAsync(Guid projectId);
        Task ClearProjectTagsAsync(Guid projectId);
    }
}