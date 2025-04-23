using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models; // Needed for UserRole
using Restaurant.Services;
using Restaurant.Views; // For navigation
using System.Diagnostics;
using System.Threading.Tasks;
using System; // For Exception
using System.Linq; // For Sum()

namespace Restaurant.ViewModels;

public partial class StatsViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    int totalClients; // Changed from totalOrders

    [ObservableProperty]
    decimal totalRevenue;

    [ObservableProperty]
    int pendingReservations;

    public IAsyncRelayCommand LoadStatsCommand { get; }

    public StatsViewModel(IDataService dataService, IAuthService authService, INotificationService notificationService)
    {
        _dataService = dataService;
        _authService = authService;
        _notificationService = notificationService;
        Title = "Restaurant Stats";
        LoadStatsCommand = new AsyncRelayCommand(LoadStatsAsync);
    }

    async Task LoadStatsAsync()
    {
         var user = await _authService.GetCurrentUserAsync();
         if (user?.Role != Models.UserRole.Staff)
         {
             if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Access Denied", "You do not have permission to view statistics.");
             if (Shell.Current != null)
                await Shell.Current.GoToAsync($"//{nameof(MenuPage)}");
             return;
         }

        if (IsBusy) return;
        IsBusy = true;
        try
        {
             // Get Total Clients
             TotalClients = await _dataService.GetUserCountByRoleAsync(UserRole.Client);

             // Get Total Revenue from Paid/Delivered Orders
             var allOrders = await _dataService.GetAllOrdersAsync();
             TotalRevenue = allOrders
                 .Where(o => o.Status == OrderStatus.Paid || o.Status == OrderStatus.Delivered)
                 .Sum(o => o.TotalAmount);

             // Get Pending Reservations
             var allReservations = await _dataService.GetAllReservationsAsync();
             PendingReservations = allReservations.Count(r => r.Status == ReservationStatus.Requested);

             Debug.WriteLine("Stats loaded.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading stats: {ex}");
            if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Error", "Failed to load statistics.");
            // Clear stats on error?
            TotalClients = 0;
            TotalRevenue = 0;
            PendingReservations = 0;
        }
        finally
        {
            IsBusy = false;
        }
    }
}