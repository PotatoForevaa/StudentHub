using StudentHub.Api.Extensions.Attributes;
using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class CreateProjectRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string? ExternalUrl { get; set; }
        [StudentHub.Api.Extensions.Attributes.FileExtensions("jpg,jpeg,png,gif,webp")]
        [FileSize(10485760)] // 10 MB
        public List<IFormFile>? Files { get; set; }
    }
}
