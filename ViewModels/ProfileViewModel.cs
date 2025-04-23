using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Views;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection; // Needed for DI

namespace Restaurant.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    User currentUser;

    public IAsyncRelayCommand LoadUserProfileCommand { get; }
    public IAsyncRelayCommand LogoutCommand { get; }

    public ProfileViewModel(IAuthService authService, INotificationService notificationService)
    {
        _authService = authService;
        _notificationService = notificationService;
        Title = "My Profile";
        LoadUserProfileCommand = new AsyncRelayCommand(LoadUserProfileAsync);
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);
    }

    async Task LoadUserProfileAsync()
    {
         if (IsBusy) return;
         IsBusy = true;
         try
         {
             CurrentUser = await _authService.GetCurrentUserAsync();
             if (CurrentUser == null)
             {
                  System.Diagnostics.Debug.WriteLine("Profile Load: User is null.");
             }
         }
         catch(Exception ex)
         {
              System.Diagnostics.Debug.WriteLine($"Error loading profile: {ex}");
         }
         finally
         {
             IsBusy = false;
         }
    }

     async Task LogoutAsync()
     {
          if (IsBusy) return;
          IsBusy = true;
          try
          {
              await _authService.LogoutAsync();
              var loginPage = MauiProgram.Services.GetRequiredService<LoginPage>();
              if (Application.Current != null)
              {
                  Application.Current.MainPage = loginPage;
              }
          }
           catch(Exception ex)
          {
               System.Diagnostics.Debug.WriteLine($"Error logging out from Profile: {ex}");
               if (_notificationService != null)
               {
                    await _notificationService.ShowNotificationAsync("Logout Error", "An error occurred during logout.");
               }
          }
          finally
          {
              IsBusy = false;
          }
     }
}