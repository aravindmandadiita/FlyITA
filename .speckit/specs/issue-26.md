# Spec: ACH Payment — Wire CardProcessService via Sidecar

**Issue:** #26  
**Epic:** #25 — Post-Migration: Integration Wiring & DevOps  
**Status:** CLEAN

---

## Problem Statement

The modern `AchPayment.cshtml.cs` page collects ACH payment details (bank name, routing number, account number, amount) and validates them, but the actual payment submission is a TODO at line 44. The existing `WcfCardProcessService` in the Infrastructure layer is a placeholder that throws `NotSupportedException` — it was designed as a direct WCF caller, which contradicts the sidecar architecture.

## Architecture Decision

Per the Legacy API Sidecar pattern: **all DLL/WCF calls stay in `FlyITA.Legacy.Api` (net48)**. The modern .NET 10 app calls the sidecar over HTTP. `PCentralLib.dll` already contains the WCF proxy class for `CardProcessService.ICardProcessService` (contract visible in `src/FlyITA/Web.config` line 148). No new SOAP proxy generation is needed.

## Current State

| Component | File | Status |
|-----------|------|--------|
| ACH Payment UI | `src/FlyITA.Web/Pages/AchPayment.cshtml` | Done — full form with validation |
| ACH Payment Page Model | `src/FlyITA.Web/Pages/AchPayment.cshtml.cs` | TODO at line 44 — no service call |
| ICardProcessService | `src/FlyITA.Core/Interfaces/ICardProcessService.cs` | Done — `ProcessPaymentAsync`, `RefundAsync` |
| PaymentRequest | `src/FlyITA.Core/Models/PaymentRequest.cs` | Done — Token, Amount, Currency, Description, ParticipantId |
| PaymentResult | `src/FlyITA.Core/Models/PaymentResult.cs` | Done — Success, TransactionId, ErrorMessage, ResponseCode |
| WcfCardProcessService | `src/FlyITA.Infrastructure/Services/WcfCardProcessService.cs` | Placeholder — throws NotSupportedException |
| DI registration | `src/FlyITA.Infrastructure/DependencyInjection.cs` lines 41-46 | Registers WcfCardProcessService directly |
| Legacy.Api PaymentsController | — | Does not exist |
| Legacy.Api Web.config | `src/FlyITA.Legacy.Api/Web.config` | No WCF binding config |

## Target State

### Call Flow

```
AchPayment.cshtml.cs (modern .NET 10)
  → ICardProcessService.ProcessPaymentAsync()
    → LegacyApiCardProcessService (new, replaces WcfCardProcessService)
      → HTTP POST to Legacy.Api sidecar: /api/payments/ach
        → PaymentsController (new, in Legacy.Api net48)
          → PCentralLib CardProcessService proxy
            → WCF SOAP to https://www.itavault.int/CardProcessService.svc
```

### Changes Required

#### 1. Legacy.Api Sidecar (net48)

**New file: `PaymentsController.cs`** in `src/FlyITA.Legacy.Api/Controllers/`

- Route: `POST api/payments/ach`
- Accepts JSON body with ACH payment fields (bank name, routing number, account number, account holder, account type, amount, participant ID)
- Uses PCentralLib's `CardProcessService.ICardProcessService` WCF proxy to submit payment
- WCF client configured via `<system.serviceModel>` in `Web.config` (config-based, matches legacy pattern)
- Binding: `WSHttpBinding`, Transport security, no client credentials (matches `src/FlyITA/Web.config` lines 138-148)
- Endpoint address in config defaults to dev (`int.itavault.int`); overridden per-environment on IIS server (same deployment pattern as connection strings)
- Controller creates proxy via named endpoint: `new CardProcessServiceClient("WSHttpBinding_ICardProcessService")`
- Returns JSON: `{ success, transactionId, errorMessage, responseCode }`
- Follow existing controller patterns (see `ParticipantsController.cs`)

**Updated: `Web.config`** in `src/FlyITA.Legacy.Api/`

- Add `<system.serviceModel>` section with WSHttpBinding config for `ICardProcessService`
- Endpoint: environment-specific (CDT → `int.itavault.int`, PRD → `www.itavault.int`)
- Security mode: Transport, clientCredentialType: None

#### 2. Modern App — Infrastructure Layer

**Replace: `WcfCardProcessService.cs`** → **`LegacyApiCardProcessService.cs`**

