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
        Result<byte[]> GetImageAsync(string path);
        Task<Result<List<LeaderboardUserDto>>> GetActivityLeaderboardAsync(string period, int page, int pageSize);
        Task<Result<List<LeaderboardUserDto>>> GetRatingLeaderboardAsync(string period, int page, int pageSize);
    }
}
