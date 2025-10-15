namespace StudentHub.Application.Interfaces
{
    public interface IAuthService
    {
        Task SignInAsync(Guid id, string userame);
        Task SignOutAsync();
    }
}
