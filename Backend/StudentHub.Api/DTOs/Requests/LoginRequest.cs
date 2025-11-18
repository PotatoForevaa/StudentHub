using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Введите имя пользователя")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Введите пароль")]        
        public string Password { get; set; }
    }
}
