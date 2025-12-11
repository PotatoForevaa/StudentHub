namespace StudentHub.Application.Entities
{
    public class ProjectComment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AuthorId { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public User Author { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
