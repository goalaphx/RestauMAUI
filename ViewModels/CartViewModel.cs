// IMPORTANT: Register this as a Singleton in MauiProgram.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MenuModel = Restaurant.Models.MenuItem;

namespace Restaurant.ViewModels;

public partial class CartViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private readonly IPaymentService _paymentService;
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;

    public ObservableCollection<CartItem> CartItems { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CartTotal))]
    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
    // Re-evaluate if order can be placed
    int itemCount;

// Calculated property for total
    public decimal CartTotal => CartItems.Sum(item => item.TotalPrice);

    public CartViewModel(INotificationService notificationService, IPaymentService paymentService,
        IDataService dataService, IAuthService authService)
    {
        _notificationService = notificationService;
        _paymentService = paymentService;
        _dataService = dataService;
        _authService = authService;
        Title = "My Cart";
        CartItems.CollectionChanged += (s, e) => UpdateItemCount(); // Update count when collection changes
    }

    private void UpdateItemCount()
    {
        ItemCount = CartItems.Sum(item => item.Quantity);
        // Also trigger PropertyChanged for CartTotal as it depends on the collection content
        OnPropertyChanged(nameof(CartTotal));
    }

    [RelayCommand]
    public void AddItem(MenuModel menuItem)
    {
        if (menuItem == null) return;

        var existingItem = CartItems.FirstOrDefault(ci => ci.MenuItemId == menuItem.Id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
            // Manually trigger PropertyChanged for the specific item if CartItem doesn't implement INotifyPropertyChanged itself
            // If CartItem *does* implement INPC for Quantity/TotalPrice, this isn't needed.
            OnPropertyChanged(nameof(CartItems)); // Refresh list binding if needed
        }
        else
        {
            CartItems.Add(new CartItem
            {
                MenuItemId = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                Quantity = 1
            });
        }

        UpdateItemCount(); // Explicitly update count and total
        Debug.WriteLine(
            $"Item Added/Updated: {menuItem.Name}. Current Count: {ItemCount}. Current Total: {CartTotal:C}");
    }

    [RelayCommand]
    public void RemoveItem(CartItem cartItem)
    {
        if (cartItem == null) return;

        var existingItem = CartItems.FirstOrDefault(ci => ci.MenuItemId == cartItem.MenuItemId);
        if (existingItem != null)
        {
            if (existingItem.Quantity > 1)
            {
                existingItem.Quantity--;
                OnPropertyChanged(nameof(CartItems)); // Refresh list binding if needed
            }
            else
            {
                CartItems.Remove(existingItem);
            }
        }

        UpdateItemCount(); // Explicitly update count and total
        Debug.WriteLine(
            $"Item Removed/Decremented: {cartItem.Name}. Current Count: {ItemCount}. Current Total: {CartTotal:C}");
    }

    [RelayCommand]
    public void ClearCart()
    {
        if (CartItems.Any())
        {
            CartItems.Clear();
            UpdateItemCount(); // Explicitly update count and total
            Debug.WriteLine($"Cart Cleared. Current Count: {ItemCount}. Current Total: {CartTotal:C}");
        }
    }


// Only allow placing order if there are items in the cart and not busy
    private bool CanPlaceOrder() => CartItems.Any() && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
    async Task PlaceOrderAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                await _notificationService.ShowNotificationAsync("Error", "You must be logged in to place an order.");
                // Optional: Navigate to Login Page
                // await Shell.Current.GoToAsync(nameof(LoginPage));
                return;
            }

            // 1. Simulate Payment
            var (paymentSuccess, transactionId) = await _paymentService.ProcessPaymentAsync(CartTotal);

            if (!paymentSuccess)
            {
                await _notificationService.ShowNotificationAsync("Payment Failed",
                    "There was an issue processing your payment. Please try again.");
                return; // Stop order placement
            }

            // 2. Create Order object
            var order = new Order
            {
                UserId = currentUser.Id, // Link to logged-in user
                OrderDate = DateTime.Now,
                TotalAmount = CartTotal,
                Status = OrderStatus.Paid, // Set status after successful payment
                PaymentId = transactionId,
                Items = CartItems.Select(ci => new OrderItem // Convert CartItems to OrderItems
                {
                    MenuItemId = ci.MenuItemId,
                    Name = ci.Name,
                    Quantity = ci.Quantity,
                    Price = ci.Price // Price at the time of order
                }).ToList()
            };

            // 3. Save Order via DataService
            bool orderPlaced = await _dataService.PlaceOrderAsync(order);

            if (orderPlaced)
            {
                await _notificationService.ShowNotificationAsync("Order Placed",
                    $"Your order #{order.Id} has been placed successfully!");
                ClearCart(); // Clear the cart after successful order

                // Navigate to Order History or back to Menu
                await Shell.Current.GoToAsync($"//{nameof(MenuPage)}"); // Go back to Menu
                // OR: await Shell.Current.GoToAsync($"//{nameof(OrderPage)}"); // Go to order history
            }
            else
            {
                await _notificationService.ShowNotificationAsync("Error",
                    "Failed to save your order after payment. Please contact support.");
                // Potentially need logic here to handle a failed save after successful payment (e.g., refund simulation)
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error placing order: {ex}");
            await _notificationService.ShowNotificationAsync("Error",
                "An unexpected error occurred while placing your order.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}