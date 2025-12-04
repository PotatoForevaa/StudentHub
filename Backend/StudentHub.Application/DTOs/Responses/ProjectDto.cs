
namespace StudentHub.Application.DTOs.Responses
{
    public record ProjectDto(
        Guid Id,
        string Name,
        string Description,
        List<string> Files,
        string Author,
        DateTime CreationDate
        );
}
