namespace Restaurant.Services;

public interface IPaymentService
{
    Task<(bool Success, string TransactionId)> ProcessPaymentAsync(decimal amount);
}