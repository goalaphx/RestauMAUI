using Restaurant.Views;
using Restaurant.ViewModels;
using Restaurant.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Restaurant;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var loginPage = MauiProgram.Services.GetRequiredService<LoginPage>();
        MainPage = loginPage;
    }
}