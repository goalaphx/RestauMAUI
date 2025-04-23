namespace Restaurant.Services;

public class MockNotificationService : INotificationService
{
    public async Task ShowNotificationAsync(string title, string message)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }
        else
        {
            // Fallback if Shell is not available (e.g., during startup)
            System.Diagnostics.Debug.WriteLine($"NOTIFICATION: {title} - {message}");
        }
    }
}