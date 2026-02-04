namespace StudentHub.Api.DTOs.Responses
{

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
    }


    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public float AverageRating { get; set; }
    }

    public class ProjectResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ExternalUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid AuthorId { get; set; }
        public List<string> ImagePaths { get; set; } = new();
    }

    public class ImageListResponse
    {
        public List<string> ImagePaths { get; set; } = new();
        public int Count { get; set; }
    }

    public class AuthResponse
    {
        public UserResponse? User { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
