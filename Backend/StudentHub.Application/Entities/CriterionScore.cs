namespace StudentHub.Application.Entities
{
    public class CriterionScore
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public Guid CriterionId { get; set; }
        public Criterion Criterion { get; set; } = null!;
        public Guid TeacherId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}