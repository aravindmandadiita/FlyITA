using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface ICardProcessService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken ct = default);
    Task<PaymentResult> RefundAsync(string transactionId, decimal amount, CancellationToken ct = default);
}
