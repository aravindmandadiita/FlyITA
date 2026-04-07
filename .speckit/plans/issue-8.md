# Plan: Phase 7 — Legacy API Sidecar & Dynamic Page Migration

**Spec:** [issue-8.md](../specs/issue-8.md)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## Implementation Strategy

Phase 7 is split into two sub-phases:
- **7A: Sidecar + Infrastructure Rewire** — Build Legacy.Api, rewire PCentralDataAccess
- **7B: Form Page Migration** — Migrate the 4 dynamic pages

7A must be complete before 7B starts (forms depend on working data access).

## Phase 7A: Sidecar + Infrastructure Rewire

### Step 1: Create FlyITA.Legacy.Api project scaffold

**What:** Create a new .NET Framework 4.8 ASP.NET Web API 2 project with its own solution file.

**Files:**
- `src/FlyITA.Legacy.Api/FlyITA.Legacy.Api.csproj` (net48 Web API)
- `src/FlyITA.Legacy.Api/FlyITA.Legacy.sln` (separate solution)
- `src/FlyITA.Legacy.Api/Global.asax` + `Global.asax.cs`
- `src/FlyITA.Legacy.Api/Web.config`
- `src/FlyITA.Legacy.Api/App_Start/WebApiConfig.cs`
- `src/FlyITA.Legacy.Api/Properties/launchSettings.json`
- `src/FlyITA.Legacy.Api/lib/` (gitignored, for DLL binaries)

**Key decisions:**
- Separate `.sln` because net48 and net10 can't share a solution cleanly
- Web API 2 (not MVC) — lightweight, JSON-only
- Port 5100 for local development
- `lib/` folder gitignored — DLLs deployed separately

### Step 2: Create Legacy.Api controllers (placeholder stubs)

**What:** Create controllers matching the REST contract from the spec. Each controller method is a placeholder that returns sample/empty data. Real DLL calls will be wired when DLLs are available in `lib/`.

**Files:**
- `src/FlyITA.Legacy.Api/Controllers/ParticipantsController.cs`
- `src/FlyITA.Legacy.Api/Controllers/PersonsController.cs`
- `src/FlyITA.Legacy.Api/Controllers/ProgramsController.cs`
- `src/FlyITA.Legacy.Api/Controllers/EmailController.cs`
- `src/FlyITA.Legacy.Api/Controllers/PageConfigController.cs`
- `src/FlyITA.Legacy.Api/Controllers/TransportationController.cs`
- `src/FlyITA.Legacy.Api/Controllers/HealthController.cs`

**Pattern:** Each action method has a `// TODO: Wire to PCentralLib.dll when available` comment and returns a placeholder response so the .NET 10 side can develop against a working API.

### Step 3: Add LegacyApiOptions to Core

**What:** Add Options class for Legacy.Api configuration.

**Files:**
- `src/FlyITA.Core/Options/LegacyApiOptions.cs` (NEW)

### Step 4: Rewire PCentralDataAccess to use HttpClient

**What:** Replace the current `IDatabaseAccess`-based implementation with `HttpClient` calls to Legacy.Api.

**Files:**
- `src/FlyITA.Infrastructure/Data/PCentralDataAccess.cs` (REWRITE)
- `src/FlyITA.Infrastructure/DependencyInjection.cs` (MODIFY — register named HttpClient)

**Key changes:**
- Constructor takes `HttpClient` instead of `IDatabaseAccess`
- Each method calls the corresponding Legacy.Api endpoint
- JSON deserialized to `Dictionary<string, object?>` for interface compatibility
- HttpClient configured with base URL from `LegacyApiOptions`

### Step 5: Update DI registration in Web

**What:** Register `LegacyApiOptions`, HttpClient factory for PCentralDataAccess.

**Files:**
- `src/FlyITA.Web/Program.cs` (MODIFY)
- `src/FlyITA.Web/appsettings.json` (MODIFY — add LegacyApi section)
- `src/FlyITA.Web/appsettings.Development.json` (MODIFY)

### Step 6: Update PCentralDataAccess tests

**What:** Rewrite tests to mock `HttpMessageHandler` instead of `IDatabaseAccess`.

**Files:**
- `tests/FlyITA.Infrastructure.Tests/Data/PCentralDataAccessTests.cs` (REWRITE)

## Phase 7B: Form Page Migration

### Step 7: GuestProfile page

**Files:**
- `src/FlyITA.Web/Pages/GuestProfile.cshtml` (NEW)
- `src/FlyITA.Web/Pages/GuestProfile.cshtml.cs` (NEW)
- `tests/FlyITA.Web.Tests/Pages/GuestProfileTests.cs` (NEW)

### Step 8: TravelerProfile page

**Files:**
- `src/FlyITA.Web/Pages/TravelerProfile.cshtml` (NEW)
- `src/FlyITA.Web/Pages/TravelerProfile.cshtml.cs` (NEW)
- `tests/FlyITA.Web.Tests/Pages/TravelerProfileTests.cs` (NEW)

### Step 9: VacationRequest page

**Files:**
- `src/FlyITA.Web/Pages/VacationRequest.cshtml` (NEW)
- `src/FlyITA.Web/Pages/VacationRequest.cshtml.cs` (NEW)
- `tests/FlyITA.Web.Tests/Pages/VacationRequestTests.cs` (NEW)

### Step 10: AchPayment page

**Files:**
- `src/FlyITA.Web/Pages/AchPayment.cshtml` (NEW)
- `src/FlyITA.Web/Pages/AchPayment.cshtml.cs` (NEW)
- `tests/FlyITA.Web.Tests/Pages/AchPaymentTests.cs` (NEW)

### Step 11: Legacy URL redirects for form pages

**Files:**
- `src/FlyITA.Web/Program.cs` (MODIFY — add redirects)

```csharp
app.MapGet("/GuestProfileInformation.aspx", () => Results.Redirect("/guest-profile", permanent: true));
app.MapGet("/Travelerprofileinformation.aspx", () => Results.Redirect("/traveler-profile", permanent: true));
app.MapGet("/VacationTravelRequest.aspx", () => Results.Redirect("/vacation-request", permanent: true));
app.MapGet("/AchPayment.aspx", () => Results.Redirect("/ach-payment", permanent: true));
```

## Build & Test Strategy

- Legacy.Api: Builds with its own solution (`dotnet build` or `msbuild` with net48 SDK)
- Main solution: `dotnet build FlyITA.sln` — unaffected by Legacy.Api
- Tests: `dotnet test` on main solution; Legacy.Api tested separately
- CI: Two build steps — one for main solution, one for Legacy.Api

## Risk Mitigation

- **No DLLs available?** Legacy.Api controllers return placeholder data. PCentralDataAccess tests mock HttpClient. Form pages use mocked IPCentralDataAccess via DI. Everything builds and tests without actual DLLs.
- **net48 SDK not on dev machine?** Legacy.Api scaffold is created but may not build locally. CI needs net48 targeting pack.
