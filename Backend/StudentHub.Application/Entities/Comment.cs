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

    public enum CommentAppealStatus
    {
        None,
        Pending,
        Approved,
        Rejected
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
        public DateTime? LastEditedAt { get; set; }
        public CommentModerationStatus ModerationStatus { get; set; } = CommentModerationStatus.Pending;
        public CommentModerationOrigin ModeratedBy { get; set; } = CommentModerationOrigin.None;
        public string? ToxicFilterTaskId { get; set; }
        public CommentAppealStatus AppealStatus { get; set; } = CommentAppealStatus.None;
        public string? AppealMessage { get; set; }
        public List<CommentReport> Reports { get; set; } = new();

        public bool IsEditable() => ModerationStatus != CommentModerationStatus.Toxic && (DateTime.UtcNow - CreatedAt).TotalMinutes <= 60;
    }
}
