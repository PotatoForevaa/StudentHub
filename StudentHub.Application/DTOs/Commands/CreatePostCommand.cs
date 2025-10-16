namespace StudentHub.Application.DTOs.Requests
{
    public class CreatePostCommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AuthorId { get; set; }
    }
}
