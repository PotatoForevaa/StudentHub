namespace StudentHub.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task SignInAsync(Guid id, string userame);
        Task SignOutAsync();
    }
}
