namespace StudentHub.Application.DTOs.Responses
{
    public record LeaderboardUserDto(
        Guid Id,
        string FullName,
        string ProfilePicturePath,
        double Score);
}
