using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class SubmitCriterionScoreRequest
    {
        [Required]
        public Guid CriterionId { get; set; }
        
        [Required]
        [Range(1, 10)]
        public int Score { get; set; }
        
        public string? Comment { get; set; }
    }

    public class SubmitCriterionScoresRequest
    {
        [Required]
        public List<SubmitCriterionScoreRequest> Scores { get; set; } = new();
    }
}