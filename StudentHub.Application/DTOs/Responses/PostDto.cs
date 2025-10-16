namespace StudentHub.Application.DTOs.Responses
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public float RatingAvg { get; set; }

    }
}
