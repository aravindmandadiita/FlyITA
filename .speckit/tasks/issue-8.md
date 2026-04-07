# Tasks: Phase 7 — Legacy API Sidecar & Dynamic Page Migration

**Plan:** [issue-8.md](../plans/issue-8.md)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## Task 1: Create LegacyApiOptions in Core

**Priority:** P0 (blocks Tasks 2-4)

**Given** the FlyITA.Core project exists  
**When** I add a LegacyApiOptions class  
**Then** it has BaseUrl and TimeoutSeconds properties with sensible defaults  

**Files:**
- `src/FlyITA.Core/Options/LegacyApiOptions.cs` (NEW)

**Test:** Compile check — options class instantiates with defaults.

---

## Task 2: Create FlyITA.Legacy.Api project scaffold

**Priority:** P0 (blocks Task 3)

**Given** we need a .NET Framework 4.8 Web API project  
**When** I create the project structure  
**Then** it has its own solution file, Web.config, WebApiConfig, Global.asax, and a health endpoint  
**And** the lib/ folder is gitignored  

**Files:**
- `src/FlyITA.Legacy.Api/FlyITA.Legacy.Api.csproj` (NEW)
- `src/FlyITA.Legacy.Api/FlyITA.Legacy.sln` (NEW)
- `src/FlyITA.Legacy.Api/Web.config` (NEW)
- `src/FlyITA.Legacy.Api/Global.asax` (NEW)
- `src/FlyITA.Legacy.Api/Global.asax.cs` (NEW)
- `src/FlyITA.Legacy.Api/App_Start/WebApiConfig.cs` (NEW)
- `src/FlyITA.Legacy.Api/Controllers/HealthController.cs` (NEW)
- `src/FlyITA.Legacy.Api/lib/.gitkeep` (NEW)
- `src/FlyITA.Legacy.Api/.gitignore` (NEW — ignore DLLs in lib/)

