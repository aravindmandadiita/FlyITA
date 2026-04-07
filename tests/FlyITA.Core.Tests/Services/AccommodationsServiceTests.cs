using Moq;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class AccommodationsServiceTests
{
    private readonly Mock<IContextManager> _contextMock = new();
    private readonly Mock<IPCentralDataAccess> _dataMock = new();
    private readonly Mock<INavigationService> _navMock = new();

    private AccommodationsService CreateService() => new(_contextMock.Object, _dataMock.Object, _navMock.Object);

    [Fact]
    public async Task GetNextPageAsync_Standard_Returns_Default()
    {
        _contextMock.SetupGet(c => c.ParticipantID).Returns(1);
        _dataMock.Setup(d => d.GetAccommodationDetailsAsync(1))
            .ReturnsAsync(new Dictionary<string, object?> { ["IsMultiDestination"] = false, ["CruiseCabinBlockID"] = 0 });
        _navMock.Setup(n => n.GetNextPageByPage("accommodations", "default")).Returns("events.aspx");

        var result = new ValidationResult();
        Assert.Equal("events.aspx", await CreateService().GetNextPageAsync(result));
    }

    [Fact]
    public async Task GetNextPageAsync_Cruise_Returns_Cruise_Page()
    {
        _contextMock.SetupGet(c => c.ParticipantID).Returns(1);
        _dataMock.Setup(d => d.GetAccommodationDetailsAsync(1))
            .ReturnsAsync(new Dictionary<string, object?> { ["IsMultiDestination"] = false, ["CruiseCabinBlockID"] = 5 });
        _navMock.Setup(n => n.GetNextPageByPage("accommodations", "cruise")).Returns("cruise.aspx");

        var result = new ValidationResult();
        Assert.Equal("cruise.aspx", await CreateService().GetNextPageAsync(result));
    }

    [Fact]
    public async Task GetNextPageAsync_MultiDestination_Returns_MultiDest_Page()
    {
        _contextMock.SetupGet(c => c.ParticipantID).Returns(1);
        _dataMock.Setup(d => d.GetAccommodationDetailsAsync(1))
            .ReturnsAsync(new Dictionary<string, object?> { ["IsMultiDestination"] = true });
        _navMock.Setup(n => n.GetNextPageByPage("accommodations", "multidestination")).Returns("multidest.aspx");

        var result = new ValidationResult();
        Assert.Equal("multidest.aspx", await CreateService().GetNextPageAsync(result));
    }

    [Fact]
    public async Task SaveOverNightInFlightAsync_Saves_And_Sets_Context()
    {
        _contextMock.SetupGet(c => c.ParticipantID).Returns(42);

        await CreateService().SaveOverNightInFlightAsync();

        _dataMock.Verify(d => d.SaveAccommodationRecordAsync(42, It.IsAny<Dictionary<string, object?>>()), Times.Once);
        _contextMock.VerifySet(c => c.OvernightInFlight = true);
    }

    [Fact]
    public async Task RemoveOverNightInFlightAsync_Deletes_And_Clears_Context()
    {
        _contextMock.SetupGet(c => c.ParticipantID).Returns(42);

        await CreateService().RemoveOverNightInFlightAsync();

        _dataMock.Verify(d => d.DeleteAccommodationRecordAsync(42, "OvernightInFlight"), Times.Once);
        _contextMock.VerifySet(c => c.OvernightInFlight = false);
    }

    [Fact]
    public void IsMultiDestination_Returns_Result_Flag()
    {
        var result = new AccommodationResult { IsMultiDestination = true };
        Assert.True(CreateService().IsMultiDestination(result));
    }
}
