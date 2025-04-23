using Restaurant.Models;
using MenuModel = Restaurant.Models.MenuItem;
using System.Threading.Tasks; // Add this
using System.Collections.Generic; // Add this

namespace Restaurant.Services;

public interface IDataService
{
    // Menu Methods... (Add/Update/Delete/Get)
    Task<List<MenuModel>> GetMenuItemsAsync();
    Task<MenuModel?> GetMenuItemAsync(int id);
    Task<bool> AddMenuItemAsync(MenuModel item);
    Task<bool> UpdateMenuItemAsync(MenuModel item);
    Task<bool> DeleteMenuItemAsync(int itemId);

    // Order Methods... (GetForUser/Get/Place/GetAll/UpdateStatus)
    Task<List<Order>> GetOrdersForUserAsync(int userId);
    Task<Order?> GetOrderAsync(int orderId);
    Task<bool> PlaceOrderAsync(Order order);
    Task<List<Order>> GetAllOrdersAsync();
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);

    // Reservation Methods... (GetForUser/GetAll/Make/UpdateStatus)
    Task<List<Reservation>> GetReservationsForUserAsync(int userId);
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<bool> MakeReservationAsync(Reservation reservation);
    Task<bool> UpdateReservationStatusAsync(int reservationId, ReservationStatus status);

    // User Methods... (GetByUsername)
    Task<User?> GetUserByUsernameAsync(string username);
    Task<int> GetUserCountByRoleAsync(UserRole role); // <-- NEW METHOD
}