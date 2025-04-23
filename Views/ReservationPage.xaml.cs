using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class ReservationPage : ContentPage
{
    public ReservationPage(ReservationViewModel viewModel) // ViewModel Injected
    {
        InitializeComponent();
        BindingContext = viewModel; // Set BindingContext
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Pre-fill user details if needed when page appears
        if (BindingContext is ReservationViewModel vm)
        {
            // Check if name is empty, indicating details might need loading
            if(string.IsNullOrEmpty(vm.CustomerName))
            {
                await vm.LoadUserDetailsCommand.ExecuteAsync(null);
            }
        }
    }
}