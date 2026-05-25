namespace StudentHub.Application.Entities
{
    public class ProjectCategory
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}