namespace StudentHub.Application.Entities
{
    public class Image
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Path { get; set; }
        public Project Project { get; set; }
    }
}