**Test:** Project structure exists; .gitignore covers lib/*.dll.

---

## Task 3: Create Legacy.Api controllers (placeholder stubs)

**Priority:** P0 (blocks Task 4)  
**Depends on:** Task 2

**Given** the Legacy.Api project scaffold exists  
**When** I add controllers for each IPCentralDataAccess method  
**Then** each endpoint returns placeholder JSON data  
**And** each has a TODO comment for wiring real DLL calls  

**Files:**
- `src/FlyITA.Legacy.Api/Controllers/ParticipantsController.cs` (NEW)
- `src/FlyITA.Legacy.Api/Controllers/PersonsController.cs` (NEW)
- `src/FlyITA.Legacy.Api/Controllers/ProgramsController.cs` (NEW)
- `src/FlyITA.Legacy.Api/Controllers/EmailController.cs` (NEW)
- `src/FlyITA.Legacy.Api/Controllers/PageConfigController.cs` (NEW)
- `src/FlyITA.Legacy.Api/Controllers/TransportationController.cs` (NEW)

**Endpoints:**
- GET `/api/participants/{id}` → placeholder participant JSON
- GET `/api/persons/{id}` → placeholder person JSON
- GET `/api/participants/{id}/party` → placeholder party JSON
- GET `/api/programs/{id}` → placeholder program JSON
- GET `/api/participants/{id}/custom-fields` → placeholder array
- PUT `/api/participants/{id}/custom-fields` → 204
- GET `/api/participants/{id}/accommodations` → placeholder JSON
- GET `/api/participants/{id}/accommodations/list` → placeholder array
- PUT `/api/participants/{id}/accommodations` → 204
- DELETE `/api/participants/{id}/accommodations/{type}` → 204
- GET `/api/email-templates/{name}?programId={id}` → placeholder string
- GET `/api/email-body/{key}?programId={id}` → placeholder string
- GET `/api/persons/{id}/contacts` → placeholder array
- GET `/api/page-config/{pageName}?programNumber={num}` → placeholder JSON
- GET `/api/participants/{id}/transportation` → placeholder JSON

**Test:** Manual — endpoints return expected HTTP status codes.

---

## Task 4: Rewire PCentralDataAccess to HttpClient

**Priority:** P0 (blocks Tasks 7-10)  
**Depends on:** Task 1, Task 3

**Given** PCentralDataAccess currently uses IDatabaseAccess with guessed SP names  
**When** I rewrite it to use HttpClient  
**Then** each method calls the corresponding Legacy.Api endpoint  
**And** responses are deserialized to Dictionary<string, object?> for interface compatibility  
**And** the constructor takes HttpClient instead of IDatabaseAccess  

**Files:**
- `src/FlyITA.Infrastructure/Data/PCentralDataAccess.cs` (REWRITE)

**Test:** Task 6 covers this.

---

## Task 5: Update DI registration

**Priority:** P0 (blocks Tasks 7-10)  
**Depends on:** Task 1, Task 4

**Given** PCentralDataAccess now needs HttpClient  
**When** I update the DI registration  
**Then** LegacyApiOptions is bound from configuration  
**And** a named/typed HttpClient is registered for PCentralDataAccess  
**And** the HttpClient has the base URL from LegacyApiOptions  

**Files:**
- `src/FlyITA.Infrastructure/DependencyInjection.cs` (MODIFY)
- `src/FlyITA.Web/Program.cs` (MODIFY)
- `src/FlyITA.Web/appsettings.json` (MODIFY)
- `src/FlyITA.Web/appsettings.Development.json` (MODIFY)

**Test:** App starts without DI errors; HttpClient resolves correctly.

---

## Task 6: Rewrite PCentralDataAccess tests

**Priority:** P0  
**Depends on:** Task 4

**Given** PCentralDataAccess now uses HttpClient  
**When** I rewrite the tests  
**Then** they mock HttpMessageHandler instead of IDatabaseAccess  
**And** they verify correct HTTP method, URL, and request body for each IPCentralDataAccess method  
**And** they verify correct deserialization of responses  

**Files:**
- `tests/FlyITA.Infrastructure.Tests/Data/PCentralDataAccessTests.cs` (REWRITE)

**Test:** All tests pass with `dotnet test`.

---

## Task 7: GuestProfile Razor Page

**Priority:** P1  
**Depends on:** Task 5

**Given** the data layer is wired to the sidecar  
**When** I create the GuestProfile page  
**Then** it renders a form with participant fields (name, address, phone, email, passport, citizenship, dietary, assistance)  
**And** it loads data via IPCentralDataAccess on GET  
**And** it validates and saves via IPCentralDataAccess on POST  
**And** custom fields render dynamically based on program config  
**And** conditional sections show/hide based on page configuration  

**Files:**
- `src/FlyITA.Web/Pages/GuestProfile.cshtml` (NEW)
- `src/FlyITA.Web/Pages/GuestProfile.cshtml.cs` (NEW)
- `tests/FlyITA.Web.Tests/Pages/GuestProfileTests.cs` (NEW)

**Test:** Page returns 200, form renders fields, POST validates required fields.

---

## Task 8: TravelerProfile Razor Page

**Priority:** P1  
**Depends on:** Task 5

**Given** the data layer is wired to the sidecar  
**When** I create the TravelerProfile page  
**Then** it renders a multi-section form (personal info, frequent flyer, hotel, rental car)  
**And** repeater sections allow add/remove entries  
**And** it loads person + contact data on GET  
**And** it validates and saves on POST  

**Files:**
- `src/FlyITA.Web/Pages/TravelerProfile.cshtml` (NEW)
- `src/FlyITA.Web/Pages/TravelerProfile.cshtml.cs` (NEW)
- `tests/FlyITA.Web.Tests/Pages/TravelerProfileTests.cs` (NEW)

**Test:** Page returns 200, form renders sections, POST validates.

---

## Task 9: VacationRequest Razor Page

**Priority:** P1  
**Depends on:** Task 5

**Given** the data layer is wired to the sidecar  
**When** I create the VacationRequest page  
**Then** it renders a travel request form (destination, dates, travelers, preferences)  
**And** it validates on POST  
**And** it links to traveler profile for pre-fill  

**Files:**
- `src/FlyITA.Web/Pages/VacationRequest.cshtml` (NEW)
- `src/FlyITA.Web/Pages/VacationRequest.cshtml.cs` (NEW)
- `tests/FlyITA.Web.Tests/Pages/VacationRequestTests.cs` (NEW)

**Test:** Page returns 200, form renders, POST validates required fields.

---

## Task 10: AchPayment Razor Page

**Priority:** P1  
**Depends on:** Task 5

**Given** the data layer is wired to the sidecar  
**When** I create the AchPayment page  
**Then** it renders a payment form (bank name, routing, account, amount)  
**And** it validates routing number format and required fields on POST  
**And** no PII is logged  

**Files:**
- `src/FlyITA.Web/Pages/AchPayment.cshtml` (NEW)
- `src/FlyITA.Web/Pages/AchPayment.cshtml.cs` (NEW)
- `tests/FlyITA.Web.Tests/Pages/AchPaymentTests.cs` (NEW)

**Test:** Page returns 200, validation rejects bad routing numbers, no PII in test logs.

---

## Task 11: Legacy URL redirects for form pages

**Priority:** P2  
**Depends on:** Tasks 7-10

**Given** the form pages exist at modern URLs  
**When** I add legacy .aspx redirects  
**Then** the old URLs return 301 to the new paths  

**Files:**
- `src/FlyITA.Web/Program.cs` (MODIFY)

**Redirects:**
- `/GuestProfileInformation.aspx` → `/guest-profile`
- `/Travelerprofileinformation.aspx` → `/traveler-profile`
- `/VacationTravelRequest.aspx` → `/vacation-request`
- `/AchPayment.aspx` → `/ach-payment`

**Test:** Integration test — GET legacy URL returns 301 with correct Location header.

---

## Task 12: Verify all tests pass

**Priority:** P0  
**Depends on:** All above

**Given** all code changes are complete  
**When** I run `dotnet test` on the main solution  
**Then** all existing tests pass (207+ from Phase 6)  
**And** all new tests pass  
**And** the solution builds with zero warnings  
