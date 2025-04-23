using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Restaurant.ViewModels;

public partial class OrderViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    public ObservableCollection<Order> PastOrders { get; } = new();

    [ObservableProperty]
    private Order _selectedOrder;

    public OrderViewModel(IDataService dataService, IAuthService authService, INotificationService notificationService)
    {
        _dataService = dataService;
        _authService = authService;
        _notificationService = notificationService;
        Title = "My Orders";
    }

    [RelayCommand]
    async Task LoadOrdersAsync()
    {
        SelectedOrder = null;

        if (IsBusy) return;

        IsBusy = true;
        try
        {
            PastOrders.Clear();
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null)
            {
                var orders = await _dataService.GetOrdersForUserAsync(currentUser.Id);
                foreach (var order in orders.OrderByDescending(o => o.OrderDate))
                {
                    PastOrders.Add(order);
                }
            }
            else
            {
                 await _notificationService.ShowNotificationAsync("Info", "Please log in to view your orders.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading orders: {ex}");
            await _notificationService.ShowNotificationAsync("Error", "Failed to load order history.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ViewOrderDetails(Order order)
    {
        if (order == null) return;

        Debug.WriteLine($"Viewing details for Order ID: {order.Id}");
        await _notificationService.ShowNotificationAsync("Order Details", $"Selected Order ID: {order.Id}\nTotal: {order.TotalAmount:C}\nStatus: {order.Status}");
        // Consider navigating to a detail page here:
        // await Shell.Current.GoToAsync($"{nameof(OrderDetailPage)}?OrderId={order.Id}");
    }
}