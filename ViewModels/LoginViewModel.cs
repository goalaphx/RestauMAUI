// ViewModels/LoginViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Services;
using Restaurant.Views;
using Restaurant.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Microsoft.Maui.ApplicationModel;

namespace Restaurant.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    [ObservableProperty] string username;
    [ObservableProperty] string password;
    [ObservableProperty] string loginError;

    public IAsyncRelayCommand LoginAsyncCommand { get; }

    public LoginViewModel(IAuthService authService, INotificationService notificationService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        Title = "Login";
        LoginAsyncCommand = new AsyncRelayCommand(LoginAsync);
    }

    async Task LoginAsync()
    {
        Debug.WriteLine("--- LoginAsync method STARTED ---");
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            LoginError = "Username and Password cannot be empty.";
            return;
        }

        IsBusy = true;
        LoginError = string.Empty;
        Models.User? loggedInUser = null;

        try
        {
            bool success = await _authService.LoginAsync(Username, Password);

            if (success)
            {
                Debug.WriteLine("Login Successful!");
                loggedInUser = await _authService.GetCurrentUserAsync();

                if (loggedInUser == null)
                {
                    throw new Exception("Login succeeded but failed to retrieve user details.");
                }

                // --- REVISED SHELL INITIALIZATION ---
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // 1. Get the AppShell instance. DI automatically creates
                    //    its required AppShellViewModel dependency (as Transient).
                    var appShell = MauiProgram.Services.GetRequiredService<AppShell>();

                    // 2. Access the ViewModel that was *injected* into this specific AppShell instance.
                    if (appShell.BindingContext is AppShellViewModel shellViewModel)
                    {
                        // 3. Initialize its roles SYNCHRONOUSLY.
                        shellViewModel.InitializeRoles(loggedInUser.Role);
                        Debug.WriteLine($"DEBUG: Initialized roles on AppShell's injected VM. IsClient={shellViewModel.IsClient}, IsStaff={shellViewModel.IsStaff}");
                    }
                    else
                    {
                        // This shouldn't happen if DI and AppShell constructor are correct
                         Debug.WriteLine("ERROR: AppShell BindingContext is not the expected AppShellViewModel!");
                         // Handle error appropriately - maybe show message and don't set MainPage
                         return; // Exit lambda
                    }

                    // 4. Set the MainPage - Shell now has its BindingContext configured correctly.
                    Application.Current.MainPage = appShell;
                    Debug.WriteLine($"Set Application.Current.MainPage to new AppShell instance for role: {loggedInUser.Role}");
                });
                // --- END REVISED SHELL INITIALIZATION ---

            }
            else
            {
                LoginError = "Invalid username or password.";
                Password = string.Empty;
            }
        }
        catch (Exception ex)
        {
            LoginError = "An error occurred during login.";
            // Logging... (keep enhanced logging)
            Debug.WriteLine("");
            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.WriteLine("!!!         LOGIN FAILED         !!!");
            // ... rest of detailed logging ...
            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.WriteLine("");
        }
        finally
        {
            IsBusy = false;
        }
    }
}