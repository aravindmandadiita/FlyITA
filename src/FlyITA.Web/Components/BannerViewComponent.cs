using Microsoft.AspNetCore.Mvc;

namespace FlyITA.Web.Components;

public class BannerViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(bool isHomePage = false) => View(isHomePage);
}
