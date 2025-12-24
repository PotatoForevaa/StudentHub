namespace StudentHub.Application.DTOs.Responses
{
    public record LeaderboardUserDto(
        Guid Id,
        string FullName,
        double Score);
}
