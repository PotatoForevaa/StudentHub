namespace StudentHub.Application.DTOs.Responses
{
    public record MuteInfoDto(
        bool IsMuted,
        DateTime? MutedUntil,
        string? Reason,
        string? MutedByUsername,
        DateTime? CreatedAt
    );
}