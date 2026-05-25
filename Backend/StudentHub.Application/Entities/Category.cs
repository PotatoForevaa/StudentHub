namespace StudentHub.Application.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public List<Criterion> Criteria { get; set; } = new List<Criterion>();
        public List<ProjectCategory> ProjectCategories { get; set; } = new List<ProjectCategory>();
    }
}