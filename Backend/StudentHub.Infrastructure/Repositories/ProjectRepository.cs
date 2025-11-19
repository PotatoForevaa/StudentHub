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
            if (page == 0 && pageSize == 0) return await _dbContext.Projects.ToListAsync();
            return await _dbContext.Projects.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Result<Project?>> GetByIdAsync(Guid id)
        {
            var project = await _dbContext.Projects.FindAsync(id);
            if (project == null) return Result<Project?>.Failure($"Проект {id} не найден", "id", ErrorType.NotFound);
            return Result<Project?>.Success(project);
        }

        public async Task<Result<Project?>> UpdateAsync(Project project)
        {
            _dbContext.Update(project);
            await _dbContext.SaveChangesAsync();
            return Result<Project?>.Success(project);
        }
    }
}
