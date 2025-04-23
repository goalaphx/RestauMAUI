using Restaurant.Models;
using System.Linq;
using System.Threading.Tasks; // Added for Task
using System.Collections.Generic; // Added for List
using MenuModel = Restaurant.Models.MenuItem;

namespace Restaurant.Services;

public class InMemoryDataService : IDataService
{
    private List<MenuModel> _menuItems;
    private List<Order> _orders;
    private List<Reservation> _reservations;
    private List<User> _users;

    private int _nextMenuItemId = 1;
    private int _nextOrderId = 1;
    private int _nextReservationId = 1;
    private int _nextUserId = 1;

    public InMemoryDataService()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        _nextUserId = 1;
        _users = new List<User>
        {
            new User { Id = _nextUserId++, Username = "client", PasswordHash = "password", FullName = "Test Client", Role = UserRole.Client },
            new User { Id = _nextUserId++, Username = "staff", PasswordHash = "password", FullName = "Staff Member", Role = UserRole.Staff }
        };

        _nextMenuItemId = 1;
        _menuItems = new List<MenuModel>
        {
            new MenuModel { Id = _nextMenuItemId++, Name = "Margherita Pizza", Description = "Classic cheese and tomato", Price = 12.50m, Category = MenuCategory.MainCourse, ImageUrl = "pizza.png" },
            new MenuModel { Id = _nextMenuItemId++, Name = "Caesar Salad", Description = "Crisp romaine, parmesan, croutons", Price = 8.00m, Category = MenuCategory.Appetizer, ImageUrl = "salad.png" },
            new MenuModel { Id = _nextMenuItemId++, Name = "Spaghetti Carbonara", Description = "Pasta with egg, cheese, pancetta", Price = 14.00m, Category = MenuCategory.MainCourse },
            new MenuModel { Id = _nextMenuItemId++, Name = "Tiramisu", Description = "Coffee-flavored Italian dessert", Price = 6.50m, Category = MenuCategory.Dessert },
            new MenuModel { Id = _nextMenuItemId++, Name = "Cola", Description = "Refreshing cola drink", Price = 2.50m, Category = MenuCategory.Beverage },
        };

        _nextOrderId = 1;
        _orders = new List<Order>();

        _nextReservationId = 1;
        _reservations = new List<Reservation>();
    }

    // Menu Methods
    public Task<List<MenuModel>> GetMenuItemsAsync() => Task.FromResult(_menuItems.ToList());
    public Task<MenuModel?> GetMenuItemAsync(int id) => Task.FromResult(_menuItems.FirstOrDefault(m => m.Id == id));

    public Task<bool> AddMenuItemAsync(MenuModel item)
    {
        if (item == null) return Task.FromResult(false);
        // Simple check if ID is already set (should be 0 for new) or name exists
        if (item.Id != 0 || _menuItems.Any(m => m.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
        {
             // Maybe log a warning or throw? For now, just fail.
             // In real scenario, check constraints properly.
            return Task.FromResult(false);
        }
        item.Id = _nextMenuItemId++;
        _menuItems.Add(item);
        return Task.FromResult(true);
    }

     public Task<bool> UpdateMenuItemAsync(MenuModel item)
     {
         if (item == null) return Task.FromResult(false);
         var index = _menuItems.FindIndex(m => m.Id == item.Id);
         if (index == -1) return Task.FromResult(false);
         _menuItems[index] = item;
         return Task.FromResult(true);
     }

     public Task<bool> DeleteMenuItemAsync(int itemId)
     {
         var itemToRemove = _menuItems.FirstOrDefault(m => m.Id == itemId);
         if (itemToRemove == null) return Task.FromResult(false);
         _menuItems.Remove(itemToRemove);
         return Task.FromResult(true);
     }

    // Order Methods
    public Task<List<Order>> GetOrdersForUserAsync(int userId) =>
        Task.FromResult(_orders.Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToList());

    public Task<Order?> GetOrderAsync(int orderId) => Task.FromResult(_orders.FirstOrDefault(o => o.Id == orderId));

    public Task<bool> PlaceOrderAsync(Order order)
    {
        if (order == null || !order.Items.Any()) return Task.FromResult(false);
        order.Id = _nextOrderId++;
        order.OrderDate = DateTime.Now; // Use Now for simplicity, UTC preferred
        _orders.Add(order);
        System.Diagnostics.Debug.WriteLine($"Order {order.Id} placed for user {order.UserId}. Total: {order.TotalAmount:C}");
        return Task.FromResult(true);
    }

     public Task<List<Order>> GetAllOrdersAsync() =>
        Task.FromResult(_orders.OrderByDescending(o => o.OrderDate).ToList());

     public Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
     {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order != null)
        {
            order.Status = status;
            System.Diagnostics.Debug.WriteLine($"Order {order.Id} status updated to {status}.");
            return Task.FromResult(true);
        }
        System.Diagnostics.Debug.WriteLine($"Order {orderId} not found for status update.");
        return Task.FromResult(false);
     }

    // Reservation Methods
    public Task<List<Reservation>> GetReservationsForUserAsync(int userId) =>
        Task.FromResult(_reservations.Where(r => r.UserId == userId).OrderByDescending(r => r.ReservationTime).ToList());

    public Task<List<Reservation>> GetAllReservationsAsync() =>
        Task.FromResult(_reservations.OrderByDescending(r => r.ReservationTime).ToList());

    public Task<bool> MakeReservationAsync(Reservation reservation)
    {
        if (reservation == null) return Task.FromResult(false);
        reservation.Id = _nextReservationId++;
        reservation.Status = ReservationStatus.Requested;
        _reservations.Add(reservation);
         System.Diagnostics.Debug.WriteLine($"Reservation {reservation.Id} requested for User {reservation.UserId} at {reservation.ReservationTime}.");
        return Task.FromResult(true);
    }

    public Task<bool> UpdateReservationStatusAsync(int reservationId, ReservationStatus status)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
        if (reservation != null)
        {
            reservation.Status = status;
             System.Diagnostics.Debug.WriteLine($"Reservation {reservation.Id} status updated to {status}.");
            return Task.FromResult(true);
        }
        System.Diagnostics.Debug.WriteLine($"Reservation {reservationId} not found for status update.");
        return Task.FromResult(false);
    }

    // User Methods
    public Task<User?> GetUserByUsernameAsync(string username) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));

    public Task<int> GetUserCountByRoleAsync(UserRole role)
    {
        int count = _users.Count(u => u.Role == role);
        return Task.FromResult(count);
    }
}