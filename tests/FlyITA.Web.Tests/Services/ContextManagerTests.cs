using Microsoft.AspNetCore.Http;
using Moq;
using FlyITA.Core.Enums;
using FlyITA.Web.Services;

namespace FlyITA.Web.Tests.Services;

public class ContextManagerTests
{
    private static (ContextManager service, ISession session) CreateService()
    {
        var session = new TestSession();
        var httpContext = new DefaultHttpContext { Session = session };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.SetupGet(a => a.HttpContext).Returns(httpContext);
        return (new ContextManager(accessor.Object), session);
    }

    [Fact]
    public void PersonID_GetSet()
    {
        var (service, _) = CreateService();
        service.PersonID = 42;
        Assert.Equal(42, service.PersonID);
    }

    [Fact]
    public void ParticipantID_GetSet()
    {
        var (service, _) = CreateService();
        service.ParticipantID = 100;
        Assert.Equal(100, service.ParticipantID);
    }

    [Fact]
    public void TransportationType_GetSet()
    {
        var (service, _) = CreateService();
        service.TransportationType = "ITAAir";
        Assert.Equal("ITAAir", service.TransportationType);
    }

    [Fact]
    public void IsRegistered_DefaultFalse()
    {
        var (service, _) = CreateService();
        Assert.False(service.IsRegistered);
    }

    [Fact]
    public void TransportationSection_GetSet()
    {
        var (service, _) = CreateService();
        service.TransportationSection = TransportationSection.Driving;
        Assert.Equal(TransportationSection.Driving, service.TransportationSection);
    }

    [Fact]
    public void DateFormat_Returns_Expected()
    {
        var (service, _) = CreateService();
        Assert.Equal("MM/dd/yyyy", service.DateFormat);
    }

    [Fact]
    public void ToDebugString_Contains_Key_Info()
    {
        var (service, _) = CreateService();
        service.PersonID = 1;
        service.ParticipantID = 2;
        var debug = service.ToDebugString();
        Assert.Contains("PersonID=1", debug);
        Assert.Contains("ParticipantID=2", debug);
    }
}

/// <summary>Simple in-memory ISession for testing.</summary>
internal class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();
    public string Id => "test-session";
    public bool IsAvailable => true;
    public IEnumerable<string> Keys => _store.Keys;

    public void Clear() => _store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _store.Remove(key);
    public void Set(string key, byte[] value) => _store[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
}
