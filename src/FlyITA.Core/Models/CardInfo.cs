namespace FlyITA.Core.Models;

public class CardInfo
{
    public string Token { get; set; } = string.Empty;
    public string LastFourDigits { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
}
