
namespace StudentHub.Application.DTOs.Responses
{
    public record ProjectDto(
        Guid Id,
        string Name,
        string Description,
        List<string> Files,
        string AuthorName,
        string AuthorUsername,
        DateTime CreationDate,
        double? AverageRating = null,
        List<ProjectCommentDto>? Comments = null
        );
}
