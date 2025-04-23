using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System; // For Exception

namespace Restaurant.ViewModels;

public partial class MyReservationsViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    public ObservableCollection<Reservation> Reservations { get; } = new();

    public IAsyncRelayCommand LoadReservationsCommand { get; }

    public MyReservationsViewModel(IDataService dataService, IAuthService authService, INotificationService notificationService)
    {
        _dataService = dataService;
        _authService = authService;
        _notificationService = notificationService;
        Title = "My Reservations";
        LoadReservationsCommand = new AsyncRelayCommand(LoadReservationsAsync);
    }

    async Task LoadReservationsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Reservations.Clear();
            var user = await _authService.GetCurrentUserAsync();
            if (user != null)
            {
                var reservations = await _dataService.GetReservationsForUserAsync(user.Id);
                foreach(var res in reservations)
                {
                    Reservations.Add(res);
                }
            }
            else
            {
                 if (_notificationService != null)
                    await _notificationService.ShowNotificationAsync("Error", "Could not identify user.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading reservations: {ex}");
             if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Error", "Failed to load reservations.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}