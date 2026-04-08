using Moq;
using Microsoft.Extensions.Configuration;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class EmailServiceTests
{
    private readonly Mock<IContextManager> _contextMock = new();
    private readonly Mock<IPCentralDataAccess> _dataMock = new();
    private readonly Mock<ISmtpClient> _smtpMock = new();
    private readonly Mock<IEmailTemplateLoader> _templateMock = new();

    private EmailService CreateService(Dictionary<string, string?>? configPairs = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configPairs ?? new())
            .Build();
        return new EmailService(_contextMock.Object, _dataMock.Object, config, _smtpMock.Object, _templateMock.Object);
    }

    // Standard email tests

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
        _templateMock.Setup(t => t.LoadTemplateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((string?)null);

        var result = await CreateService().SendRegistrationConfirmationAsync(1);
        Assert.False(result.IsValid);
        Assert.Contains("template not found", result.Errors[0]);
    }

    [Fact]
    public async Task PreviewLogonCredentialsAsync_Returns_Html()
    {
        _contextMock.SetupGet(c => c.ProgramID).Returns(1);
        _dataMock.Setup(d => d.GetParticipantByIdAsync(1)).ReturnsAsync(new Dictionary<string, object?> { ["LegalFirstName"] = "Test", ["LegalLastName"] = "User" });
        _templateMock.Setup(t => t.LoadTemplateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("<p>Hello <<LegalFirstName>></p>");
        _dataMock.Setup(d => d.GetProgramByIdAsync(1)).ReturnsAsync(new Dictionary<string, object?>());

        var html = await CreateService().PreviewLogonCredentialsAsync(1);
        Assert.Contains("<p>Hello Test</p>", html);
    }

    // Vacation email tests

    [Fact]
    public async Task SendVacationRequest_Success_ReplacesTokensAndSends()
    {
        _templateMock.Setup(t => t.LoadTemplateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Name: [NameofPersonRequesting], Phone: [PhoneNumber], City: [DepartureCity]");

        var data = new VacationEmailData
        {
            ToEmail = "to@test.com",
            FromEmail = "from@test.com",
            Subject = "Vacation Request",
            NameOfPersonRequesting = "Alice",
            PhoneNumber = "555-1234",
            DepartureCity = "Chicago"
        };

        var result = await CreateService().SendVacationRequestEmailAsync(data);

        Assert.True(result.IsValid);
        _smtpMock.Verify(s => s.SendAsync(
            It.Is<System.Net.Mail.MailMessage>(m =>
                m.Body.Contains("Alice") &&
                m.Body.Contains("555-1234") &&
                m.Body.Contains("Chicago")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendVacationRequest_ProcessesPassengerBlock()
    {
        _templateMock.Setup(t => t.LoadTemplateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Before<passengerinfoi><<pfirstname>> <<plastname>></passengerinfoi>After");

        var data = new VacationEmailData
        {
            ToEmail = "to@test.com",
            FromEmail = "from@test.com",
            Subject = "Test",
            Passengers = new List<PassengerData>
            {
                new() { FirstName = "Bob", LastName = "Smith" },
                new() { FirstName = "Eve", LastName = "Jones" }
            }
        };

        var result = await CreateService().SendVacationRequestEmailAsync(data);

        Assert.True(result.IsValid);
        _smtpMock.Verify(s => s.SendAsync(
            It.Is<System.Net.Mail.MailMessage>(m =>
                m.Body.Contains("Bob") && m.Body.Contains("Eve") &&
                !m.Body.Contains("<passengerinfoi>")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendVacationRequest_Fails_When_No_Template()
    {
        _templateMock.Setup(t => t.LoadTemplateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((string?)null);

        var data = new VacationEmailData { ToEmail = "to@test.com", FromEmail = "from@test.com" };
        var result = await CreateService().SendVacationRequestEmailAsync(data);

        Assert.False(result.IsValid);
        Assert.Contains("template not found", result.Errors[0]);
    }

    [Fact]
    public async Task SendVacationRequest_Fails_When_NoRouting()
    {
        _templateMock.Setup(t => t.LoadTemplateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("<html></html>");

        var data = new VacationEmailData { ToEmail = "", FromEmail = "" };
        var result = await CreateService().SendVacationRequestEmailAsync(data);

        Assert.False(result.IsValid);
        Assert.Contains("routing not configured", result.Errors[0]);
    }

    // Traveler profile form email tests

    [Fact]
    public async Task SendTravelerProfileForm_Success_ReplacesTokensAndSends()
    {
        _templateMock.Setup(t => t.LoadTemplateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Name: [TravelerFirstName] [TravelerLastName], Co: [CompanyName]");

        var data = new TravelerProfileEmailData
        {
            ToEmail = "to@test.com",
            FromEmail = "from@test.com",
            Subject = "Profile",
            TravelerFirstName = "John",
            TravelerLastName = "Doe",
            CompanyName = "Acme"
        };

        var result = await CreateService().SendTravelerProfileFormEmailAsync(data);

        Assert.True(result.IsValid);
        _smtpMock.Verify(s => s.SendAsync(
            It.Is<System.Net.Mail.MailMessage>(m =>
                m.Body.Contains("John") &&
                m.Body.Contains("Doe") &&
                m.Body.Contains("Acme")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendTravelerProfileForm_ProcessesFrequentFlyerBlock()
    {
        _templateMock.Setup(t => t.LoadTemplateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("<frequentflyerti><<frequentflyernumber>>:<<frequentflyernumberid>></frequentflyerti>");

        var data = new TravelerProfileEmailData
        {
            ToEmail = "to@test.com",
            FromEmail = "from@test.com",
            Subject = "Test",
            FrequentFlyers = new List<FrequentFlyerData>
            {
                new() { Airline = "Delta", Number = "123456" }
            }
        };

        var result = await CreateService().SendTravelerProfileFormEmailAsync(data);

        Assert.True(result.IsValid);
        _smtpMock.Verify(s => s.SendAsync(
            It.Is<System.Net.Mail.MailMessage>(m =>
                m.Body.Contains("Delta") && m.Body.Contains("123456") &&
                !m.Body.Contains("<frequentflyerti>")),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
