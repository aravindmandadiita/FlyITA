# Spec: Phase 7 — Legacy API Sidecar & Dynamic Page Migration

**Issue:** [#8 — Phase 7: Page Migration — Forms & Complex Pages](https://github.com/aravindmandadiita/FlyITA/issues/8)  
**Epic:** [#1 — EPIC: Platform Tech Debt Upgrade](https://github.com/aravindmandadiita/FlyITA/issues/1)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## 1. Objective

Build a .NET Framework 4.8 sidecar API (`FlyITA.Legacy.Api`) that wraps the third-party PCentralLib.dll and ITALib.dll as REST endpoints, then rewire `FlyITA.Infrastructure` to call the sidecar via HttpClient instead of guessed stored procedure names. Once the data layer is functional, migrate the four dynamic form pages: GuestProfileInformation, TravelerProfileInformation, VacationTravelRequest, and AchPayment.

## 2. Background

### 2.1 The DLL Problem

FlyITA depends on three third-party .NET Framework 4.x DLLs maintained by the EIS team:

| DLL | Purpose | Database Access |
|-----|---------|----------------|
| **PCentralLib.dll** | Business entities (Participant, Party, Program, CustomField) | Handles its own DB access internally (black box) |
| **ITALib.dll** | Low-level data access (`DB.Get_Row`), config resolution (`ITA_Configuration`) | Direct DB calls internally |
| **ITAErrorLogging.dll** | Error logging via `spInsLogMessage` | Direct DB calls internally |

These DLLs:
- Are **pre-compiled .NET Framework 4.x** — cannot load in .NET 10
- Are **black boxes** — no source code, internal implementation unknown
- Handle **all database access** — FlyITA has NO direct database access
- Get periodic updates (new DLL drops from EIS team)
- Will be upgraded to .NET Standard before 2029 (on Nicole's roadmap)

### 2.2 Current State (Phase 4 — Wrong Approach)

Phase 4 created `PCentralDataAccess` with **guessed stored procedure names** (e.g., `spWRASelParticipant`, `spWRASelPerson`). This is incorrect because:
- FlyITA never calls SPs directly — the DLLs do
- The SP names are unverified guesses
- `IDatabaseAccess` was designed for direct DB calls that FlyITA doesn't make

### 2.3 Sidecar Decision (confirmed 2026-04-03)

**Now:** Build `FlyITA.Legacy.Api` (.NET Framework 4.8) in the same solution. It loads the DLLs natively and exposes their methods as REST endpoints.

**Later (when EIS upgrades to .NET Standard):** Drop the sidecar, reference DLLs directly in Infrastructure.

```
Current Architecture (Phase 7):
FlyITA.Web (.NET 10) → HttpClient → FlyITA.Legacy.Api (.NET 4.8) → PCentralLib/ITALib → Database

Future Architecture (post-EIS upgrade):
FlyITA.Web (.NET 10) → Infrastructure → PCentralLib (.NET Standard) → Database
```

### 2.4 Clean Interface Boundary

The existing `IPCentralDataAccess` interface stays the same. Only the Infrastructure implementation changes:
- **Before:** `PCentralDataAccess` calls `IDatabaseAccess.ExecuteStoredProcedure()` with guessed SP names
- **After:** `PCentralDataAccess` calls `HttpClient` to hit Legacy.Api REST endpoints

The Web layer and Core layer are completely untouched.

## 3. Functional Requirements

### 3.1 FlyITA.Legacy.Api (NET Framework 4.8 Web API)

**New project:** `src/FlyITA.Legacy.Api/`

A standalone ASP.NET Web API 2 project targeting .NET Framework 4.8. It:
- References PCentralLib.dll and ITALib.dll (added as binary references from a `lib/` folder)
- Exposes REST endpoints that wrap DLL method calls
- Runs as a separate process (IIS application or self-hosted)
- Has its own port (configured via launchSettings)

**API contract** — mirrors `IPCentralDataAccess` methods:

| Endpoint | HTTP | DLL Call | Returns |
|----------|------|----------|---------|
| `/api/participants/{id}` | GET | `PCentralParticipant.Get(id)` | JSON participant |
| `/api/persons/{id}` | GET | `PCentralPerson.Get(id)` | JSON person |
| `/api/participants/{id}/party` | GET | `PCentralParty.GetByParticipant(id)` | JSON party |
| `/api/programs/{id}` | GET | `PCentralProgram.Get(id)` | JSON program |
| `/api/participants/{id}/custom-fields` | GET | `PCentralProgramCustomField.GetValues(id)` | JSON array |
| `/api/participants/{id}/custom-fields` | PUT | `PCentralProgramCustomField.Save(...)` | 204 |
| `/api/participants/{id}/accommodations` | GET | `PCentralParticipant.GetAccommodation(id)` | JSON |
| `/api/participants/{id}/accommodations/list` | GET | `PCentralParticipant.GetAccommodationList(id)` | JSON array |
| `/api/participants/{id}/accommodations` | PUT | `PCentralParticipant.SaveAccommodation(...)` | 204 |
| `/api/participants/{id}/accommodations/{type}` | DELETE | `PCentralParticipant.DeleteAccommodation(...)` | 204 |
| `/api/email-templates/{name}?programId={id}` | GET | template lookup | JSON string |
| `/api/email-body/{key}?programId={id}` | GET | body lookup | JSON string |
| `/api/persons/{id}/contacts` | GET | contact number lookup | JSON array |
| `/api/page-config/{pageName}?programNumber={num}` | GET | page config lookup | JSON |
| `/api/participants/{id}/transportation` | GET | transportation lookup | JSON |

**Note:** The exact DLL method signatures are unknown (black box). The controller implementations will be placeholder stubs calling the DLL types. Once we have the actual DLLs in the `lib/` folder, the real method calls get wired up. The REST contract is what matters — it's stable regardless of internal DLL implementation.

### 3.2 Infrastructure Rewire — PCentralDataAccess via HttpClient

**File:** `src/FlyITA.Infrastructure/Data/PCentralDataAccess.cs` (REWRITE)

Replace the current `IDatabaseAccess`-based implementation with `HttpClient` calls to the Legacy.Api:

- Constructor takes `HttpClient` (via `IHttpClientFactory`) instead of `IDatabaseAccess`
- Each method sends an HTTP request to the corresponding Legacy.Api endpoint
- Responses are deserialized from JSON to `Dictionary<string, object?>` (maintaining interface compatibility)
- Base URL configured via `LegacyApiOptions` (Options pattern)

### 3.3 Remove IDatabaseAccess Dependency from PCentralDataAccess

The `IDatabaseAccess` interface and `DatabaseAccess` class stay in the codebase (they may be needed for other direct DB operations like error logging). But `PCentralDataAccess` no longer depends on it.

### 3.4 LegacyApiOptions Configuration

**New file:** `src/FlyITA.Core/Options/LegacyApiOptions.cs`

```csharp
public class LegacyApiOptions
{
    public const string SectionName = "LegacyApi";
    public string BaseUrl { get; set; } = "http://localhost:5100";
    public int TimeoutSeconds { get; set; } = 30;
}
```

Added to `appsettings.json`:
```json
"LegacyApi": {
  "BaseUrl": "http://localhost:5100",
  "TimeoutSeconds": 30
}
```

### 3.5 Form Page Migrations

These pages depend on the sidecar being functional. Each becomes an ASP.NET Core Razor Page with a PageModel.

#### 3.5.1 GuestProfileInformation.aspx → `/guest-profile`

**Files:** `Pages/GuestProfile.cshtml` + `Pages/GuestProfile.cshtml.cs`

- Form with 30+ fields: name, address, phone, email, passport, citizenship, dietary restrictions, assistance needs
- Conditional sections (show/hide based on program configuration)
- Custom field integration (textbox, dropdown, required-if, write-once)
- Server-side validation + client-side validation
- Loads participant data via `IPCentralDataAccess.GetParticipantById()`
- Saves via `IPCentralDataAccess.SaveCustomFieldValue()`
- Page config via `IPCentralDataAccess.GetPageConfiguration()`

#### 3.5.2 TravelerProfileInformation.aspx → `/traveler-profile`

**Files:** `Pages/TravelerProfile.cshtml` + `Pages/TravelerProfile.cshtml.cs`

- Multi-section form: personal info, frequent flyer programs, hotel memberships, rental car preferences
- Repeater sections (add/remove frequent flyer entries)
- Captcha integration
- Email generation on submit (template replacement, guest blocks)
- Loads person data via `IPCentralDataAccess.GetPersonById()`
- Contact numbers via `IPCentralDataAccess.GetContactNumbers()`

#### 3.5.3 VacationTravelRequest.aspx → `/vacation-request`

**Files:** `Pages/VacationRequest.cshtml` + `Pages/VacationRequest.cshtml.cs`

- Travel request form: destination, dates, travelers, preferences
- Links to traveler profile for pre-fill
- Email notification on submit

#### 3.5.4 AchPayment.aspx → `/ach-payment`

**Files:** `Pages/AchPayment.cshtml` + `Pages/AchPayment.cshtml.cs`

- ACH payment form: bank name, routing number, account number, amount
- Server-side validation (routing number format, required fields)
- Integration with payment WCF services (CardProcessService)
- Secure handling — no PII logged

## 4. File Structure

```
src/
├── FlyITA.Legacy.Api/                          (NEW — .NET Framework 4.8 Web API)
│   ├── FlyITA.Legacy.Api.csproj
│   ├── App_Start/
│   │   └── WebApiConfig.cs
│   ├── Controllers/
│   │   ├── ParticipantsController.cs
│   │   ├── PersonsController.cs
│   │   ├── ProgramsController.cs
│   │   ├── EmailController.cs
│   │   ├── PageConfigController.cs
│   │   └── TransportationController.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Global.asax / Global.asax.cs
│   ├── Web.config
│   └── lib/                                     (DLL binaries — gitignored)
│       ├── PCentralLib.dll
│       ├── ITALib.dll
│       └── ITAErrorLogging.dll
├── FlyITA.Core/
│   └── Options/
│       └── LegacyApiOptions.cs                  (NEW)
├── FlyITA.Infrastructure/
│   └── Data/
│       └── PCentralDataAccess.cs                (REWRITE — HttpClient instead of IDatabaseAccess)
├── FlyITA.Web/
│   ├── Pages/
│   │   ├── GuestProfile.cshtml                  (NEW)
│   │   ├── GuestProfile.cshtml.cs               (NEW)
│   │   ├── TravelerProfile.cshtml               (NEW)
│   │   ├── TravelerProfile.cshtml.cs            (NEW)
│   │   ├── VacationRequest.cshtml               (NEW)
│   │   ├── VacationRequest.cshtml.cs            (NEW)
│   │   ├── AchPayment.cshtml                    (NEW)
│   │   └── AchPayment.cshtml.cs                 (NEW)
│   ├── Program.cs                               (MODIFY — register HttpClient, LegacyApiOptions)
│   └── appsettings.json                         (MODIFY — add LegacyApi section)

tests/
├── FlyITA.Infrastructure.Tests/
│   └── Data/
│       └── PCentralDataAccessTests.cs           (REWRITE — mock HttpClient instead of IDatabaseAccess)
├── FlyITA.Web.Tests/
│   └── Pages/
│       ├── GuestProfileTests.cs                 (NEW)
│       ├── TravelerProfileTests.cs              (NEW)
│       ├── VacationRequestTests.cs              (NEW)
│       └── AchPaymentTests.cs                   (NEW)
```

## 5. Non-Functional Requirements

### 5.1 Sidecar Deployment
- Legacy.Api has its own `.sln` file (`FlyITA.Legacy.sln`) — cannot share a solution with net10 projects
- Runs as a separate IIS application on the same server
- Communicates via localhost HTTP (no external network hops)
- Must handle concurrent requests (thread-safe DLL usage)

### 5.2 Resilience
- HttpClient configured with timeout (default 30s)
- Retry policy for transient failures (Polly recommended)
- Health check endpoint on Legacy.Api (`/api/health`)

### 5.3 Security
- Legacy.Api listens on localhost only (not exposed externally)
- No authentication between services (trusted internal network)
- ACH payment data: no PII in logs, HTTPS in production

### 5.4 Testing
- PCentralDataAccess tests mock HttpClient (HttpMessageHandler)
- Legacy.Api controllers tested with in-memory DLL mocks
- Form page tests use WebApplicationFactory with mocked IPCentralDataAccess
- Minimum 80% coverage on business logic

### 5.5 Configuration
- Legacy.Api base URL configurable per environment
- Connection strings remain on the Legacy.Api side (DLLs consume them)

## 6. Out of Scope

- Decompiling or porting PCentralLib/ITALib source code
- Direct database access from FlyITA.Web or FlyITA.Infrastructure
- Upgrading DLLs to .NET Standard (EIS team responsibility)
- Login/registration pages (Phase 8)
- Frontend modernization (Phase 9)

## 7. Acceptance Criteria

1. **Legacy.Api builds and runs:** net48 project loads DLLs, exposes REST endpoints, returns health check 200
2. **PCentralDataAccess rewired:** Uses HttpClient to Legacy.Api — no more guessed SP names or IDatabaseAccess dependency
3. **Guest Profile form works:** Loads participant data, renders form, validates, saves custom fields
4. **Traveler Profile form works:** Multi-section form with repeaters, captcha, email generation
5. **Vacation Request form works:** Submit travel request, email notification
6. **ACH Payment form works:** Secure payment form, WCF service integration
7. **All existing tests pass:** No regressions (207+ tests from Phase 6)
8. **New tests pass:** PCentralDataAccess HttpClient tests, form page rendering/validation tests
9. **Solution builds:** Zero errors, zero warnings across all projects
10. **Legacy .aspx redirects:** Add redirects for the 4 form page URLs

## 8. Dependencies

- Phase 6 (Static Pages) — MERGED (PR #18)
- PCentralLib.dll, ITALib.dll, ITAErrorLogging.dll — must be available in `lib/` folder
- .NET Framework 4.8 Developer Pack (for Legacy.Api build)
- .NET 10 SDK (for main solution)

## 9. Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|-----------|
| DLL method signatures unknown | Can't implement real controller logic | Build controllers with placeholder stubs; wire real calls when DLLs available. All development/testing uses mock implementations behind the same interfaces. |
| DLLs not thread-safe | Concurrency issues under load | Test with concurrent requests; add locking if needed |
| net48 project in same solution | Build tooling conflicts | Separate .sln file for Legacy.Api if needed |
| Form field mapping unknown | Missing/wrong fields | Scaffold forms from IPCentralDataAccess contract + issue description; refine with manual testing |
| WCF service changes | Payment flow breaks | Keep existing WCF client proxies; test against dev endpoint |
