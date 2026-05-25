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
        int? UserScore,
        Guid? ProjectId = null,
        string? ProjectName = null,
        string? ModerationStatus = null,
        string? ModeratedBy = null,
        int ReportCount = 0,
        string? AppealStatus = null,
        string? AppealMessage = null
    );
}

