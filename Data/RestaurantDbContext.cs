// Data/RestaurantDbContext.cs
using Microsoft.EntityFrameworkCore;
using Restaurant.Models; // Assuming models are in Restaurant.Models
using MenuModel = Restaurant.Models.MenuItem;

namespace Restaurant.Data;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

    public DbSet<MenuModel> MenuItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<MenuModel>()
            .Property(p => p.Price) // 'p' is fine here, or use 'm' for MenuItem
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.Price) // Use 'oi' for OrderItem, or 'i'
            .HasColumnType("decimal(18,2)");

        // --- FIX IS HERE ---
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount) // Changed 'p' to 'o'
            .HasColumnType("decimal(18,2)");
        // --- END OF FIX ---
    }
}