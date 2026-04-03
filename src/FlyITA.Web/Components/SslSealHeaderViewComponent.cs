using Microsoft.AspNetCore.Mvc;

namespace FlyITA.Web.Components;

public class SslSealHeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
