using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models;

public partial class MenuItem : ObservableObject
{
    [ObservableProperty]
    int id;

    [ObservableProperty]
    string? name;

    [ObservableProperty]
    string? description;

    [ObservableProperty]
    [property: Column(TypeName = "decimal(18,2)")] // Keep attribute on generated property
    decimal price;

    [ObservableProperty]
    MenuCategory category;

    [ObservableProperty]
    string? imageUrl;
}