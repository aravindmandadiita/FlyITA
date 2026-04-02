using FlyITA.Core.Models;

namespace FlyITA.Core.Tests.Models;

public class ValidationResultTests
{
    [Fact]
    public void NoErrors_IsValid()
    {
        var result = new ValidationResult();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void AddError_MakesInvalid()
    {
        var result = new ValidationResult();
        result.AddError("Something went wrong");
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Something went wrong", result.Errors[0]);
    }

    [Fact]
    public void AddWarning_StillValid()
    {
        var result = new ValidationResult();
        result.AddWarning("Minor issue");
        Assert.True(result.IsValid);
        Assert.Single(result.Warnings);
    }

    [Fact]
    public void Multiple_Errors_Tracked()
    {
        var result = new ValidationResult();
        result.AddError("Error 1");
        result.AddError("Error 2");
        Assert.Equal(2, result.Errors.Count);
        Assert.False(result.IsValid);
    }
}
