using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StudentHub.Application.Interfaces.UseCases;
using System.Security.Claims;

namespace StudentHub.Api.WebServices
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public AuthService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task SignInAsync(Guid id, string username)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, username)
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await _httpContextAccessor.HttpContext!.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(
                IdentityConstants.ApplicationScheme);
        }

        public async Task SignInWithOAuth2Async(string externalId, string username, string fullName)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, externalId),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.GivenName, fullName),
                    new Claim("external_id", externalId)
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await _httpContextAccessor.HttpContext!.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task ChallengeOAuth2Async(string redirectUri)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUri ?? _configuration["OAuth2:RedirectUri"]
            };

            await _httpContextAccessor.HttpContext!.ChallengeAsync(
                OpenIdConnectDefaults.AuthenticationScheme,
                properties);
        }

        public async Task HandleOAuth2CallbackAsync(string code, string state, string redirectUri)
        {
        }
    }
}
