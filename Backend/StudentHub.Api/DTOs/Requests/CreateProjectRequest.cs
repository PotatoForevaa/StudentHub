using StudentHub.Api.Extensions.Attributes;
using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class CreateProjectRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Название должно быть короче 50 символов")]
        public string Name { get; set; }
        [Required]
        [MaxLength(3000, ErrorMessage = "Описание должно быть короче 3000 символов")]
        public string Description { get; set; }
        public string? ExternalUrl { get; set; }
        [Extensions.Attributes.FileExtensions("jpg,jpeg,png,gif,webp")]
        [FileSize(8 * 1024 * 1024 * 10)] // 10 MB
        public List<IFormFile>? Files { get; set; }
    }
}
