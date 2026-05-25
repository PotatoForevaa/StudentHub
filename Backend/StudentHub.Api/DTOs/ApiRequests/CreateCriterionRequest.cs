using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class CreateCriterionRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        
        [Required]
        public Guid CategoryId { get; set; }
    }
}