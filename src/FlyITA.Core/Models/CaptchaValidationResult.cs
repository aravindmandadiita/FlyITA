namespace FlyITA.Core.Models;

public class CaptchaValidationResult
{
    public bool IsValid { get; set; }
    public double Score { get; set; }
    public string? ErrorMessage { get; set; }
}
