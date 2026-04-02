namespace FlyITA.Core.Models;

public class PaymentRequest
{
    public string Token { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
    public int ParticipantId { get; set; }
}
