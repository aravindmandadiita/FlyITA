using FlyITA.Web.Components;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FlyITA.Web.Tests.Components;

public class MainMenuViewComponentTests
{
    [Fact]
    public void Invoke_Default_ShowsLogoAndMenu()
    {
        var component = new MainMenuViewComponent();
        var result = component.Invoke();

        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var model = Assert.IsType<MainMenuModel>(viewResult.ViewData!.Model);
        Assert.True(model.ShowLogo);
        Assert.True(model.ShowMenu);
    }

    [Fact]
    public void Invoke_HideLogo_ShowLogoFalse()
    {
        var component = new MainMenuViewComponent();
        var result = component.Invoke(showLogo: false);

        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var model = Assert.IsType<MainMenuModel>(viewResult.ViewData!.Model);
        Assert.False(model.ShowLogo);
        Assert.True(model.ShowMenu);
    }

    [Fact]
    public void Invoke_HideMenu_ShowMenuFalse()
    {
        var component = new MainMenuViewComponent();
        var result = component.Invoke(showMenu: false);

        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var model = Assert.IsType<MainMenuModel>(viewResult.ViewData!.Model);
        Assert.True(model.ShowLogo);
        Assert.False(model.ShowMenu);
    }
}
