namespace FlyITA.Core.Options;

public class GuestOptions
{
    public const string SectionName = "Guest";

    public int MainGuestCount { get; set; } = 1;
    public int AdditionalGuestCount { get; set; } = 1;
    public int NonParticipatingGuestCount { get; set; } = 0;
}
