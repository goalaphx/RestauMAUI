using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel) // ViewModel Injected
    {
        InitializeComponent();
        BindingContext = viewModel; // Set BindingContext
    }

    // Prevent going back from Login page if using Modal or replacing MainPage
    protected override bool OnBackButtonPressed() => true;
}