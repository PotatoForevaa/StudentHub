namespace StudentHub.Application.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ProfilePicturePath { get; set; } = string.Empty;
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
