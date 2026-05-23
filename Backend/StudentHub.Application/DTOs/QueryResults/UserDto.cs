namespace StudentHub.Application.DTOs.Responses
{
    public record UserDto(
        Guid Id,
        string Username,
        string FullName,
        List<string>? Roles = null);
}
