namespace StudentHub.Application.DTOs.Responses
{
    public record ActivityDto(
        string Type,
        Guid Id,
        string? Title,
        string Content,
        DateTime CreatedAt,
        string? ProjectName,
        Guid? ProjectId
    );
}
