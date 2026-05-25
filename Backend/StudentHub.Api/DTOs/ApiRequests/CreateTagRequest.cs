using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class CreateTagRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}