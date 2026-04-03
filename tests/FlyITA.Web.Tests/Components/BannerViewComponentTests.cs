using FlyITA.Web.Components;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FlyITA.Web.Tests.Components;

public class BannerViewComponentTests
{
    [Fact]
    public void Invoke_HomePage_ReturnsViewWithTrue()
    {
        var component = new BannerViewComponent();
        var result = component.Invoke(isHomePage: true);

        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        Assert.Equal(true, viewResult.ViewData!.Model);
    }

    [Fact]
    public void Invoke_InternalPage_ReturnsViewWithFalse()
    {
        var component = new BannerViewComponent();
        var result = component.Invoke(isHomePage: false);

        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        Assert.Equal(false, viewResult.ViewData!.Model);
    }

    [Fact]
    public void Invoke_DefaultParameter_ReturnsFalse()
    {
        var component = new BannerViewComponent();
        var result = component.Invoke();

        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        Assert.Equal(false, viewResult.ViewData!.Model);
    }
}
