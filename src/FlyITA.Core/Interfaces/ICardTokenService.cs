using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface ICardTokenService
{
    Task<string?> TokenizeCardAsync(string cardNumber, string expiryDate, CancellationToken ct = default);
    Task<CardInfo?> GetCardInfoByTokenAsync(string token, CancellationToken ct = default);
}
