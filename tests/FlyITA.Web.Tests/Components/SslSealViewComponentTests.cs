using FlyITA.Web.Components;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FlyITA.Web.Tests.Components;

public class SslSealViewComponentTests
{
    [Fact]
    public void Invoke_ReturnsViewResult()
    {
        var component = new SslSealViewComponent();
        var result = component.Invoke();
        Assert.IsType<ViewViewComponentResult>(result);
    }

    [Fact]
    public void SslSealHeader_Invoke_ReturnsViewResult()
    {
        var component = new SslSealHeaderViewComponent();
        var result = component.Invoke();
        Assert.IsType<ViewViewComponentResult>(result);
    }
}
