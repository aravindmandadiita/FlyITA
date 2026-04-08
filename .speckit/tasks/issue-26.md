# Tasks: ACH Payment — Wire CardProcessService via Sidecar

**Issue:** #26  
**Plan:** `.speckit/plans/issue-26.md`  
**Status:** CLEAN

---

## Task 1: Update PaymentRequest model with ACH fields

**File:** `src/FlyITA.Core/Models/PaymentRequest.cs`

**Given** the existing `PaymentRequest` model with card-oriented fields  
**When** I add ACH-specific properties  
**Then** the model supports both card (Token) and ACH (bank details) payment types

**Changes:**
- Add `BankName` (string, default `string.Empty`)
- Add `RoutingNumber` (string, default `string.Empty`)
- Add `AccountNumber` (string, default `string.Empty`)
- Add `AccountHolderName` (string, default `string.Empty`)
- Add `AccountType` (string, default `string.Empty`)

**Verification:** `dotnet build src/FlyITA.Core/`

---

## Task 2: Add WCF binding config to Legacy.Api Web.config

**File:** `src/FlyITA.Legacy.Api/Web.config`

**Given** the Legacy.Api has no WCF client configuration  
**When** I add `<system.serviceModel>` section  
**Then** the sidecar can create WCF proxies for CardProcessService

**Changes:**
- Add `<system.serviceModel>` with:
  - `<wsHttpBinding>` binding `WSHttpBinding_ICardProcessService`: Transport security, no client credentials
  - `<client>` endpoint: address `https://int.itavault.int/CardProcessService.svc`, contract `CardProcessService.ICardProcessService`
- Copy binding config from `src/FlyITA/Web.config` lines 138-148

**Verification:** Legacy.Api project builds successfully

---

## Task 3: Add PaymentsController to Legacy.Api sidecar

**File:** `src/FlyITA.Legacy.Api/Controllers/PaymentsController.cs` (CREATE)

**Given** the Legacy.Api wraps PCentralLib for other entities  
**When** I add a PaymentsController with ACH endpoint  
**Then** the modern app can submit payments via HTTP to the sidecar

