using Restaurant.ViewModels;

namespace Restaurant.Views;

public partial class AllOrdersPage : ContentPage
{
    public AllOrdersPage(AllOrdersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AllOrdersViewModel vm)
        {
            if(vm.LoadOrdersCommand.CanExecute(null))
            {
                await vm.LoadOrdersCommand.ExecuteAsync(null);
            }
        }
    }
}