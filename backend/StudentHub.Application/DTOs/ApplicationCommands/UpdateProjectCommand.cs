namespace StudentHub.Application.DTOs.Commands
{
    public record UpdateProjectCommand(
        Guid ProjectId,
        Guid AuthorId,
        string Name,
        string Description,
        string? ExternalUrl,
        List<string>? Base64Images
    );
}

