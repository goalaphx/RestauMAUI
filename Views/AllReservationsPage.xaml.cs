using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class AllReservationsPage : ContentPage
{
    public AllReservationsPage(AllReservationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AllReservationsViewModel vm)
        {
            if(vm.LoadReservationsCommand.CanExecute(null))
            {
                await vm.LoadReservationsCommand.ExecuteAsync(null);
            }
        }
    }
}