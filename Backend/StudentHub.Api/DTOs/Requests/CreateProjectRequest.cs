using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public record CreateProjectRequest(
        [Required] string Name,
        [Required] string Description,
        string? ExternalUrl,
        List<IFormFile>? Files
    );
}
