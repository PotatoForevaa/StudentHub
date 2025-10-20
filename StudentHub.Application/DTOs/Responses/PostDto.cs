namespace StudentHub.Application.DTOs.Responses
{
    public record PostDto(
        Guid Id,
        string Title,
        string Description,
        Guid AuthorId,
        DateTime CreatedAt,
        float AverageRating);
}
