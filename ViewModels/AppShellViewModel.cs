// ViewModels/AppShellViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Services;
using Restaurant.Views;
using Restaurant.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Maui.ApplicationModel; // Needed for MainThread

namespace Restaurant.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    bool isStaff;

    [ObservableProperty]
    bool isClient;

    public AppShellViewModel(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    // Modified InitializeRoles
    public void InitializeRoles(UserRole role)
    {
        // Set properties immediately
        IsStaff = (role == UserRole.Staff);
        IsClient = (role == UserRole.Client);
        Debug.WriteLine($"AppShellViewModel Roles Initialized: IsStaff={IsStaff}, IsClient={IsClient}");

        // --- ADD DELAYED NOTIFICATION ---
        // Schedule a follow-up notification on the main thread after a short delay
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(50); // Small delay (50-100ms often enough)
            // Re-notify properties - this forces bindings to re-evaluate
            OnPropertyChanged(nameof(IsStaff));
            OnPropertyChanged(nameof(IsClient));
            Debug.WriteLine($"Delayed notification sent for IsStaff/IsClient");
        });
        // --- END DELAYED NOTIFICATION ---
    }


    [RelayCommand]
    async Task GoToProfileAsync()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage));
        }
    }

    [RelayCommand]
    async Task LogoutAsync()
    {
       try
        {
            await _authService.LogoutAsync();
            var loginPage = MauiProgram.Services.GetRequiredService<LoginPage>();
            if (Application.Current != null)
            {
                Application.Current.MainPage = loginPage;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during logout: {ex}");
        }
    }
}