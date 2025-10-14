using Microsoft.EntityFrameworkCore;
using StudentHub.Application.Interfaces;
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

        public async Task AddAsync(Project project)
        {
            await _dbContext.AddAsync(project);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var project = await _dbContext.Projects.FindAsync(id);
            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Project>> GetAllAsync()
        {
            var projects = await _dbContext.Projects.ToListAsync();
            return projects;
        }

        public async Task<Project?> GetByIdAsync(Guid id)
        {
            var project = await _dbContext.Projects.FindAsync(id);
            return project;
        }

        public async Task UpdateAsync(Project project)
        {
            await _dbContext.AddAsync(project);
            await _dbContext.SaveChangesAsync();
        }
    }
}
