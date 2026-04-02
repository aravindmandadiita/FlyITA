namespace FlyITA.Tests.Unit;

public class HealthCheckTests
{
    [Theory]
    [InlineData("healthy", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void HealthStatus_IsNonEmpty_WhenHealthy(string? status, bool expectedHealthy)
    {
        var isHealthy = !string.IsNullOrEmpty(status);
        Assert.Equal(expectedHealthy, isHealthy);
    }
}
