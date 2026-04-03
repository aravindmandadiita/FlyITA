using Microsoft.AspNetCore.Mvc;

namespace FlyITA.Web.Components;

public class MainMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(bool showLogo = true, bool showMenu = true)
        => View(new MainMenuModel { ShowLogo = showLogo, ShowMenu = showMenu });
}

public class MainMenuModel
{
    public bool ShowLogo { get; set; } = true;
    public bool ShowMenu { get; set; } = true;
}
