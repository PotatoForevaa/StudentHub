using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class MuteUserRequest
    {
        [Required]
        public string Duration { get; set; } = "1h"; // 1h, 6h, 24h, 3d, 7d, 30d
        public string? Reason { get; set; }
    }
}