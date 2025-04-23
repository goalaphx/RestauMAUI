// AppShell.xaml.cs
using Restaurant.ViewModels;
using Restaurant.Views;

namespace Restaurant;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel viewModel)
    {
        // Set BindingContext BEFORE InitializeComponent
        BindingContext = viewModel;
        InitializeComponent(); // Handles XAML loading
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(MenuPage), typeof(MenuPage));
        Routing.RegisterRoute(nameof(CartPage), typeof(CartPage));
        Routing.RegisterRoute(nameof(OrderPage), typeof(OrderPage));
        Routing.RegisterRoute(nameof(ReservationPage), typeof(ReservationPage));
        Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
        Routing.RegisterRoute(nameof(MyReservationsPage), typeof(MyReservationsPage));
        Routing.RegisterRoute(nameof(AllOrdersPage), typeof(AllOrdersPage));
        Routing.RegisterRoute(nameof(AllReservationsPage), typeof(AllReservationsPage));
        Routing.RegisterRoute(nameof(ManageMenuPage), typeof(ManageMenuPage));
    }
}