using FlyITA.Core.Enums;

namespace FlyITA.Core.Tests.Enums;

public class TransportationSectionTests
{
    [Fact]
    public void Values_Match_Expected_Integers()
    {
        Assert.Equal(0, (int)TransportationSection.NoSelection);
        Assert.Equal(1, (int)TransportationSection.ITAAir);
        Assert.Equal(2, (int)TransportationSection.OwnAir);
        Assert.Equal(3, (int)TransportationSection.Driving);
    }

    [Fact]
    public void Has_All_Expected_Members()
    {
        var values = Enum.GetValues<TransportationSection>();
        Assert.Equal(4, values.Length);
    }

    [Fact]
    public void AuthSystems_Values_Match()
    {
        Assert.Equal(0, (int)AuthSystems.CMS);
        Assert.Equal(1, (int)AuthSystems.WRA);
    }

    [Fact]
    public void TriBoolean_Values_Match()
    {
        Assert.Equal(0, (int)TriBoolean.Unassigned);
        Assert.Equal(1, (int)TriBoolean.True);
        Assert.Equal(2, (int)TriBoolean.False);
    }
}
