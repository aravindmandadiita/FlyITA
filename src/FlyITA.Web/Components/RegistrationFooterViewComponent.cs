using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FlyITA.Core.Options;

namespace FlyITA.Web.Components;

public class RegistrationFooterViewComponent : ViewComponent
{
    private readonly ProgramOptions _options;

    public RegistrationFooterViewComponent(IOptions<ProgramOptions> options)
    {
        _options = options.Value;
    }

    public IViewComponentResult Invoke() => View(_options);
}
