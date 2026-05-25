namespace StudentHub.Application.Entities
{
    public class Criterion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public List<CriterionScore> Scores { get; set; } = new List<CriterionScore>();
    }
}