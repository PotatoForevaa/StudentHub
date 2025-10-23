using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class CreatePostRequest
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
