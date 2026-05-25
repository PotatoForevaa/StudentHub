namespace StudentHub.Application.DTOs.Responses
{
    public record CriterionDto(
        Guid Id,
        Guid CategoryId,
        string CategoryName,
        string Name
    );
}