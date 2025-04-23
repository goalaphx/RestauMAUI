using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class OrderPage : ContentPage
{
    public OrderPage(OrderViewModel viewModel) // ViewModel Injected
    {
        InitializeComponent();
        BindingContext = viewModel; // Set BindingContext
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Always reload orders when the page appears, as new orders might exist
        if (BindingContext is OrderViewModel vm)
        {
            await vm.LoadOrdersCommand.ExecuteAsync(null);
        }
    }
}