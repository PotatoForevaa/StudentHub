namespace StudentHub.Application.DTOs.Responses
{
    public record UserDto(
        Guid Id,
        string Username,
        string FullName);
}
