using Moq;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class FeesDisplayServiceTests
{
    private readonly Mock<IContextManager> _contextMock = new();
    private readonly Mock<IAccommodationsService> _accomMock = new();
    private FeesDisplayService CreateService() => new(_contextMock.Object, _accomMock.Object);

    [Fact]
    public void GetFeeDisplayTravelType_ITAAir_Returns_ITAAir()
    {
        _contextMock.SetupGet(c => c.TransportationType).Returns("ITAAir");
        Assert.Equal("ITAAir", CreateService().GetFeeDisplayTravelType());
    }

    [Fact]
    public void GetFeeDisplayTravelType_Other_Returns_Unknown()
    {
        _contextMock.SetupGet(c => c.TransportationType).Returns("OwnAir");
        Assert.Equal("unknown", CreateService().GetFeeDisplayTravelType());
    }

    [Fact]
    public void GetFeeDisplayRentalCar_Returns_Context_Value()
    {
        _contextMock.SetupGet(c => c.FeeDisplayRentalCar).Returns(true);
        Assert.True(CreateService().GetFeeDisplayRentalCar());
    }

    [Fact]
    public void GetFeeDisplayExtending_Returns_Context_Value()
    {
        _contextMock.SetupGet(c => c.FeeDisplayExtending).Returns(false);
        Assert.False(CreateService().GetFeeDisplayExtending());
    }

    [Fact]
    public void GetFeeDisplayExtendingGroup_Returns_Context_Value()
    {
        _contextMock.SetupGet(c => c.FeeDisplayExtendingGroup).Returns(true);
        Assert.True(CreateService().GetFeeDisplayExtendingGroup());
    }

    [Fact]
    public void GetFeeDisplayAccomAssistance_Returns_True_When_NonGroupHotel_Exists()
    {
        _contextMock.SetupGet(c => c.NonGroupHotelCount).Returns(2);
        Assert.True(CreateService().GetFeeDisplayAccomAssistance());
    }

    [Fact]
    public void GetFeeDisplayAccomAssistance_Returns_False_When_No_NonGroupHotel()
    {
        _contextMock.SetupGet(c => c.NonGroupHotelCount).Returns(0);
        Assert.False(CreateService().GetFeeDisplayAccomAssistance());
    }
}
