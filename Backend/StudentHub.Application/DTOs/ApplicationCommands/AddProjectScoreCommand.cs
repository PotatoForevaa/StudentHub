namespace StudentHub.Application.DTOs.Commands
{
    public record AddProjectScoreCommand(
        Guid ProjectId,
        Guid AuthorId,
        int Score
    );
}
