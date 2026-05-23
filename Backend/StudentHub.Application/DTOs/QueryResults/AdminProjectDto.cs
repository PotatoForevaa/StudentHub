namespace StudentHub.Application.DTOs.Responses
{
    public record AdminProjectDto(
        Guid Id,
        string Name,
        string Description,
        string AuthorUsername,
        string AuthorName,
        DateTime CreatedAt);
}
