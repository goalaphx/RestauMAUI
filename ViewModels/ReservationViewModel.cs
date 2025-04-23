using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Views;
using System.Diagnostics;
using System;
using System.Threading.Tasks;

namespace Restaurant.ViewModels;

public partial class ReservationViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    DateTime reservationDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    TimeSpan reservationTime = new TimeSpan(19, 0, 0);

    [ObservableProperty]
    int partySize = 2;

    [ObservableProperty]
    string customerName;

    [ObservableProperty]
    string customerPhone;

    [ObservableProperty]
    string customerEmail;

    [ObservableProperty]
    string specialRequests;

    public IAsyncRelayCommand SubmitReservationCommand { get; }
    public IAsyncRelayCommand LoadUserDetailsCommand { get; }

    public ReservationViewModel(IDataService dataService, IAuthService authService, INotificationService notificationService)
    {
        _dataService = dataService;
        _authService = authService;
        _notificationService = notificationService;
        Title = "Make a Reservation";
        SubmitReservationCommand = new AsyncRelayCommand(SubmitReservationAsync);
        LoadUserDetailsCommand = new AsyncRelayCommand(LoadUserDetailsAsync);
    }

     async Task LoadUserDetailsAsync()
     {
         var user = await _authService.GetCurrentUserAsync();
         if (user != null)
         {
             CustomerName = user.FullName;
         }
     }

    async Task SubmitReservationAsync()
    {
        if (IsBusy) return;

         if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(CustomerPhone) || PartySize <= 0)
         {
             await _notificationService.ShowNotificationAsync("Validation Error", "Please fill in Name, Phone, and Party Size.");
             return;
         }
          DateTime combinedLocalDateTime = ReservationDate.Date + ReservationTime;
         if (combinedLocalDateTime <= DateTime.Now)
         {
              await _notificationService.ShowNotificationAsync("Validation Error", "Please select a future date and time for the reservation.");
             return;
         }

        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null)
        {
            await _notificationService.ShowNotificationAsync("Error", "You must be logged in to make a reservation.");
            return;
        }

        IsBusy = true;
        try
        {
            // --- FIX: Convert to UTC before assigning ---
            DateTime combinedUtcDateTime = combinedLocalDateTime.ToUniversalTime();
            // --- END FIX ---

            var reservation = new Reservation
            {
                CustomerName = this.CustomerName,
                CustomerPhone = this.CustomerPhone,
                PartySize = this.PartySize,
                ReservationTime = combinedUtcDateTime, // Assign UTC time
                UserId = currentUser.Id
            };

            bool success = await _dataService.MakeReservationAsync(reservation);

            if (success)
            {
                await _notificationService.ShowNotificationAsync("Reservation Requested", "Your reservation request has been received. We will confirm shortly.");
                ClearForm();
                await Shell.Current.GoToAsync($"//{nameof(MyReservationsPage)}");
            }
            else
            {
                await _notificationService.ShowNotificationAsync("Error", "Failed to submit your reservation request. Please try again.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error submitting reservation: {ex}");
            // Log the inner exception as well if it exists
             if(ex.InnerException != null)
             {
                 Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
             }
            await _notificationService.ShowNotificationAsync("Error", "An unexpected error occurred.");
        }
        finally
        {
            IsBusy = false;
        }
    }

     private void ClearForm()
    {
        ReservationDate = DateTime.Today.AddDays(1);
        ReservationTime = new TimeSpan(19, 0, 0);
        PartySize = 2;
        CustomerName = string.Empty;
        CustomerPhone = string.Empty;
        CustomerEmail = string.Empty;
        SpecialRequests = string.Empty;
    }
}