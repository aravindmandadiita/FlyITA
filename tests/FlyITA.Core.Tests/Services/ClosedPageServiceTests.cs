using Moq;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class ClosedPageServiceTests
{
    [Fact]
    public void GetClosedMessage_Returns_Message_From_Config()
    {
        var pageConfigMock = new Mock<IPageConfigurationService>();
        pageConfigMock.Setup(p => p.ReadPageConfigSettings("closed.aspx", null))
            .Returns(new Dictionary<string, object> { ["closeregistrationtext"] = "Registration is closed." });

        var service = new ClosedPageService(pageConfigMock.Object);
        var result = service.GetClosedMessage("closed.aspx");

        Assert.Equal("Registration is closed.", result.ClosedMessage);
    }

    [Fact]
    public void GetClosedMessage_Returns_Empty_When_No_Config()
    {
        var pageConfigMock = new Mock<IPageConfigurationService>();
        pageConfigMock.Setup(p => p.ReadPageConfigSettings("closed.aspx", null))
            .Returns(new Dictionary<string, object>());

        var service = new ClosedPageService(pageConfigMock.Object);
        var result = service.GetClosedMessage("closed.aspx");

        Assert.Equal(string.Empty, result.ClosedMessage);
    }
}
