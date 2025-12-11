namespace StudentHub.Application.Interfaces.UseCases
{
    public interface IAuthService
    {
        Task SignInAsync(Guid id, string userame);
        Task SignOutAsync();
    }
}
