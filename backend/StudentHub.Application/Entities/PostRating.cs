namespace StudentHub.Application.Entities
{
    public class PostRating
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
        public int Score { get; set; }
        public DateTime DateTime { get; set; }
    }
}
