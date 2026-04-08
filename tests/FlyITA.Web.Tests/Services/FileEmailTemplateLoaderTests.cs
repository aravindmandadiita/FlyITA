using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Web.Services;

namespace FlyITA.Web.Tests.Services;

public class FileEmailTemplateLoaderTests : IDisposable
{
    private readonly string _tempDir;
    private readonly Mock<IWebHostEnvironment> _envMock = new();
    private readonly Mock<IPCentralDataAccess> _dataMock = new();
    private readonly Mock<IContextManager> _contextMock = new();
    private readonly Mock<ILogger<FileEmailTemplateLoader>> _loggerMock = new();

    public FileEmailTemplateLoaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"flyita_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(_tempDir, "Templates"));
        _envMock.SetupGet(e => e.ContentRootPath).Returns(_tempDir);
        _contextMock.SetupGet(c => c.ProgramID).Returns(1);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public async Task LoadTemplate_FileExists_ReturnsFileContent()
    {
        var templatePath = Path.Combine(_tempDir, "Templates", "test.html");
        await File.WriteAllTextAsync(templatePath, "<html>Hello</html>");

        var loader = new FileEmailTemplateLoader(_envMock.Object, _dataMock.Object, _contextMock.Object, _loggerMock.Object);

        var result = await loader.LoadTemplateAsync("test.html");

        Assert.Equal("<html>Hello</html>", result);
        _dataMock.Verify(d => d.GetEmailTemplateAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task LoadTemplate_FileMissing_FallsBackToSidecar()
    {
        _dataMock.Setup(d => d.GetEmailTemplateAsync("missing.html", 1))
            .ReturnsAsync("<html>From sidecar</html>");

        var loader = new FileEmailTemplateLoader(_envMock.Object, _dataMock.Object, _contextMock.Object, _loggerMock.Object);

        var result = await loader.LoadTemplateAsync("missing.html");

        Assert.Equal("<html>From sidecar</html>", result);
        _dataMock.Verify(d => d.GetEmailTemplateAsync("missing.html", 1), Times.Once);
    }

    [Fact]
    public async Task LoadTemplate_BothMissing_ReturnsNull()
    {
        _dataMock.Setup(d => d.GetEmailTemplateAsync("nowhere.html", 1))
            .ReturnsAsync((string?)null);

        var loader = new FileEmailTemplateLoader(_envMock.Object, _dataMock.Object, _contextMock.Object, _loggerMock.Object);

        var result = await loader.LoadTemplateAsync("nowhere.html");

        Assert.Null(result);
    }
}
