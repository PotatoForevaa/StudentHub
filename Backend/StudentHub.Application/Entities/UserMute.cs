namespace StudentHub.Application.Entities
{
    public class UserMute
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid MutedByUserId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime MutedUntil { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive => MutedUntil > DateTime.UtcNow;

        public User User { get; set; } = null!;
        public User MutedByUser { get; set; } = null!;
    }
}