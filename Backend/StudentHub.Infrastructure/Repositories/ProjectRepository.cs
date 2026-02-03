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
            var project = await _dbContext.Projects.FindAsync(id);
            if (project == null) return Result.Failure($"Проект {id} не найден", "id", ErrorType.NotFound);
            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<List<Project>>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            var projects = page == 0 && pageSize == 0
                ? await _dbContext.Projects.Include(p => p.Images).Include(p => p.Author).OrderByDescending(p => p.CreatedAt).ToListAsync()
                : await _dbContext.Projects.Skip((page - 1) * pageSize).Take(pageSize).Include(p => p.Images).Include(p => p.Author).ToListAsync();
            return Result<List<Project>>.Success(projects);
        }

        public async Task<Result<List<Project>>> GetProjectsByAuthorIdAsync(Guid authorId)
        {
            var projects = await _dbContext.Projects
                .Where(p => p.AuthorId == authorId)
                .Include(p => p.Images)
                .Include(p => p.Author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return Result<List<Project>>.Success(projects);
        }

        public async Task<Result<Project?>> GetByIdAsync(Guid id)
        {
            var project = await _dbContext.Projects.Include(p => p.Images).Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == id);
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
            var project = await _dbContext.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return Result<List<string>>.Failure($"Проект {id} не найден", "id", ErrorType.NotFound);
            var imageList = project.Images.Select(i => i.Path).ToList();
            return Result<List<string>>.Success(imageList);
        }

        public async Task<Result<double>> AddRatingAsync(Rating rating)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == rating.ProjectId);
            if (project == null) return Result<double>.Failure($"Проект {rating.ProjectId} не найден", "projectId", ErrorType.NotFound);

            var existing = await _dbContext.ProjectRatings.FirstOrDefaultAsync(r => r.ProjectId == rating.ProjectId && r.AuthorId == rating.AuthorId);
            if (existing != null)
            {
                existing.Score = rating.Score;
                existing.DateTime = DateTime.UtcNow;
                _dbContext.ProjectRatings.Update(existing);
            }
            else
            {
                await _dbContext.ProjectRatings.AddAsync(rating);
            }

            await _dbContext.SaveChangesAsync();

            var avg = await _dbContext.ProjectRatings.Where(r => r.ProjectId == rating.ProjectId).AverageAsync(r => r.Score);
            return Result<double>.Success(avg);
        }

        public async Task<Result<double>> GetAverageRatingAsync(Guid projectId)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null) return Result<double>.Failure($"Проект {projectId} не найден", "projectId", ErrorType.NotFound);

            var ratings = await _dbContext.ProjectRatings.Where(r => r.ProjectId == projectId).ToListAsync();
            var avg = ratings.Any() ? ratings.Average(r => r.Score) : 0.0;
            return Result<double>.Success(avg);
        }

        public async Task<Result<Comment>> AddCommentAsync(Comment comment)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == comment.ProjectId);
            if (project == null) return Result<Comment>.Failure($"Проект {comment.ProjectId} не найден", "projectId", ErrorType.NotFound);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == comment.AuthorId);
            if (user == null) return Result<Comment>.Failure($"Пользователь {comment.AuthorId} не найден", "authorId", ErrorType.NotFound);

            await _dbContext.ProjectComments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();

            var commentWithAuthor = await _dbContext.ProjectComments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            return Result<Comment>.Success(commentWithAuthor ?? comment);
        }

        public async Task<Result<List<Comment>>> GetCommentsByProjectIdAsync(Guid projectId)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null) return Result<List<Comment>>.Failure($"Проект {projectId} не найден", "projectId", ErrorType.NotFound);

            var comments = await _dbContext.ProjectComments
                .Include(c => c.Author)
                .Where(c => c.ProjectId == projectId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Result<List<Comment>>.Success(comments);
        }

        public async Task<int?> GetUserScoreForProjectAsync(Guid userId, Guid projectId)
        {
            var rating = await _dbContext.ProjectRatings
                .FirstOrDefaultAsync(r => r.AuthorId == userId && r.ProjectId == projectId);

            return rating?.Score;
        }

        public async Task<Result<List<Comment>>> GetCommentsByAuthorIdAsync(Guid authorId)
        {
            var comments = await _dbContext.ProjectComments
                .Include(c => c.Author)
                .Include(c => c.Project)
                .Where(c => c.AuthorId == authorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Result<List<Comment>>.Success(comments);
        }

        public async Task<Result<List<Rating>>> GetRatingsByAuthorIdAsync(Guid authorId)
        {
            var ratings = await _dbContext.ProjectRatings
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

            var ratingCounts = await _dbContext.ProjectRatings
                .Where(r => r.DateTime >= start && r.DateTime <= end)
                .GroupBy(r => r.AuthorId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            var commentCounts = await _dbContext.ProjectComments
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
                    AvgRating = _dbContext.ProjectRatings
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
    }
}
