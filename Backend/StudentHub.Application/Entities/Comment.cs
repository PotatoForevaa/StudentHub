namespace StudentHub.Application.Entities
{
    public enum CommentModerationStatus
    {
        Pending,
        Approved,
        Toxic
    }

    public enum CommentModerationOrigin
    {
        None,
        AI,
        Human
    }

    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AuthorId { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public User Author { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public CommentModerationStatus ModerationStatus { get; set; } = CommentModerationStatus.Pending;
        public CommentModerationOrigin ModeratedBy { get; set; } = CommentModerationOrigin.None;
        public List<CommentReport> Reports { get; set; } = new();
    }
}
