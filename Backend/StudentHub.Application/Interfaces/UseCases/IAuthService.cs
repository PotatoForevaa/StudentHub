namespace StudentHub.Application.Interfaces.UseCases
{
    public interface IAuthService
    {
        Task SignInAsync(Guid id, string username);
        Task SignOutAsync();
        Task SignInWithOAuth2Async(string externalId, string username, string fullName);
        Task ChallengeOAuth2Async(string redirectUri);
        Task HandleOAuth2CallbackAsync(string code, string state, string redirectUri);
    }
}
