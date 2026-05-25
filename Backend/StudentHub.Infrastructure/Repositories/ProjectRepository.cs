using Microsoft.EntityFrameworkCore;
using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Entities;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Infrastructure.Data;

namespace StudentHub.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _dbContext;
        public ProjectRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Project?>> AddAsync(Project project)
        {
            await _dbContext.AddAsync(project);
            await _dbContext.SaveChangesAsync();
            var resultProject = await GetByIdAsync(project.Id);
            return Result<Project?>.Success(project);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var project = await _dbContext.Projects
                .Include(p => p.Attachments)
                .Include(p => p.Ratings)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Reports)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return Result.Failure($"Проект {id} не найден", "id", ErrorType.NotFound);

            _dbContext.CommentReports.RemoveRange(project.Comments.SelectMany(c => c.Reports));
            _dbContext.Comments.RemoveRange(project.Comments);
            _dbContext.Ratings.RemoveRange(project.Ratings);
            _dbContext.Attachments.RemoveRange(project.Attachments);
            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<List<Project>>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            var projects = page == 0 && pageSize == 0
                ? await _dbContext.Projects.Include(p => p.Attachments).Include(p => p.Author).Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).Include(p => p.ProjectTags).ThenInclude(pt => pt.Tag).OrderByDescending(p => p.CreatedAt).ToListAsync()
                : await _dbContext.Projects.Skip((page - 1) * pageSize).Take(pageSize).Include(p => p.Attachments).Include(p => p.Author).Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).Include(p => p.ProjectTags).ThenInclude(pt => pt.Tag).ToListAsync();
            return Result<List<Project>>.Success(projects);
        }

        public async Task<(List<Project> Projects, int TotalCount)> SearchProjectsAsync(string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _dbContext.Projects
                .Include(p => p.Author)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToUpper();
                query = query.Where(p =>
                    p.Name.ToUpper().Contains(term) ||
                    p.Description.ToUpper().Contains(term) ||
                    p.Author.Username.ToUpper().Contains(term) ||
                    p.Author.FullName.ToUpper().Contains(term));
            }

            var total = await query.CountAsync();
            var projects = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (projects, total);
        }

        public async Task<Result<List<Project>>> GetProjectsByAuthorIdAsync(Guid authorId)
        {
            var projects = await _dbContext.Projects
                .Where(p => p.AuthorId == authorId)
                .Include(p => p.Attachments)
                .Include(p => p.Author)
                .Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category)
                .Include(p => p.ProjectTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return Result<List<Project>>.Success(projects);
        }

        public async Task<Result<Project?>> GetByIdAsync(Guid id)
        {
            var project = await _dbContext.Projects
                .Include(p => p.Attachments)
                .Include(p => p.Author)
                .Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category)
                .Include(p => p.ProjectTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.CriterionScores).ThenInclude(cs => cs.Criterion)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return Result<Project?>.Failure($"Проект {id} не найден", "id", ErrorType.NotFound);
            return Result<Project?>.Success(project);
        }

        public async Task<Result<Project>> UpdateAsync(Project project)
        {
            await _dbContext.SaveChangesAsync();
            return Result<Project>.Success(project);
        }

        public async Task<Result<List<string>>> GetImageListByIdAsync(Guid id)
        {
            var project = await _dbContext.Projects.Include(p => p.Attachments).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return Result<List<string>>.Failure($"Проект {id} не найден", "id", ErrorType.NotFound);
            var imageList = project.Attachments.Select(i => i.Path).ToList();
            return Result<List<string>>.Success(imageList);
        }

        public async Task<Result<double>> AddRatingAsync(Rating rating)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == rating.ProjectId);
            if (project == null) return Result<double>.Failure($"Проект {rating.ProjectId} не найден", "projectId", ErrorType.NotFound);

            var existing = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.ProjectId == rating.ProjectId && r.AuthorId == rating.AuthorId);
            if (existing != null)
            {
                existing.Score = rating.Score;
                existing.DateTime = DateTime.UtcNow;
                _dbContext.Ratings.Update(existing);
            }
            else
            {
                await _dbContext.Ratings.AddAsync(rating);
            }

            await _dbContext.SaveChangesAsync();

            var avg = await _dbContext.Ratings.Where(r => r.ProjectId == rating.ProjectId).AverageAsync(r => r.Score);
            return Result<double>.Success(avg);
        }

        public async Task<Result<double>> GetAverageRatingAsync(Guid projectId)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null) return Result<double>.Failure($"Проект {projectId} не найден", "projectId", ErrorType.NotFound);

            var ratings = await _dbContext.Ratings.Where(r => r.ProjectId == projectId).ToListAsync();
            var avg = ratings.Any() ? ratings.Average(r => r.Score) : 0.0;
            return Result<double>.Success(avg);
        }

        public async Task<Result<Comment>> AddCommentAsync(Comment comment)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == comment.ProjectId);
            if (project == null) return Result<Comment>.Failure($"Проект {comment.ProjectId} не найден", "projectId", ErrorType.NotFound);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == comment.AuthorId);
            if (user == null) return Result<Comment>.Failure($"Пользователь {comment.AuthorId} не найден", "authorId", ErrorType.NotFound);

            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();

            var commentWithAuthor = await _dbContext.Comments
                .Include(c => c.Author)
                .Include(c => c.Reports)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            return Result<Comment>.Success(commentWithAuthor ?? comment);
        }

        public async Task<Result<List<Comment>>> GetCommentsByProjectIdAsync(Guid projectId, int page = 0, int pageSize = 0, bool onlyApproved = true)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null) return Result<List<Comment>>.Failure($"Проект {projectId} не найден", "projectId", ErrorType.NotFound);

            var query = _dbContext.Comments
                .Include(c => c.Author)
                .Include(c => c.Reports)
                .Where(c => c.ProjectId == projectId);

            if (onlyApproved)
            {
                query = query.Where(c =>
                    c.ModerationStatus == CommentModerationStatus.Approved &&
                    !c.Reports.Any());
            }

            query = query.OrderByDescending(c => c.CreatedAt);

            var comments = page <= 0 || pageSize <= 0
                ? await query.ToListAsync()
                : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Result<List<Comment>>.Success(comments);
        }

        public async Task<int> CountCommentsByProjectIdAsync(Guid projectId, bool onlyApproved = true)
        {
            var query = _dbContext.Comments
                .Include(c => c.Reports)
                .Where(c => c.ProjectId == projectId);

            if (onlyApproved)
            {
                query = query.Where(c =>
                    c.ModerationStatus == CommentModerationStatus.Approved &&
                    !c.Reports.Any());
            }

            return await query.CountAsync();
        }

        public async Task<int?> GetUserScoreForProjectAsync(Guid userId, Guid projectId)
        {
            var rating = await _dbContext.Ratings
                .FirstOrDefaultAsync(r => r.AuthorId == userId && r.ProjectId == projectId);

            return rating?.Score;
        }

        public async Task<Result<List<Comment>>> GetCommentsByAuthorIdAsync(Guid authorId, int page = 0, int pageSize = 0, bool onlyApproved = true)
        {
            var query = _dbContext.Comments
                .Include(c => c.Author)
                .Include(c => c.Project)
                .Include(c => c.Reports)
                .Where(c => c.AuthorId == authorId);

            if (onlyApproved)
            {
                query = query.Where(c =>
                    c.ModerationStatus == CommentModerationStatus.Approved &&
                    !c.Reports.Any());
            }

            query = query.OrderByDescending(c => c.CreatedAt);

            var comments = page <= 0 || pageSize <= 0
                ? await query.ToListAsync()
                : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Result<List<Comment>>.Success(comments);
        }

        public async Task<Result<List<Comment>>> GetCommentsByModerationStatusAsync(CommentModerationStatus status, CommentModerationOrigin? origin = null, int page = 0, int pageSize = 0)
        {
            var query = _dbContext.Comments
                .Include(c => c.Author)
                .Include(c => c.Project)
                .Include(c => c.Reports)
                .Where(c => c.ModerationStatus == status);

            if (origin.HasValue)
            {
                query = query.Where(c => c.ModeratedBy == origin.Value);
            }

            query = query.OrderByDescending(c => c.CreatedAt);

            var comments = page <= 0 || pageSize <= 0
                ? await query.ToListAsync()
                : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Result<List<Comment>>.Success(comments);
        }

        public async Task<int> CountCommentsByModerationStatusAsync(CommentModerationStatus status, CommentModerationOrigin? origin = null)
        {
            var query = _dbContext.Comments.Where(c => c.ModerationStatus == status);
            if (origin.HasValue)
            {
                query = query.Where(c => c.ModeratedBy == origin.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Result<List<Comment>>> GetReportedCommentsAsync(int page = 0, int pageSize = 0)
        {
            var query = _dbContext.Comments
                .Include(c => c.Author)
                .Include(c => c.Project)
                .Include(c => c.Reports)
                .Where(c => c.Reports.Any())
                .OrderByDescending(c => c.CreatedAt);

            var comments = page <= 0 || pageSize <= 0
                ? await query.ToListAsync()
                : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Result<List<Comment>>.Success(comments);
        }

        public async Task<int> CountReportedCommentsAsync()
        {
            return await _dbContext.Comments.CountAsync(c => c.Reports.Any());
        }

        public async Task<Result<Comment>> GetCommentByIdAsync(Guid commentId)
        {
            var comment = await _dbContext.Comments
                .Include(c => c.Author)
                .Include(c => c.Project)
                .Include(c => c.Reports)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null) return Result<Comment>.Failure($"Комментарий {commentId} не найден", "commentId", ErrorType.NotFound);
            return Result<Comment>.Success(comment);
        }

        public async Task<Result> UpdateCommentAsync(Comment comment)
        {
            var existingReports = await _dbContext.CommentReports
                .Where(r => r.CommentId == comment.Id)
                .ToListAsync();

            if (comment.Reports.Count == 0 && existingReports.Count > 0)
            {
                _dbContext.CommentReports.RemoveRange(existingReports);
            }

            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<CommentReport>> AddCommentReportAsync(CommentReport report)
        {
            var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == report.CommentId);
            if (comment == null) return Result<CommentReport>.Failure($"Комментарий {report.CommentId} не найден", "commentId", ErrorType.NotFound);

            var reporter = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == report.ReporterId);
            if (reporter == null) return Result<CommentReport>.Failure($"Пользователь {report.ReporterId} не найден", "reporterId", ErrorType.NotFound);

            if (await _dbContext.CommentReports.AnyAsync(r => r.CommentId == report.CommentId && r.ReporterId == report.ReporterId))
            {
                return Result<CommentReport>.Failure("Comment already reported by this user", "reporterId", ErrorType.Conflict);
            }

            await _dbContext.CommentReports.AddAsync(report);
            await _dbContext.SaveChangesAsync();
            return Result<CommentReport>.Success(report);
        }

        public async Task<Result<List<Rating>>> GetRatingsByAuthorIdAsync(Guid authorId)
        {
            var ratings = await _dbContext.Ratings
                .Include(r => r.Project)
                .Where(r => r.AuthorId == authorId)
                .OrderByDescending(r => r.DateTime)
                .ToListAsync();

            return Result<List<Rating>>.Success(ratings);
        }

        public Result<byte[]> GetImageAsync(string path)
        {
            try
            {
                return Result<byte[]>.Success(File.ReadAllBytes(path));
            }
            catch
            {
                return Result<byte[]>.Failure("File not found");
            }
        }

        private (DateTime Start, DateTime End) GetDateRange(string period)
        {
            var now = DateTime.UtcNow;
            return period switch
            {
                "weekly-current" => (now.AddDays(-(int)now.DayOfWeek + 1), now.AddDays(7 - (int)now.DayOfWeek)),
                "weekly-last" => (now.AddDays(-(int)now.DayOfWeek + 1 - 7), now.AddDays(-(int)now.DayOfWeek)),
                "monthly-current" => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddDays(-1)),
                "monthly-last" => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1), new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(-1)),
                "yearly-current" => (new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(now.Year, 12, 31, 23, 59, 59, DateTimeKind.Utc)),
                _ => throw new ArgumentException("Invalid period")
            };
        }

        public async Task<Result<List<LeaderboardUserDto>>> GetActivityLeaderboardAsync(string period, int page, int pageSize)
        {
            var (start, end) = GetDateRange(period);

            var projectCounts = await _dbContext.Projects
                .Where(p => p.CreatedAt >= start && p.CreatedAt <= end)
                .GroupBy(p => p.AuthorId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            var ratingCounts = await _dbContext.Ratings
                .Where(r => r.DateTime >= start && r.DateTime <= end)
                .GroupBy(r => r.AuthorId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            var commentCounts = await _dbContext.Comments
                .Where(c => c.CreatedAt >= start && c.CreatedAt <= end)
                .GroupBy(c => c.AuthorId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            var userActivities = new List<(Guid UserId, string FullName, int ActivityCount)>();

            foreach (var user in _dbContext.Users)
            {
                var activityCount = (projectCounts.GetValueOrDefault(user.Id, 0) +
                                     ratingCounts.GetValueOrDefault(user.Id, 0) +
                                     commentCounts.GetValueOrDefault(user.Id, 0));

                if (activityCount > 0)
                {
                    userActivities.Add((user.Id, user.FullName, activityCount));
                }
            }

            var leaderboard = userActivities
                .OrderByDescending(x => x.ActivityCount)
                .ThenBy(x => x.FullName)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(x =>
                {
                    var user = _dbContext.Users.FirstOrDefault(u => u.Id == x.UserId);
                    var profilePicturePath = user?.ProfilePicturePath;
                    if (string.IsNullOrEmpty(profilePicturePath)) profilePicturePath = "default-pfp.png";
                    return new LeaderboardUserDto(x.UserId, x.FullName, $"api/users/by-username/{user?.Username ?? "default"}/profile-picture", x.ActivityCount);
                })
                .ToList();

            return Result<List<LeaderboardUserDto>>.Success(leaderboard);
        }

        public async Task<Result<List<LeaderboardUserDto>>> GetRatingLeaderboardAsync(string period, int page, int pageSize)
        {
            var (start, end) = GetDateRange(period);

            var projectRatings = await _dbContext.Projects
                .Where(p => p.CreatedAt >= start && p.CreatedAt <= end)
                .Select(p => new
                {
                    AuthorId = p.AuthorId,
                    AvgRating = _dbContext.Ratings
                        .Where(r => r.ProjectId == p.Id)
                        .Average(r => (double?)r.Score) ?? 0.0
                })
                .Where(x => x.AvgRating > 0)
                .ToListAsync();

            var userAverages = projectRatings
                .GroupBy(x => x.AuthorId)
                .Select(g => new
                {
                    UserId = g.Key,
                    AvgRating = g.Average(x => x.AvgRating)
                })
                .ToDictionary(x => x.UserId, x => x.AvgRating);

            var userIds = userAverages.Keys.ToList();
            var users = await _dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            var leaderboard = users
                .Select(u =>
                {
                    var user = _dbContext.Users.FirstOrDefault(us => us.Id == u.Id);
                    var profilePicturePath = user?.ProfilePicturePath;
                    if (string.IsNullOrEmpty(profilePicturePath)) profilePicturePath = "default-pfp.png";
                    return new LeaderboardUserDto(u.Id, u.FullName, $"api/users/by-username/{user?.Username ?? "default"}/profile-picture", userAverages[u.Id]);
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.FullName)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToList();

            return Result<List<LeaderboardUserDto>>.Success(leaderboard);
        }

        public async Task<Result<List<Comment>>> GetCommentsWithAppealPendingAsync(int page = 1, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var comments = await _dbContext.Comments
                .Include(c => c.Author)
                .Include(c => c.Project)
                .Include(c => c.Reports)
                .Where(c => c.AppealStatus == CommentAppealStatus.Pending)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<List<Comment>>.Success(comments);
        }

        public async Task<int> CountCommentsWithAppealPendingAsync()
        {
            return await _dbContext.Comments.CountAsync(c => c.AppealStatus == CommentAppealStatus.Pending);
        }

        // --- New methods for Categories, Tags, Criteria, CriterionScores, Filtering ---

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await _dbContext.Categories.Include(c => c.Criteria).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category != null)
            {
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _dbContext.Tags.ToListAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(Guid id)
        {
            return await _dbContext.Tags.FindAsync(id);
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();
            return tag;
        }

        public async Task DeleteTagAsync(Guid id)
        {
            var tag = await _dbContext.Tags.FindAsync(id);
            if (tag != null)
            {
                _dbContext.Tags.Remove(tag);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Criterion>> GetCriteriaByCategoryIdAsync(Guid categoryId)
        {
            return await _dbContext.Criteria
                .Include(c => c.Category)
                .Where(c => c.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Criterion?> GetCriterionByIdAsync(Guid id)
        {
            return await _dbContext.Criteria.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Criterion> CreateCriterionAsync(Criterion criterion)
        {
            _dbContext.Criteria.Add(criterion);
            await _dbContext.SaveChangesAsync();
            return criterion;
        }

        public async Task DeleteCriterionAsync(Guid id)
        {
            var criterion = await _dbContext.Criteria.FindAsync(id);
            if (criterion != null)
            {
                _dbContext.Criteria.Remove(criterion);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<CriterionScore> AddCriterionScoreAsync(CriterionScore score)
        {
            _dbContext.CriterionScores.Add(score);
            await _dbContext.SaveChangesAsync();
            return score;
        }

        public async Task<List<CriterionScore>> GetCriterionScoresByProjectIdAsync(Guid projectId)
        {
            return await _dbContext.CriterionScores
                .Include(cs => cs.Criterion)
                .Where(cs => cs.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<List<CriterionScore>> GetCriterionScoresByProjectAndTeacherAsync(Guid projectId, Guid teacherId)
        {
            return await _dbContext.CriterionScores
                .Include(cs => cs.Criterion)
                .Where(cs => cs.ProjectId == projectId && cs.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<(List<Project> Projects, int TotalCount)> GetFilteredProjectsAsync(string? search, Guid? categoryId, Guid? tagId, int page, int pageSize)
        {
            var query = _dbContext.Projects
                .Include(p => p.Author)
                .Include(p => p.Attachments)
                .Include(p => p.ProjectCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProjectTags)
                    .ThenInclude(pt => pt.Tag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToUpper();
                query = query.Where(p =>
                    p.Name.ToUpper().Contains(term) ||
                    p.Description.ToUpper().Contains(term) ||
                    p.Author.Username.ToUpper().Contains(term) ||
                    p.Author.FullName.ToUpper().Contains(term));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ProjectCategories.Any(pc => pc.CategoryId == categoryId.Value));
            }

            if (tagId.HasValue)
            {
                query = query.Where(p => p.ProjectTags.Any(pt => pt.TagId == tagId.Value));
            }

            var total = await query.CountAsync();
            var projects = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (projects, total);
        }

        public async Task AddProjectCategoriesAsync(Guid projectId, List<Guid> categoryIds)
        {
            var categories = await _dbContext.Categories.Where(c => categoryIds.Contains(c.Id)).ToListAsync();
            foreach (var category in categories)
            {
                _dbContext.ProjectCategories.Add(new ProjectCategory { ProjectId = projectId, CategoryId = category.Id });
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddProjectTagsAsync(Guid projectId, List<Guid> tagIds)
        {
            var tags = await _dbContext.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
            foreach (var tag in tags)
            {
                _dbContext.ProjectTags.Add(new ProjectTag { ProjectId = projectId, TagId = tag.Id });
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task ClearProjectCategoriesAsync(Guid projectId)
        {
            var existing = await _dbContext.ProjectCategories.Where(pc => pc.ProjectId == projectId).ToListAsync();
            _dbContext.ProjectCategories.RemoveRange(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ClearProjectTagsAsync(Guid projectId)
        {
            var existing = await _dbContext.ProjectTags.Where(pt => pt.ProjectId == projectId).ToListAsync();
            _dbContext.ProjectTags.RemoveRange(existing);
            await _dbContext.SaveChangesAsync();
        }
    }
}