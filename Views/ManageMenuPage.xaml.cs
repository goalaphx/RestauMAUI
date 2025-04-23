using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class ManageMenuPage : ContentPage
{
    public ManageMenuPage(ManageMenuViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ManageMenuViewModel vm && vm.LoadItemsCommand.CanExecute(null))
        {
            await vm.LoadItemsCommand.ExecuteAsync(null);
        }
    }
}