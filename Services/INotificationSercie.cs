namespace Restaurant.Services;

public interface INotificationService
{
    Task ShowNotificationAsync(string title, string message);
}