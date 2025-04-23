using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class CartPage : ContentPage
{
    public CartPage(CartViewModel viewModel) // ViewModel Injected
    {
        InitializeComponent();
        BindingContext = viewModel; // Set BindingContext (Singleton VM)
    }

    // Optional: Recalculate/refresh things when page appears, though CartViewModel should handle this reactively
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // If CartViewModel properties need explicit refreshing based on external factors, do it here.
        // Example: Maybe re-check item availability if prices change frequently.
        // (CartViewModel already updates total reactively)
    }
}