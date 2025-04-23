using System;
using CommunityToolkit.Mvvm.ComponentModel; // Add this

namespace Restaurant.Models;

// Make partial and inherit
public partial class Reservation : ObservableObject
{
    [ObservableProperty]
    int id;

    [ObservableProperty]
    DateTime reservationTime;

    [ObservableProperty]
    int partySize;

    [ObservableProperty]
    string customerName;

    [ObservableProperty]
    string customerPhone;

    [ObservableProperty] // Notify UI when status changes
    ReservationStatus status;

    [ObservableProperty]
    int? tableNumber;

    [ObservableProperty]
    int userId;
}