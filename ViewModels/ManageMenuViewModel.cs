using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using MenuModel = Restaurant.Models.MenuItem;

namespace Restaurant.ViewModels;

public partial class ManageMenuViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly INotificationService _notificationService;
    private readonly IAuthService _authService;

    public ObservableCollection<MenuModel> MenuItems { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEditingOrAdding))]
    bool isEditingOrAdding = false;

    private MenuModel? _editableMenuItem;
    public MenuModel? EditableMenuItem
    {
        get => _editableMenuItem;
        set
        {
            if (_editableMenuItem != null)
            {
                _editableMenuItem.PropertyChanged -= EditableMenuItem_PropertyChanged;
            }
            if (SetProperty(ref _editableMenuItem, value))
            {
                 if (_editableMenuItem != null)
                 {
                     _editableMenuItem.PropertyChanged += EditableMenuItem_PropertyChanged;
                 }
                 SaveItemCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private void EditableMenuItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MenuModel.Name) || e.PropertyName == nameof(MenuModel.Price))
        {
            SaveItemCommand.NotifyCanExecuteChanged();
        }
    }

    public bool IsNotEditingOrAdding => !IsEditingOrAdding;

    public IAsyncRelayCommand LoadItemsCommand { get; }
    public IRelayCommand AddNewItemCommand { get; }
    public IRelayCommand<MenuModel> BeginEditItemCommand { get; }
    public IAsyncRelayCommand SaveItemCommand { get; }
    public IRelayCommand CancelEditCommand { get; }
    public IAsyncRelayCommand<MenuModel> DeleteItemCommand { get; }

    public List<MenuCategory> CategoryOptions { get; } = Enum.GetValues(typeof(MenuCategory)).Cast<MenuCategory>().ToList();


    public ManageMenuViewModel(IDataService dataService, INotificationService notificationService, IAuthService authService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        Title = "Manage Menu";

        LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
        AddNewItemCommand = new RelayCommand(AddNewItem);
        BeginEditItemCommand = new RelayCommand<MenuModel>(BeginEditItem);
        SaveItemCommand = new AsyncRelayCommand(SaveItemAsync, CanSaveItem);
        CancelEditCommand = new RelayCommand(CancelEdit);
        DeleteItemCommand = new AsyncRelayCommand<MenuModel>(DeleteItemAsync);
    }

    private async Task CheckAccess()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user?.Role != UserRole.Staff)
        {
            if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Access Denied", "You do not have permission to manage the menu.");

            if (Shell.Current != null)
                await Shell.Current.GoToAsync($"//{nameof(MenuPage)}");

            throw new UnauthorizedAccessException("User is not Staff.");
        }
    }

    async Task LoadItemsAsync()
    {
        if (IsBusy) return;
        try
        {
            await CheckAccess();
            IsBusy = true;
            MenuItems.Clear();
            var items = await _dataService.GetMenuItemsAsync();
            foreach (var item in items.OrderBy(i => i.Category).ThenBy(i => i.Name))
            {
                MenuItems.Add(item);
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading menu items for management: {ex}");
             if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Error", "Could not load menu items.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    void AddNewItem()
    {
        EditableMenuItem = new MenuModel();
        IsEditingOrAdding = true;
    }

    void BeginEditItem(MenuModel? itemToEdit)
    {
        if (itemToEdit == null) return;
        EditableMenuItem = new MenuModel
        {
            Id = itemToEdit.Id,
            Name = itemToEdit.Name,
            Description = itemToEdit.Description,
            Price = itemToEdit.Price,
            Category = itemToEdit.Category,
            ImageUrl = itemToEdit.ImageUrl
        };
        IsEditingOrAdding = true;
    }

     bool CanSaveItem()
    {
        return EditableMenuItem != null
               && !string.IsNullOrWhiteSpace(EditableMenuItem.Name)
               && EditableMenuItem.Price > 0;
    }

    async Task SaveItemAsync()
    {
        if (!CanSaveItem() || EditableMenuItem == null)
        {
             if (_notificationService != null)
                await _notificationService.ShowNotificationAsync("Validation Error", "Please provide a Name and a valid Price.");
            return;
        }
        if (IsBusy) return;

        try
        {
            await CheckAccess();
            IsBusy = true;
            bool success;
            string successMessage;

            if (EditableMenuItem.Id == 0)
            {
                success = await _dataService.AddMenuItemAsync(EditableMenuItem);
                successMessage = "Item added successfully.";
                if (success && EditableMenuItem != null) // Check EditableMenuItem again
                {
                     MenuItems.Add(EditableMenuItem);
                }
            }
            else
            {
                success = await _dataService.UpdateMenuItemAsync(EditableMenuItem);
                successMessage = "Item updated successfully.";
                if (success)
                {
                    var itemInList = MenuItems.FirstOrDefault(m => m.Id == EditableMenuItem.Id);
                    if (itemInList != null)
                    {
                        itemInList.Name = EditableMenuItem.Name;
                        itemInList.Description = EditableMenuItem.Description;
                        itemInList.Price = EditableMenuItem.Price;
                        itemInList.Category = EditableMenuItem.Category;
                        itemInList.ImageUrl = EditableMenuItem.ImageUrl;
                    }
                    else {
                         await LoadItemsAsync();
                    }
                }
            }

            if (success)
            {
                 if (_notificationService != null)
                    await _notificationService.ShowNotificationAsync("Success", successMessage);
                IsEditingOrAdding = false;
                EditableMenuItem = null;
            }
            else
            {
                 if (_notificationService != null)
                    await _notificationService.ShowNotificationAsync("Error", "Failed to save the item.");
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (Exception ex)
        {
             Debug.WriteLine($"Error saving menu item: {ex}");
              if (_notificationService != null)
                 await _notificationService.ShowNotificationAsync("Error", "An unexpected error occurred while saving.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    void CancelEdit()
    {
        IsEditingOrAdding = false;
        EditableMenuItem = null;
    }

    async Task DeleteItemAsync(MenuModel? itemToDelete)
    {
        if (itemToDelete == null || IsBusy) return;

        bool confirmed = await Shell.Current.DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete '{itemToDelete.Name}'?",
            "Yes, Delete",
            "Cancel");

        if (!confirmed) return;

        try
        {
            await CheckAccess();
            IsBusy = true;
            bool success = await _dataService.DeleteMenuItemAsync(itemToDelete.Id);

            if (success)
            {
                  if (_notificationService != null)
                     await _notificationService.ShowNotificationAsync("Deleted", $"'{itemToDelete.Name}' was deleted.");
                 MenuItems.Remove(itemToDelete);
            }
            else
            {
                  if (_notificationService != null)
                     await _notificationService.ShowNotificationAsync("Error", "Failed to delete the item.");
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting menu item: {ex}");
             if (_notificationService != null)
                 await _notificationService.ShowNotificationAsync("Error", "An unexpected error occurred during deletion.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}