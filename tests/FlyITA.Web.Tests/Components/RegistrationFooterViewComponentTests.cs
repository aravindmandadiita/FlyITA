using FlyITA.Core.Options;
using FlyITA.Web.Components;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using MsOptions = Microsoft.Extensions.Options;

namespace FlyITA.Web.Tests.Components;

public class RegistrationFooterViewComponentTests
{
    [Fact]
    public void Invoke_WithContactInfo_PassesModelToView()
    {
        var options = MsOptions.Options.Create(new ProgramOptions
        {
            ContactName = "Travel HQ",
            ContactEmail = "travel@ita.com",
            ContactPhone = "800-555-1234"
        });
        var component = new RegistrationFooterViewComponent(options);

        var result = component.Invoke();

        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var model = Assert.IsType<ProgramOptions>(viewResult.ViewData!.Model);
        Assert.Equal("Travel HQ", model.ContactName);
        Assert.Equal("travel@ita.com", model.ContactEmail);
        Assert.Equal("800-555-1234", model.ContactPhone);
    }

    [Fact]
    public void Invoke_WithEmptyContact_PassesEmptyModel()
    {
        var options = MsOptions.Options.Create(new ProgramOptions());
        var component = new RegistrationFooterViewComponent(options);

        var result = component.Invoke();

        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var model = Assert.IsType<ProgramOptions>(viewResult.ViewData!.Model);
        Assert.Equal(string.Empty, model.ContactName);
        Assert.Equal(string.Empty, model.ContactEmail);
        Assert.Equal(string.Empty, model.ContactPhone);
    }
}
