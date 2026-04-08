using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;

namespace FlyITA.Web.Pages;

public class AchPaymentModel : PageModel
{
    private readonly ICardProcessService _cardProcessService;

    public AchPaymentModel(ICardProcessService cardProcessService)
    {
        _cardProcessService = cardProcessService;
    }

    [BindProperty, Required] public string BankName { get; set; } = "";
    [BindProperty, Required] public string RoutingNumber { get; set; } = "";
    [BindProperty, Required] public string AccountNumber { get; set; } = "";
    [BindProperty, Required] public string ConfirmAccountNumber { get; set; } = "";
    [BindProperty, Required] public string AccountHolderName { get; set; } = "";
    [BindProperty, Required] public string AccountType { get; set; } = "Checking";
    [BindProperty, Required, Range(0.01, 999999.99)] public decimal Amount { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Validate routing number format (9 digits, passes ABA checksum)
        if (!IsValidRoutingNumber(RoutingNumber))
        {
            ModelState.AddModelError(nameof(RoutingNumber), "Please enter a valid 9-digit ABA routing number.");
            return Page();
        }

        // Validate account numbers match
        if (AccountNumber != ConfirmAccountNumber)
        {
            ModelState.AddModelError(nameof(ConfirmAccountNumber), "Account numbers do not match.");
            return Page();
        }

        var request = new PaymentRequest
        {
            BankName = BankName,
            RoutingNumber = RoutingNumber,
            AccountNumber = AccountNumber,
            AccountHolderName = AccountHolderName,
            AccountType = AccountType,
            Amount = Amount,
            Currency = "USD",
            Description = "ACH Payment"
        };

        var result = await _cardProcessService.ProcessPaymentAsync(request);

        if (result.Success)
            SuccessMessage = $"Payment submitted successfully. Transaction ID: {result.TransactionId}";
        else
            ErrorMessage = result.ErrorMessage ?? "Payment failed. Please try again.";

        return Page();
    }

    internal static bool IsValidRoutingNumber(string routingNumber)
    {
        if (string.IsNullOrWhiteSpace(routingNumber))
            return false;

        if (!Regex.IsMatch(routingNumber, @"^\d{9}$"))
            return false;

        // ABA routing number checksum: 3(d1 + d4 + d7) + 7(d2 + d5 + d8) + (d3 + d6 + d9) mod 10 == 0
        var digits = routingNumber.Select(c => c - '0').ToArray();
        var checksum = 3 * (digits[0] + digits[3] + digits[6])
                     + 7 * (digits[1] + digits[4] + digits[7])
                     + (digits[2] + digits[5] + digits[8]);
        return checksum % 10 == 0;
    }
}
