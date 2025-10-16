namespace StudentHub.Application.DTOs.Requests
{
    public class RegisterUserCommand
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}
