namespace StudentHub.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public Uri? ExternalUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid AuthorId { get; set; }
        public List<Image> Images { get; set; } = new List<Image>();
    }
}
