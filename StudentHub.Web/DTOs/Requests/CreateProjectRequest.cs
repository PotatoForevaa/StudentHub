using System.ComponentModel.DataAnnotations;

namespace StudentHub.Web.DTOs.Requests
{
    public class CreateProjectRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string? ExternalUrl { get; set; }
        public List<string>? Base64Images { get; set; }
    }
}
