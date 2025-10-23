using Microsoft.EntityFrameworkCore;
using StudentHub.Application.DTOs;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Domain.Entities;
using StudentHub.Infrastructure.Data;

namespace StudentHub.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _dbContext;

        public PostRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Post?>> AddAsync(Post post)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();

            return Result<Post?>.Success(post);
        }

        public async Task<Result<Post?>> UpdateAsync(Post post)
        {
            var dbPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == post.Id);
            dbPost.Description = post.Description;
            dbPost.Title = post.Title;
            _dbContext.Posts.Update(dbPost);
            await _dbContext.SaveChangesAsync();
            return Result<Post?>.Success(post);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var post = await _dbContext.Posts.FindAsync(id);
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<Post?>> GetByIdAsync(Guid id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return Result<Post?>.Failure($"Post {id} not found", ErrorType.NotFound);
            return Result<Post?>.Success(post);
        }

        public async Task<List<Post>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            if (page == 0 && pageSize == 0) return await _dbContext.Posts.ToListAsync();
            return await _dbContext.Posts.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
