using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel; // Add this
using System.ComponentModel.DataAnnotations.Schema; // Add this

namespace Restaurant.Models;

public partial class Order : ObservableObject // Make partial and inherit
{
    [ObservableProperty]
    int id;

    [ObservableProperty]
    DateTime orderDate;

    // List itself doesn't need change notification usually, content changes handled elsewhere
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    [ObservableProperty]
    [property: Column(TypeName = "decimal(18,2)")]
    decimal totalAmount;

    [ObservableProperty]
    int userId;

    [ObservableProperty] // Notify UI when status changes
    OrderStatus status;

    [ObservableProperty]
    string? paymentId;
}