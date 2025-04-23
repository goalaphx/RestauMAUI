using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Restaurant.ViewModels;

public partial class AllOrdersViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    public ObservableCollection<Order> Orders { get; } = new();

    public IAsyncRelayCommand LoadOrdersCommand { get; }
    public IAsyncRelayCommand<Order> ChangeOrderStatusCommand { get; }

    public AllOrdersViewModel(IDataService dataService, IAuthService authService, INotificationService notificationService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        Title = "All Orders";
        LoadOrdersCommand = new AsyncRelayCommand(LoadOrdersAsync);
        ChangeOrderStatusCommand = new AsyncRelayCommand<Order>(ChangeOrderStatusAsync);
    }

    async Task LoadOrdersAsync()
    {
        if (IsBusy) return;

        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser?.Role != UserRole.Staff)
        {
             if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Access Denied", "You don't have permission to view this page.");
             if (Shell.Current != null)
                await Shell.Current.GoToAsync($"//{nameof(MenuPage)}");
            return;
        }

        IsBusy = true;
        try
        {
            Orders.Clear();
            var orders = await _dataService.GetAllOrdersAsync();
            foreach(var order in orders)
            {
                Orders.Add(order);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading all orders: {ex}");
             if (_notificationService != null)
                 await _notificationService.ShowNotificationAsync("Error", "Failed to load orders.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ChangeOrderStatusAsync(Order? orderToUpdate)
    {
        if (orderToUpdate == null || IsBusy) return;

        var statusOptions = Enum.GetNames(typeof(OrderStatus));

        string? selectedStatusStr = await Shell.Current.DisplayActionSheet(
            $"Update Status for Order #{orderToUpdate.Id}",
            "Cancel",
            null,
            statusOptions);

        if (string.IsNullOrWhiteSpace(selectedStatusStr) || selectedStatusStr == "Cancel")
        {
            return;
        }

        if (Enum.TryParse<OrderStatus>(selectedStatusStr, out var newStatus))
        {
            IsBusy = true;
            try
            {
                bool success = await _dataService.UpdateOrderStatusAsync(orderToUpdate.Id, newStatus);
                if (success)
                {
                    orderToUpdate.Status = newStatus; // This should trigger UI update via INPC
                    if (_notificationService != null)
                        await _notificationService.ShowNotificationAsync("Success", $"Order #{orderToUpdate.Id} status updated to {newStatus}.");
                }
                else
                {
                     if (_notificationService != null)
                        await _notificationService.ShowNotificationAsync("Error", "Failed to update order status.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating order status: {ex}");
                 if (_notificationService != null)
                    await _notificationService.ShowNotificationAsync("Error", "An unexpected error occurred.");
            }
            finally
            {
                IsBusy = false;
            }
        }
        else
        {
            if (_notificationService != null)
                 await _notificationService.ShowNotificationAsync("Error", "Invalid status selected.");
        }
    }
}