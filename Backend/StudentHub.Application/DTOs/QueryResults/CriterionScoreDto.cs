namespace StudentHub.Application.DTOs.Responses
{
    public record CriterionScoreDto(
        Guid CriterionId,
        string CriterionName,
        string CategoryName,
        int Score,
        string? Comment,
        string TeacherName,
        DateTime CreatedAt
    );
}
