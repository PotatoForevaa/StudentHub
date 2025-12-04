namespace StudentHub.Application.DTOs
{
    public class Error
    {
        public string Message { get; set; } = default!;
        public string? Field { get; set; }
    }
}
