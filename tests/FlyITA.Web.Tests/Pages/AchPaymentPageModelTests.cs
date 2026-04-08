using Moq;
using Xunit;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using FlyITA.Web.Pages;

namespace FlyITA.Web.Tests.Pages;

public class AchPaymentPageModelTests
{
    [Fact]
    public async Task OnPostAsync_ValidPayment_ReturnsSuccessMessage()
    {
        var mockService = new Mock<ICardProcessService>();
        mockService
            .Setup(s => s.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult
            {
                Success = true,
                TransactionId = "TXN-99999",
                ResponseCode = "OK"
            });

        var model = CreatePageModel(mockService.Object);
        model.BankName = "Test Bank";
        model.RoutingNumber = "021000021"; // Valid ABA checksum
        model.AccountNumber = "123456789";
        model.ConfirmAccountNumber = "123456789";
        model.AccountHolderName = "John Doe";
        model.AccountType = "Checking";
        model.Amount = 100.00m;

        await model.OnPostAsync();

        Assert.NotNull(model.SuccessMessage);
        Assert.Contains("TXN-99999", model.SuccessMessage);
        Assert.Null(model.ErrorMessage);
        mockService.Verify(s => s.ProcessPaymentAsync(
            It.Is<PaymentRequest>(r =>
                r.BankName == "Test Bank" &&
                r.RoutingNumber == "021000021" &&
                r.Amount == 100.00m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_ServiceFailure_ReturnsErrorMessage()
    {
        var mockService = new Mock<ICardProcessService>();
        mockService
            .Setup(s => s.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult
            {
                Success = false,
                ErrorMessage = "Payment gateway unavailable"
            });

        var model = CreatePageModel(mockService.Object);
        model.BankName = "Test Bank";
        model.RoutingNumber = "021000021";
        model.AccountNumber = "123456789";
        model.ConfirmAccountNumber = "123456789";
        model.AccountHolderName = "John Doe";
        model.AccountType = "Checking";
        model.Amount = 50.00m;

        await model.OnPostAsync();

        Assert.NotNull(model.ErrorMessage);
        Assert.Contains("Payment gateway unavailable", model.ErrorMessage);
        Assert.Null(model.SuccessMessage);
    }

    [Fact]
    public async Task OnPostAsync_InvalidRoutingNumber_ReturnsModelError()
    {
        var mockService = new Mock<ICardProcessService>();

        var model = CreatePageModel(mockService.Object);
        model.BankName = "Test Bank";
        model.RoutingNumber = "123456789"; // Invalid ABA checksum
        model.AccountNumber = "123456789";
        model.ConfirmAccountNumber = "123456789";
        model.AccountHolderName = "John Doe";
        model.AccountType = "Checking";
        model.Amount = 50.00m;

        await model.OnPostAsync();

        Assert.True(model.ModelState.ContainsKey(nameof(AchPaymentModel.RoutingNumber)));
        mockService.Verify(
            s => s.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_MismatchedAccountNumbers_ReturnsModelError()
    {
        var mockService = new Mock<ICardProcessService>();

        var model = CreatePageModel(mockService.Object);
        model.BankName = "Test Bank";
        model.RoutingNumber = "021000021";
        model.AccountNumber = "123456789";
        model.ConfirmAccountNumber = "987654321"; // Mismatch
        model.AccountHolderName = "John Doe";
        model.AccountType = "Checking";
        model.Amount = 50.00m;

        await model.OnPostAsync();

        Assert.True(model.ModelState.ContainsKey(nameof(AchPaymentModel.ConfirmAccountNumber)));
        mockService.Verify(
            s => s.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static AchPaymentModel CreatePageModel(ICardProcessService service)
    {
        var model = new AchPaymentModel(service);
        // Simulate valid ModelState (ASP.NET runtime does this automatically)
        model.ModelState.Clear();
        return model;
    }
}
