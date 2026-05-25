namespace StudentHub.Application.Entities
{
    public class Tag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public List<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
    }
}