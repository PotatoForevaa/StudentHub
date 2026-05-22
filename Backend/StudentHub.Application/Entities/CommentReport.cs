namespace StudentHub.Application.Entities
{
    public class CommentReport
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; } = null!;
        public Guid ReporterId { get; set; }
        public User Reporter { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
