namespace StudentHub.Application.DTOs.Requests
{
    public record RegisterUserCommand(
        string Username,
        string Password,
        string FullName);
}
