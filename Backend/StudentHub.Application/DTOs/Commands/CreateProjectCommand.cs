namespace StudentHub.Application.DTOs.Commands
{
    public record CreateProjectCommand(
        string Name,
        string Description,
        Guid AuthorId,
        List<string> FilePaths,
        string? Url);
}
