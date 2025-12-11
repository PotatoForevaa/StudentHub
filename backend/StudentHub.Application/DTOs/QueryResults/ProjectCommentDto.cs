namespace StudentHub.Application.DTOs.Responses
{
    public record ProjectCommentDto(
        Guid Id,
        Guid AuthorId,
        string AuthorUsername,
        string AuthorFullName,
        string? AuthorProfilePicturePath,
        string Content,
        DateTime CreatedAt,
        int? UserScore
    );
}

