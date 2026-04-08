# Plan: ACH Payment — Wire CardProcessService via Sidecar

**Issue:** #26  
**Spec:** `.speckit/specs/issue-26.md`  
**Status:** CLEAN

---

## Implementation Order

Changes are ordered by dependency — each step builds on the previous and results in a compilable state.

### Phase A: Core Model Update (no dependencies)

**A1. Update `PaymentRequest` model**
- File: `src/FlyITA.Core/Models/PaymentRequest.cs`
- Add 5 ACH fields: `BankName`, `RoutingNumber`, `AccountNumber`, `AccountHolderName`, `AccountType`
- All `string`, default to `string.Empty`
- Existing fields unchanged
- No interface changes

### Phase B: Legacy.Api Sidecar (depends on nothing in modern app)

**B1. Update Legacy.Api `Web.config` with WCF binding**
- File: `src/FlyITA.Legacy.Api/Web.config`
- Add `<system.serviceModel>` section:
  - `<wsHttpBinding>` binding named `WSHttpBinding_ICardProcessService` with Transport security, no client credentials
  - `<client>` endpoint: address `https://int.itavault.int/CardProcessService.svc` (dev default), binding `wsHttpBinding`, contract `CardProcessService.ICardProcessService`, name `WSHttpBinding_ICardProcessService`
- Copy binding config verbatim from `src/FlyITA/Web.config` lines 138-148

**B2. Add `PaymentsController.cs`**
- File: `src/FlyITA.Legacy.Api/Controllers/PaymentsController.cs`
- Route prefix: `api/payments`
- `POST api/payments/ach`:
  - Accept `AchPaymentApiRequest` model (bank name, routing number, account number, account holder, account type, amount, participant ID, currency, description)
  - Create WCF proxy: `new CardProcessServiceClient("WSHttpBinding_ICardProcessService")`
  - Call the proxy method to process payment (best-effort based on known WCF contract pattern — PCentralLib is a black box)
  - Wrap in try/catch — on WCF `FaultException`, `CommunicationException`, `TimeoutException` return 502 with error details
  - On success return 200 with `{ success, transactionId, errorMessage, responseCode }`
  - Log all calls (entry, exit, errors) via `System.Diagnostics.Trace`
