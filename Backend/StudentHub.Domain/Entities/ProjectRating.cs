namespace StudentHub.Domain.Entities
{
    public class ProjectRating
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AuthorId { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public int Score { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
