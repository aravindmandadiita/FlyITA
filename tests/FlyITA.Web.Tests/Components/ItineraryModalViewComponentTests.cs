using FlyITA.Web.Components;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FlyITA.Web.Tests.Components;

public class ItineraryModalViewComponentTests
{
    [Fact]
    public void Invoke_ReturnsViewResult()
    {
        var component = new ItineraryModalViewComponent();
        var result = component.Invoke();
        Assert.IsType<ViewViewComponentResult>(result);
    }
}
