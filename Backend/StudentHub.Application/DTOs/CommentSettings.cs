namespace StudentHub.Application.DTOs
{
    public class CommentSettings
    {
        public int MinLength { get; set; } = 5;
        public int MaxLength { get; set; } = 5000;
        public int ModerationPollingIntervalSeconds { get; set; } = 5;
    }
}
