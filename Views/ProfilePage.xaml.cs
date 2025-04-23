using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
        {
            // Explicitly await the command if needed, or just execute
            await vm.LoadUserProfileCommand.ExecuteAsync(null);
        }
    }
}