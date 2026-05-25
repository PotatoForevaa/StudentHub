namespace StudentHub.Application.DTOs.Responses
{
    public record ToxicFilterTaskResultDto(
        string Status,
        bool IsToxic,
        float ToxicProbability,
        string Prediction,
        string? ErrorMessage = null
    );
}
