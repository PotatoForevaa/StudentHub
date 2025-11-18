using Microsoft.AspNetCore.Identity;

namespace StudentHub.Api.Extensions
{
    public class CustomIdentityErrorDescriberRu : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
            => new() { Code = nameof(DefaultError), Description = "Произошла неизвестная ошибка." };

        public override IdentityError ConcurrencyFailure()
            => new() { Code = nameof(ConcurrencyFailure), Description = "Ошибка конкурентного доступа: объект был изменён." };

        public override IdentityError PasswordMismatch()
            => new() { Code = nameof(PasswordMismatch), Description = "Неверный пароль." };

        public override IdentityError InvalidToken()
            => new() { Code = nameof(InvalidToken), Description = "Недействительный токен." };

        public override IdentityError LoginAlreadyAssociated()
            => new() { Code = nameof(LoginAlreadyAssociated), Description = "Пользователь с этим логином уже существует." };

        public override IdentityError InvalidUserName(string userName)
            => new() { Code = nameof(InvalidUserName), Description = $"Имя пользователя '{userName}' недопустимо: можно использовать только буквы и цифры." };

        public override IdentityError InvalidEmail(string email)
            => new() { Code = nameof(InvalidEmail), Description = $"Email '{email}' недействителен." };

        public override IdentityError DuplicateUserName(string userName)
            => new() { Code = nameof(DuplicateUserName), Description = $"Имя пользователя '{userName}' уже занято." };

        public override IdentityError DuplicateEmail(string email)
            => new() { Code = nameof(DuplicateEmail), Description = $"Email '{email}' уже используется." };

        public override IdentityError InvalidRoleName(string role)
            => new() { Code = nameof(InvalidRoleName), Description = $"Название роли '{role}' недопустимо." };

        public override IdentityError DuplicateRoleName(string role)
            => new() { Code = nameof(DuplicateRoleName), Description = $"Название роли '{role}' уже используется." };

        public override IdentityError UserAlreadyHasPassword()
            => new() { Code = nameof(UserAlreadyHasPassword), Description = "У пользователя уже установлен пароль." };

        public override IdentityError UserLockoutNotEnabled()
            => new() { Code = nameof(UserLockoutNotEnabled), Description = "Блокировка для этого пользователя не включена." };

        public override IdentityError UserAlreadyInRole(string role)
            => new() { Code = nameof(UserAlreadyInRole), Description = $"Пользователь уже состоит в роли '{role}'." };

        public override IdentityError UserNotInRole(string role)
            => new() { Code = nameof(UserNotInRole), Description = $"Пользователь не состоит в роли '{role}'." };

        public override IdentityError PasswordTooShort(int length)
            => new() { Code = nameof(PasswordTooShort), Description = $"Пароль должен быть не короче {length} символов." };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Пароль должен содержать хотя бы один неалфанумерический символ." };

        public override IdentityError PasswordRequiresDigit()
            => new() { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать хотя бы одну цифру ('0'-'9')." };

        public override IdentityError PasswordRequiresLower()
            => new() { Code = nameof(PasswordRequiresLower), Description = "Пароль должен содержать хотя бы одну строчную букву ('a'-'z')." };

        public override IdentityError PasswordRequiresUpper()
            => new() { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать хотя бы одну заглавную букву ('A'-'Z')." };
    }

}
