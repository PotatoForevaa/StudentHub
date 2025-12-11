namespace StudentHub.Application.DTOs.Commands
{
    public record UpdatePostCommand(Guid Id,
        string Title,
        string Description,
        Guid AuthorId);
}
