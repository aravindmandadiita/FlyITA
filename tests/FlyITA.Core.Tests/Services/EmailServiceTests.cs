using Moq;
using Microsoft.Extensions.Configuration;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class EmailServiceTests
{
    private readonly Mock<IContextManager> _contextMock = new();
    private readonly Mock<IPCentralDataAccess> _dataMock = new();
    private readonly Mock<ISmtpClient> _smtpMock = new();

    private EmailService CreateService(Dictionary<string, string?>? configPairs = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configPairs ?? new())
            .Build();
        return new EmailService(_contextMock.Object, _dataMock.Object, config, _smtpMock.Object);
    }

    [Fact]
    public async Task ReplacePlaceholdersAsync_Replaces_Name()
    {
        var service = CreateService();
        _contextMock.SetupGet(c => c.ProgramID).Returns(1);
        _dataMock.Setup(d => d.GetProgramByIdAsync(1)).ReturnsAsync(new Dictionary<string, object?>());

        var participant = new Dictionary<string, object?>
        {
            ["LegalFirstName"] = "John",
            ["LegalLastName"] = "Doe",
            ["EmailAddress"] = "john@test.com"
        };

        var body = "Hello <<LegalFirstName>> <<LegalLastName>>, email: <<EmailAddress>>";
        var result = await service.ReplacePlaceholdersAsync(body, participant);

        Assert.Equal("Hello John Doe, email: john@test.com", result);
    }

    [Fact]
    public async Task ReplacePlaceholdersAsync_Replaces_Participant_Name_Token()
    {
        var service = CreateService();
        _contextMock.SetupGet(c => c.ProgramID).Returns(1);
        _dataMock.Setup(d => d.GetProgramByIdAsync(1)).ReturnsAsync(new Dictionary<string, object?>());

        var participant = new Dictionary<string, object?>
        {
            ["LegalFirstName"] = "Jane",
            ["LegalLastName"] = "Smith"
        };

        var result = await service.ReplacePlaceholdersAsync("Dear [PARTICIPANT_NAME]", participant);
        Assert.Equal("Dear Jane Smith", result);
    }

    [Fact]
    public async Task SendRegistrationConfirmation_Fails_When_No_Participant()
    {
        _dataMock.Setup(d => d.GetParticipantByIdAsync(999)).ReturnsAsync((Dictionary<string, object?>?)null);

        var result = await CreateService().SendRegistrationConfirmationAsync(999);
        Assert.False(result.IsValid);
        Assert.Contains("Participant not found", result.Errors[0]);
    }

    [Fact]
    public async Task SendRegistrationConfirmation_Fails_When_No_Template()
    {
        _dataMock.Setup(d => d.GetParticipantByIdAsync(1)).ReturnsAsync(new Dictionary<string, object?> { ["EmailAddress"] = "test@test.com" });
        _dataMock.Setup(d => d.GetEmailTemplateAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((string?)null);

        var result = await CreateService().SendRegistrationConfirmationAsync(1);
        Assert.False(result.IsValid);
        Assert.Contains("template not found", result.Errors[0]);
    }

    [Fact]
    public async Task PreviewLogonCredentialsAsync_Returns_Html()
    {
        _contextMock.SetupGet(c => c.ProgramID).Returns(1);
        _dataMock.Setup(d => d.GetParticipantByIdAsync(1)).ReturnsAsync(new Dictionary<string, object?> { ["LegalFirstName"] = "Test", ["LegalLastName"] = "User" });
        _dataMock.Setup(d => d.GetEmailTemplateAsync(It.IsAny<string>(), 1)).ReturnsAsync("<p>Hello <<LegalFirstName>></p>");
        _dataMock.Setup(d => d.GetProgramByIdAsync(1)).ReturnsAsync(new Dictionary<string, object?>());

        var html = await CreateService().PreviewLogonCredentialsAsync(1);
        Assert.Contains("<p>Hello Test</p>", html);
    }
}
