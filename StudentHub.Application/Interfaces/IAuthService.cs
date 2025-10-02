using StudentHub.Domain.Entities;

namespace StudentHub.Application.Interfaces
{
    public interface IAuthService
    {
        Task SignInAsync(User user);
        Task SignOutAsync();
    }
}