- Implements `ICardProcessService`
- Uses `HttpClient` (via HttpClientFactory) to call the sidecar's `POST /api/payments/ach`
- Maps between `PaymentRequest`/`PaymentResult` (Core models) and sidecar JSON
- Async with CancellationToken throughout
- Structured logging on success, failure, and timeout
- No direct WCF — all WCF stays in the sidecar

**Updated: `DependencyInjection.cs`**

- Replace `WcfCardProcessService` registration (lines 41-46) with `LegacyApiCardProcessService` using `AddHttpClient<>` pattern
- Use `LegacyApiOptions` for base URL and timeout (already configured at `appsettings.json` lines 113-116)
- Add Polly retry + circuit breaker policies for resilience

#### 3. Modern App — Web Layer

**Updated: `AchPayment.cshtml.cs`**

- Inject `ICardProcessService` via constructor
- Make `OnPost` async (`OnPostAsync`)
- At line 44 TODO: build `PaymentRequest` from form fields and call `ProcessPaymentAsync`
- Handle success → `SuccessMessage` with transaction ID
- Handle failure → `ErrorMessage` with service error

#### 4. Configuration

**Updated: `ExternalServicesOptions.cs`** — Remove only `CardProcessServiceUrl` property. The class itself and its other properties (`CardTokenServiceUrl`, `PerformanceCentralServiceUrl`, `ServiceTimeoutSeconds`) remain — they are still used by `WcfCardTokenService` and `WcfPerformanceCentralClient`.

**Updated: `appsettings.json`** — Remove only `CardProcessServiceUrl` from the `ExternalServices` section. The sidecar URL is already in `LegacyApi.BaseUrl`.

#### 5. PaymentRequest Model Update

Add ACH-specific fields to the existing `PaymentRequest` model:

- `BankName` (string) — bank name
- `RoutingNumber` (string) — 9-digit ABA routing number
- `AccountNumber` (string) — bank account number
- `AccountHolderName` (string) — name on account
- `AccountType` (string) — "Checking" or "Savings"

Existing fields (`Token`, `Amount`, `Currency`, `Description`, `ParticipantId`) remain unchanged. Card payments use `Token`; ACH payments use the bank fields. The `ICardProcessService` interface stays the same — `ProcessPaymentAsync(PaymentRequest)` handles both.

#### 6. RefundAsync Implementation

`LegacyApiCardProcessService.RefundAsync` must implement the interface but refund is not wired in any UI. Implementation: call `POST /api/payments/refund` on the sidecar. The sidecar's `PaymentsController` exposes the endpoint but returns HTTP 501 (Not Implemented) until #29 (DLL Runtime Testing) verifies the PCentralLib refund method. This keeps the contract honest without faking success.

#### 7. Existing Test Replacement

Delete `WcfCardProcessServiceTests.cs` (tests a class being removed). Replace with:

- `LegacyApiCardProcessServiceTests.cs` — mock HttpClient, verify correct URL/payload/headers, verify success/failure mapping, verify timeout handling
- Update any integration tests that reference `WcfCardProcessService`

## Acceptance Criteria

1. `POST /api/payments/ach` endpoint exists in Legacy.Api sidecar
2. Sidecar endpoint calls PCentralLib's CardProcessService WCF proxy with correct binding config
3. Modern `AchPayment.cshtml.cs` calls `ICardProcessService.ProcessPaymentAsync()` via HTTP to sidecar
4. `WcfCardProcessService` is removed; replaced by `LegacyApiCardProcessService` (HTTP client)
5. DI registration uses `AddHttpClient<>` with Polly resilience policies
6. Success shows transaction ID to user; failure shows error message
7. All existing tests pass; new unit tests for `LegacyApiCardProcessService` and `PaymentsController`
8. Integration tests verify the HTTP client ↔ sidecar contract
9. No direct WCF calls from the modern .NET 10 app
10. Build succeeds with zero warnings

## Out of Scope

- Runtime testing against live `itavault.int` (blocked on dev environment)
- Card tokenization flow (separate from ACH — that's `ICardTokenService`)
- Refund flow (exists in interface but not wired in any UI currently)
- Other payment methods (credit card, etc.)

## Risks

- **PCentralLib method signatures unknown** — the DLL is a black box. The PaymentsController may need adjustment once we can inspect the actual proxy class at runtime. Mitigation: code the controller to compile and log clearly; verify in #29 (DLL Runtime Testing).
- **WSHttpBinding vs BasicHttpBinding** — legacy uses `WSHttpBinding` but the sidecar runs on net48 which supports both natively. Must match the binding from `Web.config`.
