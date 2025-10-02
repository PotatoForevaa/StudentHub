using System.ComponentModel.DataAnnotations;

namespace StudentHub.Web.DTOs.Requests
{
    public class RegistrationRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [Length(8, 32, ErrorMessage = "Пароль должен быть от 8 до 32 символов")]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
    }
}
