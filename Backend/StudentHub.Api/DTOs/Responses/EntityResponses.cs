namespace StudentHub.Api.DTOs.Responses
{
    /// <summary>
    /// User account information response.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// User unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User login username.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User full name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Path to user's profile picture.
        /// </summary>
        public string? ProfilePicturePath { get; set; }
    }

    /// <summary>
    /// Post response with all details.
    /// </summary>
    public class PostResponse
    {
        /// <summary>
        /// Post unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Post title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Post description/content.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Author's user ID.
        /// </summary>
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Post creation date and time (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Average rating of the post.
        /// </summary>
        public float AverageRating { get; set; }
    }

    /// <summary>
    /// Project response with all details.
    /// </summary>
    public class ProjectResponse
    {
        /// <summary>
        /// Project unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Project name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Project description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// External project URL (if applicable).
        /// </summary>
        public string? ExternalUrl { get; set; }

        /// <summary>
        /// Project creation date and time (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Author's user ID.
        /// </summary>
        public Guid AuthorId { get; set; }

        /// <summary>
        /// List of image file names/paths in the project.
        /// </summary>
        public List<string> ImagePaths { get; set; } = new();
    }

    /// <summary>
    /// Image list response for a project.
    /// </summary>
    public class ImageListResponse
    {
        /// <summary>
        /// List of image file paths.
        /// </summary>
        public List<string> ImagePaths { get; set; } = new();

        /// <summary>
        /// Total count of images.
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// Authentication response for login.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// User information.
        /// </summary>
        public UserResponse? User { get; set; }

        /// <summary>
        /// Authentication message.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
