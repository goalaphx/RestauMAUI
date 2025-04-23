
using Restaurant.Models;

namespace Restaurant.Services;

public interface IAuthService
{
    Task<User?> GetCurrentUserAsync();
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
    // Task<bool> RegisterAsync(string username, string password, string fullName); // Optional
}