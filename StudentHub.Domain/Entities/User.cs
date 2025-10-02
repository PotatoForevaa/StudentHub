namespace StudentHub.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}