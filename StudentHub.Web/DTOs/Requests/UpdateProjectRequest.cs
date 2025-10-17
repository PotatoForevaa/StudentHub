using System.ComponentModel.DataAnnotations;

namespace StudentHub.Web.DTOs.Requests
{
    public class UpdateProjectRequest
    {
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string? ExternalUrl { get; set; }
        public List<string>? Base64Images { get; set; } = new List<string>();
    }
}
