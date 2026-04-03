using Microsoft.AspNetCore.Mvc;

namespace FlyITA.Web.Components;

public class CustomFieldViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CustomFieldViewModel model) => View(model);
}
