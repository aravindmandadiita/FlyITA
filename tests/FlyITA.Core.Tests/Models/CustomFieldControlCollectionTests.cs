using FlyITA.Core.Models;

namespace FlyITA.Core.Tests.Models;

public class CustomFieldControlCollectionTests
{
    private static CustomFieldControlModel CreateField(int id, string name) =>
        new() { CustomFieldID = id, CustomFieldName = name };

    [Fact]
    public void Add_And_Retrieve_ByName()
    {
        var collection = new CustomFieldControlCollection();
        collection.Add(CreateField(1, "DietaryPreference"));

        var result = collection["DietaryPreference"];
        Assert.NotNull(result);
        Assert.Equal(1, result!.CustomFieldID);
    }

    [Fact]
    public void Retrieve_ByName_CaseInsensitive()
    {
        var collection = new CustomFieldControlCollection();
        collection.Add(CreateField(1, "DietaryPreference"));

        var result = collection["dietarypreference"];
        Assert.NotNull(result);
    }

    [Fact]
    public void Add_And_Retrieve_ById()
    {
        var collection = new CustomFieldControlCollection();
        collection.Add(CreateField(42, "TestField"));

        var result = collection.ByID(42);
        Assert.NotNull(result);
        Assert.Equal("TestField", result!.CustomFieldName);
    }

    [Fact]
    public void ContainsField_ByName_True()
    {
        var collection = new CustomFieldControlCollection();
        collection.Add(CreateField(1, "MyField"));

        Assert.True(collection.ContainsField("MyField"));
    }

    [Fact]
    public void ContainsField_ByName_False()
    {
        var collection = new CustomFieldControlCollection();
        Assert.False(collection.ContainsField("NonExistent"));
    }

    [Fact]
    public void ContainsField_ById_True()
    {
        var collection = new CustomFieldControlCollection();
        collection.Add(CreateField(10, "Field"));

        Assert.True(collection.ContainsField(10));
    }

    [Fact]
    public void ContainsField_ById_False()
    {
        var collection = new CustomFieldControlCollection();
        Assert.False(collection.ContainsField(999));
    }

    [Fact]
    public void RemoveAt_Removes()
    {
        var collection = new CustomFieldControlCollection();
        collection.Add(CreateField(1, "First"));
        collection.Add(CreateField(2, "Second"));

        collection.RemoveAt(0);
        Assert.Single(collection);
        Assert.Equal("Second", collection[0].CustomFieldName);
    }
}
