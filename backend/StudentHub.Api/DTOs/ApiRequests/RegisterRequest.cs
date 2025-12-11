using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Введите имя пользователя")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Length(8, 32, ErrorMessage = "Пароль должен быть от 8 до 32 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите ФИО")]
        public string FullName { get; set; }
    }
}
