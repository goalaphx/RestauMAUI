using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class MyReservationsPage : ContentPage
{
    public MyReservationsPage(MyReservationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MyReservationsViewModel vm)
        {
            if(vm.LoadReservationsCommand.CanExecute(null))
            {
                await vm.LoadReservationsCommand.ExecuteAsync(null);
            }
        }
    }
}