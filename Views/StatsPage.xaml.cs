using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class StatsPage : ContentPage
{
    public StatsPage(StatsViewModel viewModel) // ViewModel Injected
    {
        InitializeComponent();
        BindingContext = viewModel; // Set BindingContext
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Load stats when the page appears
        if (BindingContext is StatsViewModel vm)
        {
            await vm.LoadStatsCommand.ExecuteAsync(null);
        }
    }
}