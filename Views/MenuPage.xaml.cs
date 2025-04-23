using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class MenuPage : ContentPage
{
    // Keep a reference if needed for events, otherwise BindingContext is enough
    // private readonly MenuViewModel _viewModel;

    public MenuPage(MenuViewModel viewModel) // ViewModel Injected
    {
        InitializeComponent();
        BindingContext = viewModel; // Set BindingContext
        // _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Load data when page appears if not already loaded
        if (BindingContext is MenuViewModel vm && vm.MenuItems.Count == 0) // Check if data needs loading
        {
            await vm.LoadMenuItemsCommand.ExecuteAsync(null);
        }
    }
}