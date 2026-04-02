using Microsoft.Extensions.Options;
using FlyITA.Web.Options;
using FlyITA.Web.Services;

using MSOptions = Microsoft.Extensions.Options.Options;

namespace FlyITA.Web.Tests.Services;

public class SmtpClientWrapperTests
{
    [Fact]
    public void Constructor_AcceptsNetworkOptions()
    {
        var options = MSOptions.Create(new SmtpOptions
        {
            DeliveryMethod = "Network",
            Host = "smtp.test.com",
            Port = 587,
            EnableSsl = true,
            UserName = "user",
            Password = "pass"
        });

        var wrapper = new SmtpClientWrapper(options);
        Assert.NotNull(wrapper);
    }

    [Fact]
    public void Constructor_AcceptsPickupDirectoryOptions()
    {
        var options = MSOptions.Create(new SmtpOptions
        {
            DeliveryMethod = "PickupDirectory",
            PickupDirectoryLocation = @"C:\inetpub\mailroot\Pickup"
        });

        var wrapper = new SmtpClientWrapper(options);
        Assert.NotNull(wrapper);
    }

    [Fact]
    public void Constructor_AcceptsDefaultOptions()
    {
        var wrapper = new SmtpClientWrapper(MSOptions.Create(new SmtpOptions()));
        Assert.NotNull(wrapper);
    }
}
