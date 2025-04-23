namespace Restaurant.Models; // Or your actual Models namespace

public enum OrderStatus
{
    Pending,    // Order created but not yet confirmed or paid
    Confirmed,  // Order accepted by the restaurant
    Paid,       // Payment received
    Preparing,  // Food is being prepared (Optional status)
    Ready,      // Order is ready for pickup/delivery (Optional status)
    Delivered,  // Order received by customer
    Cancelled   // Order was cancelled
}