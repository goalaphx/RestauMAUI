using Microsoft.Extensions.Logging;
using Restaurant.Services;
using Restaurant.ViewModels;
using Restaurant.Views;
using Restaurant.Converters;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace Restaurant;

public static class MauiProgram
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres";

    private static MauiApp? _mauiApp;

    public static MauiApp CreateMauiApp()
    {
        if (_mauiApp == null)
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            #if DEBUG
            builder.Logging.AddDebug();
            #endif

            builder.Services.AddDbContextFactory<RestaurantDbContext>(options =>
                options.UseNpgsql(ConnectionString));

            builder.Services.AddSingleton<IDataService, PostgresDataService>();
            builder.Services.AddSingleton<IAuthService, MockAuthService>();
            builder.Services.AddSingleton<INotificationService, MockNotificationService>();
            builder.Services.AddSingleton<IPaymentService, MockPaymentService>();

            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddSingleton<CartViewModel>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MenuViewModel>();
            builder.Services.AddTransient<OrderViewModel>();
            builder.Services.AddTransient<ReservationViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<StatsViewModel>();
            builder.Services.AddTransient<MyReservationsViewModel>();
            builder.Services.AddTransient<AllOrdersViewModel>();
            builder.Services.AddTransient<AllReservationsViewModel>();
            builder.Services.AddTransient<ManageMenuViewModel>();

            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MenuPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<OrderPage>();
            builder.Services.AddTransient<ReservationPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<StatsPage>();
            builder.Services.AddTransient<MyReservationsPage>();
            builder.Services.AddTransient<AllOrdersPage>();
            builder.Services.AddTransient<AllReservationsPage>();
            builder.Services.AddTransient<ManageMenuPage>();
            builder.Services.AddTransient<MainPage>();

             _mauiApp = builder.Build();
        }
        return _mauiApp;
    }

    public static IServiceProvider Services => _mauiApp?.Services ?? throw new InvalidOperationException("MauiApp not built yet.");
}