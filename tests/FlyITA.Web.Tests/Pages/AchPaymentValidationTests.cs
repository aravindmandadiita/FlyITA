using Xunit;
using FlyITA.Web.Pages;

namespace FlyITA.Web.Tests.Pages;

public class AchPaymentValidationTests
{
    [Theory]
    [InlineData("021000021", true)]   // JPMorgan Chase
    [InlineData("011401533", true)]   // Bank of America
    [InlineData("123456789", false)]  // Invalid checksum
    [InlineData("12345678", false)]   // Too short
    [InlineData("1234567890", false)] // Too long
    [InlineData("abcdefghi", false)]  // Non-numeric
    [InlineData("", false)]           // Empty
    public void IsValidRoutingNumber_ValidatesCorrectly(string routingNumber, bool expected)
    {
        // Use reflection to test the private static method
        var method = typeof(AchPaymentModel).GetMethod("IsValidRoutingNumber",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (bool)method!.Invoke(null, new object[] { routingNumber })!;

        Assert.Equal(expected, result);
    }
}
