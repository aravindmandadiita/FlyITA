using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;

namespace FlyITA.Core.Services;

public class ClosedPageService : IClosedPageService
{
    private readonly IPageConfigurationService _pageConfig;

    public ClosedPageService(IPageConfigurationService pageConfig)
    {
        _pageConfig = pageConfig;
    }

    public ClosedPageModel GetClosedMessage(string currentPage)
    {
        var model = new ClosedPageModel();

        var settings = _pageConfig.ReadPageConfigSettings(currentPage);

        if (settings.TryGetValue("closeregistrationtext", out var value) && value is string message)
        {
            model.ClosedMessage = message;
        }

        return model;
    }
}
