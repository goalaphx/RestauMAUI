// Services/PostgresDataService.cs
using Microsoft.EntityFrameworkCore;
using Restaurant.Data; // Context namespace
using Restaurant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MenuModel = Restaurant.Models.MenuItem; // Alias if needed

namespace Restaurant.Services;

public class PostgresDataService : IDataService
{
    private readonly IDbContextFactory<RestaurantDbContext> _contextFactory;

    public PostgresDataService(IDbContextFactory<RestaurantDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // --- Menu ---
    public async Task<List<MenuModel>> GetMenuItemsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MenuItems.AsNoTracking().ToListAsync();
    }

    public async Task<MenuModel?> GetMenuItemAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MenuItems.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> AddMenuItemAsync(MenuModel item)
    {
        if (item == null) return false;
        // Ensure ID is 0 for auto-increment
        item.Id = 0;
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.MenuItems.Add(item);
        int result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateMenuItemAsync(MenuModel item)
    {
        if (item == null) return false;
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.MenuItems.Update(item); // Tracks the entity for update
        // Or: context.Entry(item).State = EntityState.Modified;
        int result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteMenuItemAsync(int itemId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var item = await context.MenuItems.FindAsync(itemId);
        if (item == null) return false;
        context.MenuItems.Remove(item);
        int result = await context.SaveChangesAsync();
        return result > 0;
    }

    // --- Orders ---
    public async Task<List<Order>> GetOrdersForUserAsync(int userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items) // Eager load items
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderAsync(int orderId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
         return await context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<bool> PlaceOrderAsync(Order order)
    {
        if (order == null || !order.Items.Any()) return false;
        order.Id = 0; // Ensure auto-increment
        order.OrderDate = DateTime.UtcNow; // Use UTC time
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Orders.Add(order); // EF Core handles adding related OrderItems too
        int result = await context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<int> GetUserCountByRoleAsync(UserRole role)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.CountAsync(u => u.Role == role);
    }
    
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

     public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
     {
         await using var context = await _contextFactory.CreateDbContextAsync();
         var order = await context.Orders.FindAsync(orderId);
         if (order == null) return false;
         order.Status = status;
         context.Orders.Update(order);
         int result = await context.SaveChangesAsync();
         return result > 0;
     }

    // --- Reservations ---
     public async Task<List<Reservation>> GetReservationsForUserAsync(int userId)
     {
         await using var context = await _contextFactory.CreateDbContextAsync();
         return await context.Reservations
            .Where(r => r.UserId == userId)
            .AsNoTracking()
            .OrderByDescending(r => r.ReservationTime)
            .ToListAsync();
     }

    public async Task<List<Reservation>> GetAllReservationsAsync()
    {
         await using var context = await _contextFactory.CreateDbContextAsync();
         return await context.Reservations
            .AsNoTracking()
            .OrderByDescending(r => r.ReservationTime)
            .ToListAsync();
    }

     public async Task<bool> MakeReservationAsync(Reservation reservation)
     {
         if (reservation == null) return false;
         reservation.Id = 0; // Ensure auto-increment
         reservation.Status = ReservationStatus.Requested;
         await using var context = await _contextFactory.CreateDbContextAsync();
         context.Reservations.Add(reservation);
         int result = await context.SaveChangesAsync();
         return result > 0;
     }

     public async Task<bool> UpdateReservationStatusAsync(int reservationId, ReservationStatus status)
     {
         await using var context = await _contextFactory.CreateDbContextAsync();
         var reservation = await context.Reservations.FindAsync(reservationId);
         if (reservation == null) return false;
         reservation.Status = status;
         context.Reservations.Update(reservation);
         int result = await context.SaveChangesAsync();
         return result > 0;
     }

    // --- Users ---
    // Note: User interactions (login, register) are often better handled by a dedicated
    // AuthService that might still use the DbContext, but focuses on auth logic.
    // MockAuthService won't work directly with the DB unless adapted.
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            // Use case-insensitive comparison suitable for your DB collation if needed
    }
}