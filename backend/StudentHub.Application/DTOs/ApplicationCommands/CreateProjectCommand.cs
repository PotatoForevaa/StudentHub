namespace StudentHub.Application.DTOs.Commands
{
    public record CreateProjectCommand(
        string Name,
        string Description,
        Guid AuthorId,
        List<Stream>? Files,
        string? Url);
}
