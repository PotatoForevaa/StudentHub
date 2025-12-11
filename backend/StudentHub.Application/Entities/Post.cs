namespace StudentHub.Application.Entities
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AuthorId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<PostRating> Ratings { get; set; } = new List<PostRating>();
    }
}