- `POST api/payments/refund`:
  - Accept `RefundApiRequest` model (transaction ID, amount)
  - Return HTTP 501 Not Implemented (placeholder until #29)
- Follow `ParticipantsController` patterns (route attributes, `IHttpActionResult` return types)

**B3. Add request/response models for sidecar API**
- File: `src/FlyITA.Legacy.Api/Controllers/PaymentsController.cs` (inner classes, same as `CustomFieldRequest` pattern)
- `AchPaymentApiRequest`: BankName, RoutingNumber, AccountNumber, AccountHolderName, AccountType, Amount (decimal), Currency, Description, ParticipantId
- `RefundApiRequest`: TransactionId, Amount
- `PaymentApiResponse`: Success (bool), TransactionId, ErrorMessage, ResponseCode

### Phase C: Modern App — Infrastructure (depends on A1)

**C1. Create `LegacyApiCardProcessService.cs`**
- File: `src/FlyITA.Infrastructure/Services/LegacyApiCardProcessService.cs`
- Implements `ICardProcessService`
- Constructor: `HttpClient` (injected via HttpClientFactory), `ILogger<LegacyApiCardProcessService>`
- `ProcessPaymentAsync`:
  - Build JSON payload from `PaymentRequest` fields
  - `POST` to `api/payments/ach` (relative to HttpClient base address)
  - Deserialize response to `PaymentResult` using `System.Text.Json`
  - On HTTP error: log and return `PaymentResult { Success = false, ErrorMessage = ... }`
  - On timeout/network error: log and return failure
  - Pass `CancellationToken` through to `HttpClient.PostAsync`
- `RefundAsync`:
  - `POST` to `api/payments/refund`
  - On 501 response: return `PaymentResult { Success = false, ErrorMessage = "Refund not yet available" }`
  - Pass `CancellationToken` through

**C2. Delete `WcfCardProcessService.cs`**
- File: `src/FlyITA.Infrastructure/Services/WcfCardProcessService.cs` — delete entirely

**C3. Update `DependencyInjection.cs`**
- File: `src/FlyITA.Infrastructure/DependencyInjection.cs`
- Remove lines 41-46 (WcfCardProcessService registration)
- Add `AddHttpClient<ICardProcessService, LegacyApiCardProcessService>` using existing `LegacyApiOptions` for base URL and timeout
- Add resilience policies via `Microsoft.Extensions.Http.Resilience` (.NET 10 / Polly v8):
  - Use `.AddStandardResilienceHandler()` which provides retry (exponential backoff), circuit breaker, and timeout out of the box
  - Override defaults if needed: 3 retry attempts, 30s circuit breaker duration
- Remove `using System.ServiceModel` if no longer needed

**C4. Update `ExternalServicesOptions.cs`**
- File: `src/FlyITA.Core/Options/ExternalServicesOptions.cs`
- Remove `CardProcessServiceUrl` property

**C5. Update `appsettings.json`**
- File: `src/FlyITA.Web/appsettings.json`
- Remove `"CardProcessServiceUrl": ""` from `ExternalServices` section

### Phase D: Modern App — Web Layer (depends on C1)

**D1. Update `AchPayment.cshtml.cs`**
- File: `src/FlyITA.Web/Pages/AchPayment.cshtml.cs`
- Add constructor with `ICardProcessService` injection
- Change `OnPost()` to `async Task<IActionResult> OnPostAsync()`
- At line 44 TODO: 
  - Build `PaymentRequest` from form properties (BankName, RoutingNumber, AccountNumber, AccountHolderName, AccountType, Amount)
  - Set `Currency = "USD"`, `Description = "ACH Payment"`
  - Call `await _cardProcessService.ProcessPaymentAsync(request)`
  - If `result.Success`: set `SuccessMessage = $"Payment submitted. Transaction ID: {result.TransactionId}"`
  - If `!result.Success`: set `ErrorMessage = result.ErrorMessage ?? "Payment failed. Please try again."`
  - Return `Page()`

### Phase E: Tests (alongside each phase, but grouped here for clarity)

**E1. Unit tests for `LegacyApiCardProcessService`**
- File: `tests/FlyITA.Infrastructure.Tests/Services/LegacyApiCardProcessServiceTests.cs`
- Tests:
  - ProcessPayment_Success — mock HttpClient returns 200 with valid JSON → returns PaymentResult.Success
  - ProcessPayment_ServerError — mock returns 500 → returns PaymentResult with error
  - ProcessPayment_Timeout — mock throws TaskCanceledException → returns PaymentResult with timeout message
  - ProcessPayment_InvalidJson — mock returns 200 with garbage → returns failure
  - Refund_NotImplemented — mock returns 501 → returns failure with "not yet available"
  - Verify correct URL path (`api/payments/ach`)
  - Verify JSON payload contains all ACH fields

**E2. Delete `WcfCardProcessServiceTests.cs`**
- File: `tests/FlyITA.Infrastructure.Tests/Services/WcfCardProcessServiceTests.cs` — delete entirely

**E3. Create integration test for AchPayment page**
- File: `tests/FlyITA.Web.Tests/Pages/AchPaymentIntegrationTests.cs` (CREATE — no integration test exists; `AchPaymentValidationTests.cs` only tests routing number)
- Test OnPostAsync with mocked `ICardProcessService`:
  - Valid submission → success message shown
  - Service failure → error message shown
  - Invalid routing number → model error (existing test in `AchPaymentValidationTests.cs`, verify still passes)

**E4. Update existing tests that reference AchPaymentModel**
- `tests/FlyITA.Web.Tests/Pages/FormPageTests.cs` — may need update if it creates `AchPaymentModel` directly (now requires constructor injection)
- `tests/FlyITA.E2E.Tests/FormTests.cs` — E2E tests hit the page via browser, should work without changes but verify
- Grep for `WcfCardProcessService` across all test files and update references

**E5. Legacy.Api testing approach**
- No separate test project for Legacy.Api (net48). The `PaymentsController` is tested indirectly:
  - Modern app integration tests mock the HTTP boundary (verify the contract)
  - Runtime testing of the actual PCentralLib WCF calls deferred to #29 (DLL Runtime Testing)

### Phase F: Cleanup

**F1. Remove System.ServiceModel package reference if unused**
- Check if `WcfCardTokenService` or `WcfPerformanceCentralClient` still use it
- If yes: keep. If no: remove from `FlyITA.Infrastructure.csproj`
- Expected: **keep** — the other two WCF services still reference it

**F2. Verify build + all tests pass**
- `dotnet build FlyITA.Modern.sln`
- `dotnet test FlyITA.Modern.sln`
- Zero warnings, zero failures

## File Change Summary

| Action | File |
|--------|------|
| MODIFY | `src/FlyITA.Core/Models/PaymentRequest.cs` |
| MODIFY | `src/FlyITA.Core/Options/ExternalServicesOptions.cs` |
| CREATE | `src/FlyITA.Infrastructure/Services/LegacyApiCardProcessService.cs` |
| DELETE | `src/FlyITA.Infrastructure/Services/WcfCardProcessService.cs` |
| MODIFY | `src/FlyITA.Infrastructure/DependencyInjection.cs` |
| MODIFY | `src/FlyITA.Web/Pages/AchPayment.cshtml.cs` |
| MODIFY | `src/FlyITA.Web/appsettings.json` |
| CREATE | `src/FlyITA.Legacy.Api/Controllers/PaymentsController.cs` |
| MODIFY | `src/FlyITA.Legacy.Api/Web.config` |
| CREATE | `tests/FlyITA.Infrastructure.Tests/Services/LegacyApiCardProcessServiceTests.cs` |
| DELETE | `tests/FlyITA.Infrastructure.Tests/Services/WcfCardProcessServiceTests.cs` |
| CREATE | `tests/FlyITA.Web.Tests/Pages/AchPaymentIntegrationTests.cs` |
| VERIFY | `tests/FlyITA.Web.Tests/Pages/FormPageTests.cs` (may need constructor update) |
| VERIFY | `tests/FlyITA.E2E.Tests/FormTests.cs` (should pass without changes) |

## Dependencies

```
A1 ──────────────────┐
                     ├── C1 → C2 → C3 → C4 → C5 → D1
B1 → B2 → B3        │
                     └── E1 → E2 → E3 → E4 → F1 → F2
```

A1 and B-chain can run in parallel. C-chain depends on A1. D depends on C1. E depends on C and D. E5 is documentation only (no test project for net48 sidecar).

## Resilience Configuration

```csharp
// .NET 10 standard resilience handler (Polly v8 under the hood)
.AddStandardResilienceHandler();
// Provides: retry (exponential backoff, 3 attempts), circuit breaker, total request timeout
// Defaults are sensible; override via .Configure() only if needed
```

Requires NuGet: `Microsoft.Extensions.Http.Resilience`

## Package Changes

| Package | Project | Action |
|---------|---------|--------|
| `Microsoft.Extensions.Http.Resilience` | `FlyITA.Infrastructure` | ADD |
