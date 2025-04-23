using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Views; // For navigation if needed
using System.Collections.ObjectModel;
using System.Diagnostics;
using MenuModel = Restaurant.Models.MenuItem;

namespace Restaurant.ViewModels;

public partial class MenuViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly CartViewModel _cartViewModel; // Inject singleton CartViewModel
    private readonly INotificationService _notificationService;

    public ObservableCollection<MenuModel> MenuItems { get; } = new();

    // Optional: Grouping
    // public ObservableCollection<Grouping<MenuCategory, MenuItem>> GroupedMenuItems { get; } = new();

    public MenuViewModel(IDataService dataService, CartViewModel cartViewModel, INotificationService notificationService)
    {
        _dataService = dataService;
        _cartViewModel = cartViewModel;
        _notificationService = notificationService;
        Title = "Menu";
    }

    [RelayCommand]
    async Task LoadMenuItemsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            MenuItems.Clear();
            // GroupedMenuItems.Clear(); // If using grouping

            var items = await _dataService.GetMenuItemsAsync();
            foreach (var item in items.OrderBy(i => i.Category).ThenBy(i => i.Name))
            {
                MenuItems.Add(item);
            }

            // --- Optional Grouping Logic ---
            // var grouped = items.GroupBy(i => i.Category)
            //                    .OrderBy(g => g.Key)
            //                    .Select(g => new Grouping<MenuCategory, MenuItem>(g.Key, g.OrderBy(i=>i.Name)));
            // foreach(var group in grouped)
            // {
            //     GroupedMenuItems.Add(group);
            // }
            // --- End Grouping ---

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading menu items: {ex}");
            await _notificationService.ShowNotificationAsync("Error", "Failed to load menu.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void AddToCart(MenuModel? item)
    {
        if (item == null) return;

        _cartViewModel.AddItemCommand.Execute(item);
        // Optionally show a quick confirmation (Toast notification is better UX than Alert here)
        // Using Alert for simplicity:
        // await _notificationService.ShowNotificationAsync("Added", $"{item.Name} added to cart.");
        Debug.WriteLine($"{item.Name} added to cart.");
    }

    [RelayCommand]
    async Task GoToCartAsync()
    {
         // Assumes CartPage is registered with this route name in AppShell.xaml.cs
        await Shell.Current.GoToAsync(nameof(CartPage));
    }
}
// Helper class for grouping (if used)
public class Grouping<K, T> : ObservableCollection<T>
{
    public K Key { get; private set; }
    public Grouping(K key, IEnumerable<T> items)
    {
        Key = key;
        foreach (var item in items)
            this.Items.Add(item);
    }
}

