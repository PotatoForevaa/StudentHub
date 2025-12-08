namespace StudentHub.Application.DTOs.Commands
{
    public record CreateProjectCommentCommand(
        Guid ProjectId,
        Guid AuthorId,
        string Content
    );
}

