// Services/MockAuthService.cs (Modified for DB lookup - STILL INSECURE PASSWORD HANDLING)
using Restaurant.Models;
using System.Threading.Tasks; // For Task

namespace Restaurant.Services;

public class MockAuthService : IAuthService
{
    private readonly IDataService _dataService; // Now points to PostgresDataService
    private User? _currentUser;

    // Remove the mock credentials dictionary

    public MockAuthService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public Task<User?> GetCurrentUserAsync()
    {
        return Task.FromResult(_currentUser);
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _dataService.GetUserByUsernameAsync(username);

        // !!! INSECURE PASSWORD CHECK - FOR DEMO ONLY !!!
        // In a real app, retrieve user.PasswordHash and use a password hashing library
        // (e.g., BCrypt.Net) to verify the entered password against the stored hash.
        if (user != null && user.PasswordHash == password) // Comparing plain text! Bad!
        {
            _currentUser = user;
            return true;
        }
        // !!! END INSECURE CHECK !!!

        _currentUser = null;
        return false;
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        return Task.CompletedTask;
    }
}