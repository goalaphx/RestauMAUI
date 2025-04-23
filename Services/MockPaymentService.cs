namespace Restaurant.Services;

public class MockPaymentService : IPaymentService
{
    public async Task<(bool Success, string TransactionId)> ProcessPaymentAsync(decimal amount)
    {
        System.Diagnostics.Debug.WriteLine($"Attempting payment for {amount:C}");
        await Task.Delay(1500); // Simulate network delay

        // Simulate success/failure (e.g., succeed 90% of the time)
        bool success = new Random().Next(10) != 0;

        if (success)
        {
            string transactionId = $"MOCK_TRANS_{Guid.NewGuid().ToString().Substring(0, 8)}";
            System.Diagnostics.Debug.WriteLine($"Payment Successful. Transaction ID: {transactionId}");
            return (true, transactionId);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Payment Failed.");
            return (false, string.Empty);
        }
    }
}