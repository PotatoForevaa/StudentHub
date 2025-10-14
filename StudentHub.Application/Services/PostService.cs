using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Interfaces;

namespace StudentHub.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public Task CreatePostAsync(PostDto post)
        {
            throw new NotImplementedException();
        }

        public Task DeletePostAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<PostDto> GetPostAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostDto>> GetPostDtosAsync(int page, int pagesize)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePostAsync(PostDto post)
        {
            throw new NotImplementedException();
        }
    }
}
