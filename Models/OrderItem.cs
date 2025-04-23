// Models/OrderItem.cs
namespace Restaurant.Models; // Or your models namespace

public class OrderItem
{
    // --- ADD PRIMARY KEY ---
    public int Id { get; set; } // Primary Key for the OrderItems table itself
    // --- END ADDITION ---

    public int MenuItemId { get; set; } // Foreign key reference to MenuItem
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    // Optional: Foreign Key to Order (EF Core might infer this, but can be explicit)
    // public int OrderId { get; set; }
    // public Order Order { get; set; } // Navigation property back to Order
}