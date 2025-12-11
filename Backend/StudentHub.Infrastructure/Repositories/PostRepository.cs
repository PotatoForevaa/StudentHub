using Microsoft.EntityFrameworkCore;
using StudentHub.Application.DTOs;
using StudentHub.Application.Entities;
using StudentHub.Application.Interfaces.Repositories;
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
            if (post == null) return Result<Post?>.Failure($"Пост {id} не найден", "id", ErrorType.NotFound);
            return Result<Post?>.Success(post);
        }

        public async Task<Result<List<Post>>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            var posts = page == 0 && pageSize == 0 ? await _dbContext.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync() : await _dbContext.Posts.OrderByDescending(p => p.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return Result<List<Post>>.Success(posts);
        }

        public async Task<Result<List<Post>>> GetPostsByAuthorIdAsync(Guid authorId)
        {
            var posts = await _dbContext.Posts
                .Where(p => p.AuthorId == authorId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return Result<List<Post>>.Success(posts);
        }
    }
}
