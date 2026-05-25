namespace StudentHub.Application.DTOs.Responses
{
    public record CriterionScoreDto(
        Guid CriterionId,
        string CriterionName,
        int Score,
        string? Comment,
        string TeacherName,
        DateTime CreatedAt
    );
}