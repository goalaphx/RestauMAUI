using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Restaurant.ViewModels;

public partial class AllReservationsViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    public ObservableCollection<Reservation> Reservations { get; } = new();

    public IAsyncRelayCommand LoadReservationsCommand { get; }
    public IAsyncRelayCommand<Reservation> ChangeReservationStatusCommand { get; }

    public AllReservationsViewModel(IDataService dataService, IAuthService authService, INotificationService notificationService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        Title = "All Reservations";
        LoadReservationsCommand = new AsyncRelayCommand(LoadReservationsAsync);
        ChangeReservationStatusCommand = new AsyncRelayCommand<Reservation>(ChangeReservationStatusAsync);
    }

    async Task LoadReservationsAsync()
    {
        if (IsBusy) return;

        var currentUser = await _authService.GetCurrentUserAsync();
         if (currentUser?.Role != UserRole.Staff)
        {
             if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Access Denied", "You don't have permission to view this page.");
             if (Shell.Current != null)
                await Shell.Current.GoToAsync($"//{nameof(MenuPage)}");
            return;
        }


        IsBusy = true;
        try
        {
            Reservations.Clear();
            var reservations = await _dataService.GetAllReservationsAsync();
            foreach(var res in reservations)
            {
                Reservations.Add(res);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading all reservations: {ex}");
             if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Error", "Failed to load reservations.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ChangeReservationStatusAsync(Reservation? reservationToUpdate)
    {
        if (reservationToUpdate == null || IsBusy) return;

        var statusOptions = Enum.GetNames(typeof(ReservationStatus));

        string? selectedStatusStr = await Shell.Current.DisplayActionSheet(
            $"Update Status for Reservation #{reservationToUpdate.Id}",
            "Cancel",
            null,
            statusOptions);

        if (string.IsNullOrWhiteSpace(selectedStatusStr) || selectedStatusStr == "Cancel")
        {
            return;
        }

        if (Enum.TryParse<ReservationStatus>(selectedStatusStr, out var newStatus))
        {
            IsBusy = true;
            try
            {
                bool success = await _dataService.UpdateReservationStatusAsync(reservationToUpdate.Id, newStatus);
                if (success)
                {
                    reservationToUpdate.Status = newStatus; // Should trigger UI update via INPC
                    if (_notificationService != null)
                        await _notificationService.ShowNotificationAsync("Success", $"Reservation #{reservationToUpdate.Id} status updated to {newStatus}.");
                }
                else
                {
                     if (_notificationService != null)
                        await _notificationService.ShowNotificationAsync("Error", "Failed to update reservation status.");
                }
            }
            catch (Exception ex)
            {
                 Debug.WriteLine($"Error updating reservation status: {ex}");
                 if (_notificationService != null)
                    await _notificationService.ShowNotificationAsync("Error", "An unexpected error occurred.");
            }
            finally
            {
                IsBusy = false;
            }
        }
         else
        {
             if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Error", "Invalid status selected.");
        }
    }
}