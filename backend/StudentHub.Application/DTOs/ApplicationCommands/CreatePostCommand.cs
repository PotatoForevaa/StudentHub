namespace StudentHub.Application.DTOs.Requests
{
    public record CreatePostCommand(
        string Title,
        string Description,
        Guid AuthorId);
}