**Changes:**
- Route prefix: `api/payments`
- `POST api/payments/ach`:
  - Accept `AchPaymentApiRequest` (inner class): BankName, RoutingNumber, AccountNumber, AccountHolderName, AccountType, Amount, Currency, Description, ParticipantId
  - Add `using CardProcessService;` (namespace from PCentralLib.dll, per Web.config contract)
  - Create WCF proxy: `new CardProcessServiceClient("WSHttpBinding_ICardProcessService")`
  - Call proxy to process payment (best-effort — PCentralLib is black box, method signature TBD at runtime in #29)
  - If `CardProcessServiceClient` doesn't compile: check PCentralLib's exported types via Object Browser or `ildasm`, adjust class name accordingly. Document the actual class name in a code comment for future reference.
  - Catch `FaultException`, `CommunicationException`, `TimeoutException` → return 502 with error JSON
  - On success → return 200 with `PaymentApiResponse` (inner class): Success, TransactionId, ErrorMessage, ResponseCode
  - Log entry/exit/errors via `System.Diagnostics.Trace`
- `POST api/payments/refund`:
  - Accept `RefundApiRequest` (inner class): TransactionId, Amount
  - Return HTTP 501 Not Implemented (placeholder until #29)
- Follow `ParticipantsController` patterns

**Verification:** Legacy.Api project builds successfully

---

## Task 4: Create LegacyApiCardProcessService

**File:** `src/FlyITA.Infrastructure/Services/LegacyApiCardProcessService.cs` (CREATE)

**Given** the modern app needs to call the sidecar over HTTP  
**When** I create an `ICardProcessService` implementation using `HttpClient`  
**Then** payment requests are forwarded to the Legacy.Api sidecar

**Changes:**
- Implements `ICardProcessService`
- Constructor: `HttpClient http`, `ILogger<LegacyApiCardProcessService> logger`
- `ProcessPaymentAsync(PaymentRequest, CancellationToken)`:
  - Serialize `PaymentRequest` to JSON (`System.Text.Json`)
  - `POST` to `api/payments/ach` (relative URL)
  - On 200: deserialize response to `PaymentResult`
  - On HTTP error (4xx/5xx): log, return `PaymentResult { Success = false, ErrorMessage = statusCode + body }`
  - On exception (timeout, network): log, return `PaymentResult { Success = false, ErrorMessage = ex.Message }`
  - Pass `CancellationToken` to all async calls
- `RefundAsync(string, decimal, CancellationToken)`:
  - `POST` to `api/payments/refund`
  - On 501: return `PaymentResult { Success = false, ErrorMessage = "Refund not yet available" }`
  - Same error handling pattern

**Verification:** `dotnet build src/FlyITA.Infrastructure/`

---

## Task 5: Add unit tests for LegacyApiCardProcessService

**File:** `tests/FlyITA.Infrastructure.Tests/Services/LegacyApiCardProcessServiceTests.cs` (CREATE)

**Given** `LegacyApiCardProcessService` calls HttpClient  
**When** I mock HttpClient responses  
**Then** I verify correct URL, payload, and response mapping

**Tests:**
1. `ProcessPayment_Success` — mock returns 200 with valid PaymentResult JSON → returns `Success = true` with TransactionId
2. `ProcessPayment_ServerError` — mock returns 500 → returns `Success = false` with error message
3. `ProcessPayment_Timeout` — mock throws `TaskCanceledException` → returns `Success = false` with timeout message
4. `ProcessPayment_InvalidJson` — mock returns 200 with malformed body → returns `Success = false`
5. `ProcessPayment_SendsCorrectPayload` — verify JSON body contains all ACH fields (BankName, RoutingNumber, etc.)
6. `ProcessPayment_SendsToCorrectUrl` — verify request URL is `api/payments/ach`
7. `Refund_NotImplemented` — mock returns 501 → returns `Success = false`, "not yet available"
8. `Refund_SendsToCorrectUrl` — verify request URL is `api/payments/refund`

**Verification:** `dotnet test tests/FlyITA.Infrastructure.Tests/`

---

## Task 6: Delete WcfCardProcessService and its tests

**Files:**
- DELETE `src/FlyITA.Infrastructure/Services/WcfCardProcessService.cs`
- DELETE `tests/FlyITA.Infrastructure.Tests/Services/WcfCardProcessServiceTests.cs`

**Given** `WcfCardProcessService` is replaced by `LegacyApiCardProcessService`  
**When** I delete the old class and tests  
**Then** no dead code remains

**Verification:** `dotnet build FlyITA.Modern.sln` — no compile errors from missing references

---

## Task 7: Update DI registration and add resilience

**File:** `src/FlyITA.Infrastructure/DependencyInjection.cs`

**Given** the DI currently registers `WcfCardProcessService`  
**When** I replace it with `LegacyApiCardProcessService` via `AddHttpClient<>`  
**Then** payments route through the sidecar with resilience policies

**Changes:**
- Remove lines 41-46 (`WcfCardProcessService` registration)
- Add:
  ```csharp
  services.AddHttpClient<ICardProcessService, LegacyApiCardProcessService>((sp, client) =>
  {
      var opts = sp.GetRequiredService<IOptions<LegacyApiOptions>>().Value;
      client.BaseAddress = new Uri(opts.BaseUrl);
      client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
  })
  .AddStandardResilienceHandler();
  ```
- Add NuGet package: `Microsoft.Extensions.Http.Resilience` to `FlyITA.Infrastructure.csproj`

**Verification:** `dotnet build src/FlyITA.Infrastructure/`

---

## Task 8: Remove CardProcessServiceUrl from config

**Files:**
- `src/FlyITA.Core/Options/ExternalServicesOptions.cs` — remove `CardProcessServiceUrl` property
- `src/FlyITA.Web/appsettings.json` — remove `"CardProcessServiceUrl": ""` line from `ExternalServices`

**Given** CardProcessService now routes through the sidecar (LegacyApi.BaseUrl)  
**When** I remove the unused config property  
**Then** no orphaned configuration remains

**Verification:** `dotnet build FlyITA.Modern.sln`

---

## Task 9: Wire AchPayment page to ICardProcessService

**File:** `src/FlyITA.Web/Pages/AchPayment.cshtml.cs`

**Given** the page has a TODO at line 44  
**When** I inject `ICardProcessService` and call `ProcessPaymentAsync`  
**Then** ACH payments are submitted through the sidecar pipeline

**Changes:**
- Add private field `private readonly ICardProcessService _cardProcessService;`
- Add constructor: `public AchPaymentModel(ICardProcessService cardProcessService)`
- Change `OnPost()` → `async Task<IActionResult> OnPostAsync()`
- Replace TODO at line 44:
  ```csharp
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
  ```

**Verification:** `dotnet build src/FlyITA.Web/`

---

## Task 10: Create AchPayment integration tests

**File:** `tests/FlyITA.Web.Tests/Pages/AchPaymentIntegrationTests.cs` (CREATE)

**Given** AchPayment now calls `ICardProcessService`  
**When** I test with a mocked service  
**Then** I verify end-to-end page behavior

**Setup:** Use `WebApplicationFactory<Program>.WithWebHostBuilder` to override DI:
```csharp
factory.WithWebHostBuilder(builder =>
    builder.ConfigureTestServices(services =>
        services.AddSingleton<ICardProcessService>(mockService.Object)));
```

**Tests:**
1. `OnPostAsync_ValidPayment_ReturnsSuccessMessage` — mock service returns success → page shows "Payment submitted successfully"
2. `OnPostAsync_ServiceFailure_ReturnsErrorMessage` — mock service returns failure → page shows error
3. `OnPostAsync_InvalidRoutingNumber_ReturnsModelError` — bad routing number → model state error (no service call)
4. `OnPostAsync_MismatchedAccountNumbers_ReturnsModelError` — different account numbers → model state error (no service call)

**Verification:** `dotnet test tests/FlyITA.Web.Tests/`

---

## Task 11: Verify existing tests and full build

**Given** all changes are complete  
**When** I run the full test suite  
**Then** zero failures, zero warnings

**Steps:**
1. `dotnet build FlyITA.Modern.sln` — zero warnings
2. `dotnet test FlyITA.Modern.sln` — all tests pass
3. Verify `FormPageTests` still passes (GET /ach-payment returns 200)
4. Verify `AchPaymentValidationTests` still passes (routing number validation unchanged)
5. Grep for `WcfCardProcessService` — only in .speckit/ files, not in source code

---

## Task Order & Dependencies

```
Task 1 (model) ────────────────────┐
                                   ├── Task 4 (service) → Task 5 (tests)
Task 2 (web.config) → Task 3 (controller)              ↓
                                   ├── Task 6 (delete old) → Task 7 (DI) → Task 8 (config)
                                   │                                          ↓
                                   └──────────────────────── Task 9 (page) → Task 10 (integration tests)
                                                                              ↓
                                                                        Task 11 (verify all)
```
