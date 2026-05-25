namespace StudentHub.Application.DTOs.Responses
{
    public record ToxicFilterResultDto(
        bool IsToxic,
        float ToxicProbability,
        string Prediction
    );
}
