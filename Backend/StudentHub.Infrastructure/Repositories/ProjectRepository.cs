using Microsoft.EntityFrameworkCore;
using StudentHub.Application.DTOs;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Domain.Entities;
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

        public async Task<List<Project>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            if (page == 0 && pageSize == 0) return await _dbContext.Projects.Include(p => p.Images).Include(p => p.Author).ToListAsync();
            return await _dbContext.Projects.Skip((page - 1) * pageSize).Take(pageSize).Include(p => p.Images).Include(p => p.Author).ToListAsync();
        }

        public async Task<Result<Project?>> GetByIdAsync(Guid id)
        {
            var project = await _dbContext.Projects.Include(p => p.Images).Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return Result<Project?>.Failure($"Проект {id} не найден", "id", ErrorType.NotFound);
            return Result<Project?>.Success(project);
        }

        public async Task<Result<Project?>> UpdateAsync(Project project)
        {
            _dbContext.Update(project);
            await _dbContext.SaveChangesAsync();
            return Result<Project?>.Success(project);
        }

        public async Task<Result<List<string>>> GetImageListByIdAsync(Guid id)
        {
            var project = await _dbContext.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return Result<List<string>>.Failure($"Проект {id} не найден", "id", ErrorType.NotFound);
            var imageList = project.Images.Select(i => i.Path).ToList();
            return Result<List<string>>.Success(imageList);
        }

        public async Task<Result<double>> AddRatingAsync(ProjectRating rating)
        {
            // Ensure project exists
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == rating.ProjectId);
            if (project == null) return Result<double>.Failure($"Проект {rating.ProjectId} не найден", "projectId", ErrorType.NotFound);

            // Check if the user has already rated this project
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

        public async Task<Result<ProjectComment>> AddCommentAsync(ProjectComment comment)
        {
            // Ensure project exists
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == comment.ProjectId);
            if (project == null) return Result<ProjectComment>.Failure($"Проект {comment.ProjectId} не найден", "projectId", ErrorType.NotFound);

            // Ensure user exists
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == comment.AuthorId);
            if (user == null) return Result<ProjectComment>.Failure($"Пользователь {comment.AuthorId} не найден", "authorId", ErrorType.NotFound);

            await _dbContext.ProjectComments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();

            // Reload comment with author
            var commentWithAuthor = await _dbContext.ProjectComments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            return Result<ProjectComment>.Success(commentWithAuthor ?? comment);
        }

        public async Task<Result<List<ProjectComment>>> GetCommentsByProjectIdAsync(Guid projectId)
        {
            // Ensure project exists
            var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null) return Result<List<ProjectComment>>.Failure($"Проект {projectId} не найден", "projectId", ErrorType.NotFound);

            var comments = await _dbContext.ProjectComments
                .Include(c => c.Author)
                .Where(c => c.ProjectId == projectId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Result<List<ProjectComment>>.Success(comments);
        }

        public async Task<int?> GetUserScoreForProjectAsync(Guid userId, Guid projectId)
        {
            var rating = await _dbContext.ProjectRatings
                .FirstOrDefaultAsync(r => r.AuthorId == userId && r.ProjectId == projectId);

            return rating?.Score;
        }
    }
}
