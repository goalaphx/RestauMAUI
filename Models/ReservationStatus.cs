namespace Restaurant.Models; // Or your actual Models namespace

public enum ReservationStatus
{
    Requested,  // Initial status when a client makes a request
    Confirmed,  // Staff has confirmed the reservation
    Cancelled,  // Reservation has been cancelled (by client or staff)
    Seated,     // Client has arrived and been seated (Optional)
    NoShow      // Client did not show up for the reservation (Optional)
}