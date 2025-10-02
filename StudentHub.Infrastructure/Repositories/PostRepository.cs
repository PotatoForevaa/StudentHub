using StudentHub.Application.Interfaces;
using StudentHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudentHub.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _dbContext;

        public PostRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Post post)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var post = await _dbContext.Posts.FindAsync(id);
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Post?> GetByIdAsync(Guid id) =>
            await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<List<Post>> GetAllAsync() =>
            await _dbContext.Posts.ToListAsync();
    }
}
