using DemoREST.Domain;

namespace DemoREST.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string Email, string Password);
        Task<AuthenticationResult> LoginAsync(string Email, string Password);
        Task<AuthenticationResult> RefreshTokenAsync(string Token, string RefreshToken);
    }
}
