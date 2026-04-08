# Plan: Email Templates — Port CustomEmails.cs Template Logic

**Issue:** #27  
**Spec:** `.speckit/specs/issue-27.md`  
**Status:** CLEAN

---

## Architecture Decision

`EmailService` lives in `FlyITA.Core` and can't access `IWebHostEnvironment` for file paths. Introduce `IEmailTemplateLoader` (interface in Core, implementation in Web) to abstract template loading:
- Checks local files first (`Templates/` folder)
- Falls back to sidecar (`IPCentralDataAccess.GetEmailTemplateAsync()`)

This replaces the direct `_dataAccess.GetEmailTemplateAsync()` calls in `EmailService`.

## Implementation Order

### Phase A: Template Infrastructure

**A1. Create `IEmailTemplateLoader` interface**
- File: `src/FlyITA.Core/Interfaces/IEmailTemplateLoader.cs`
- Method: `Task<string?> LoadTemplateAsync(string templateName)`

**A2. Create `FileEmailTemplateLoader` implementation**
- File: `src/FlyITA.Web/Services/FileEmailTemplateLoader.cs`
- Inject `IWebHostEnvironment` and `IPCentralDataAccess` and `IContextManager`
- Strategy: check `ContentRootPath/Templates/{templateName}` → if exists, read file → else fall back to `_dataAccess.GetEmailTemplateAsync(templateName, contextManager.ProgramID)`
- Register in DI as scoped (needs `IContextManager` which is scoped)
- Add `NullEmailTemplateLoader` to `Core/DependencyInjection.cs` (follows existing TryAdd pattern)

**A2b. Update `Core/DependencyInjection.cs`**
- Add `services.TryAddScoped<IEmailTemplateLoader, NullEmailTemplateLoader>()` 
- `NullEmailTemplateLoader` returns null (same pattern as NullPCentralDataAccess)

**A3. Copy template files**
- Copy `src/FlyITA/configuration/TravelerProfiler.html` → `src/FlyITA.Web/Templates/TravelerProfiler.html`
- Copy `src/FlyITA/configuration/VacationTravelRequest.html` → `src/FlyITA.Web/Templates/VacationTravelRequest.html`
- Mark as `Content` / `CopyToOutputDirectory: PreserveNewest` in `.csproj`

**A4. Update `appsettings.json` with template paths**
- Set `Email:TravelerProfileTemplate` → `TravelerProfiler.html`
- Set `Email:VacationTravelRequestTemplate` → `VacationTravelRequest.html`

### Phase B: Email Template Engine (Token Replacement)

**B1. Create `EmailTemplateEngine` in Core**
- File: `src/FlyITA.Core/Services/EmailTemplateEngine.cs`
- Static/utility class — pure string operations, no dependencies
- Methods:
  - `ReplaceParticipantTokens(string body, Dictionary<string, object?> participant)` — all `<<...>>` participant tokens
  - `ReplaceProgramTokens(string body, Dictionary<string, object?> program)` — all `[PROGRAM_...]` tokens
  - `ReplaceFormFieldTokens(string body, Dictionary<string, string> formValues)` — all `[FieldName]` tokens from form data
  - `ProcessRepeatingBlock(string body, string startTag, string endTag, List<Dictionary<string, string>> items, Dictionary<string, string> tokenMap)` — generic block processor for guest/passenger/frequent flyer blocks
  - `ReplaceCustomFieldTokens(string body, Dictionary<string, string> customFields)` — `<<CustomField.{name}>>` pattern

**B2. Update `EmailService` to use `IEmailTemplateLoader` and `EmailTemplateEngine`**
- File: `src/FlyITA.Core/Services/EmailService.cs`
- Add `IEmailTemplateLoader` to constructor (alongside existing `IConfiguration`, `IPCentralDataAccess`, `ISmtpClient`, `IContextManager`)
- Replace `_dataAccess.GetEmailTemplateAsync()` calls with `_templateLoader.LoadTemplateAsync()`
- Replace hand-coded 9-token `ReplacePlaceholdersAsync()` with calls to `EmailTemplateEngine` methods
- Keep `IConfiguration` for non-template config (addresses, subjects for standard emails)
- Add full token replacement for standard emails (participant + program data from sidecar)

### Phase C: Vacation Email (Template-Based)

**C1. Create `VacationEmailData` model**
- File: `src/FlyITA.Core/Models/VacationEmailData.cs`
- Email routing fields: `ToEmail`, `FromEmail`, `Subject` (page passes these from `EmailOptions`, since `EmailService` in Core can't access Web-layer options)
- Fields matching legacy form tokens: NameOfPersonRequesting, GeneralAndPassengerEmail, PhoneNumber, DepartureCity, PreferredAirline, DestinationsInterestedIn, PreferredDatesOfTravel, DestinationsNotInterestedIn, VacationDetails, ImportantAmenities, RoomsNeeded
- Nested list: `List<PassengerData>` (FirstName, LastName, BirthDate, Gender, PassportNumber, PassportExpirationDate)
- Nested list: `List<FrequentFlyerData>` (Airline, Number)

**C2. Add `SendVacationRequestEmailAsync` to `IEmailService` / `EmailService`**
- Load `VacationTravelRequest.html` template via `IEmailTemplateLoader`
- Replace form field tokens using `EmailTemplateEngine.ReplaceFormFieldTokens()`
- Process `<passengerinfoi>...</passengerinfoi>` repeating block
- Process `<frequentflyerti>...</frequentflyerti>` repeating block
- Send via `ISmtpClient`

**C3. Wire `Vacation.cshtml.cs` to use `IEmailService`**
- Replace `BuildEmailBody()` hardcoded HTML + direct `ISmtpClient` usage
- Inject `IEmailService` instead of `ISmtpClient` directly
- Build `VacationEmailData` from form properties
- Call `SendVacationRequestEmailAsync(data)`
- Remove `SendVacationRequestEmailAsync` and `BuildEmailBody` private methods from page model

### Phase D: Traveler Profile Email (Template-Based)

**D1. Create `TravelerProfileEmailData` model**
- File: `src/FlyITA.Core/Models/TravelerProfileEmailData.cs`
- Email routing fields: `ToEmail`, `FromEmail`, `Subject` (same pattern as VacationEmailData)
- Fields matching legacy form tokens: all 30+ fields from `Travelerprofileinformation.aspx.cs`
- Nested lists for repeating blocks: FrequentFlyers, HotelMemberships, RentalCarMemberships

**D2. Add `SendTravelerProfileFormEmailAsync` to `IEmailService` / `EmailService`**
- Load `TravelerProfiler.html` template
- Replace form field tokens
- Process repeating blocks (frequentflyerti, hotelclubmembershipsti, rentalcarmembershipsti)
- Send via `ISmtpClient`

**D3. Wire `TravelerProfile.cshtml.cs` to send email on submit**
- Inject `IEmailService`
- In `OnPostAsync`, after successful save, build `TravelerProfileEmailData` from form properties
- Call `SendTravelerProfileFormEmailAsync(data)`

### Phase E: Tests

**E1. Unit tests for `EmailTemplateEngine`**
- File: `tests/FlyITA.Core.Tests/Services/EmailTemplateEngineTests.cs`
- Test each replacement method: participant tokens, program tokens, form field tokens, custom field tokens
- Test repeating block processor with 0, 1, multiple items
- Test block markers removed even when no data

**E2. Unit tests for `FileEmailTemplateLoader`**
- File: `tests/FlyITA.Web.Tests/Services/FileEmailTemplateLoaderTests.cs`
- Test file exists → returns content
- Test file missing → falls back to sidecar
- Test sidecar also returns null → returns null

**E3. Update existing `EmailServiceTests`**
- File: `tests/FlyITA.Core.Tests/Services/EmailServiceTests.cs`
- Update to mock `IEmailTemplateLoader` instead of `IPCentralDataAccess.GetEmailTemplateAsync`
- Add tests for vacation email and traveler profile email methods
- Verify correct token replacement in email body

**E4. Page model tests**
- Update Vacation page tests to verify `IEmailService` is called (not `ISmtpClient` directly)
- Add TravelerProfile email test

### Phase F: Cleanup & Verify

**F1. Remove unused imports/code**
- Remove `ISmtpClient` injection from Vacation page model (now uses IEmailService)
- Remove `BuildEmailBody()` from Vacation page model
- Clean up old `ReplacePlaceholdersAsync` from EmailService (replaced by EmailTemplateEngine)

**F2. Full build + test**
- `dotnet build FlyITA.Modern.sln` — zero warnings
- `dotnet test FlyITA.Modern.sln --filter "FullyQualifiedName!~E2E"` — all pass

## File Change Summary

| Action | File |
|--------|------|
| CREATE | `src/FlyITA.Core/Interfaces/IEmailTemplateLoader.cs` |
| CREATE | `src/FlyITA.Core/Services/EmailTemplateEngine.cs` |
| CREATE | `src/FlyITA.Core/Models/VacationEmailData.cs` |
| CREATE | `src/FlyITA.Core/Models/TravelerProfileEmailData.cs` |
| MODIFY | `src/FlyITA.Core/Interfaces/IEmailService.cs` |
| MODIFY | `src/FlyITA.Core/Services/EmailService.cs` |
| MODIFY | `src/FlyITA.Core/DependencyInjection.cs` |
| CREATE | `src/FlyITA.Web/Services/FileEmailTemplateLoader.cs` |
| COPY   | `src/FlyITA.Web/Templates/TravelerProfiler.html` |
| COPY   | `src/FlyITA.Web/Templates/VacationTravelRequest.html` |
| MODIFY | `src/FlyITA.Web/Pages/Vacation.cshtml.cs` |
| MODIFY | `src/FlyITA.Web/Pages/TravelerProfile.cshtml.cs` |
| MODIFY | `src/FlyITA.Web/Program.cs` (register FileEmailTemplateLoader) |
| MODIFY | `src/FlyITA.Web/appsettings.json` |
| CREATE | `tests/FlyITA.Core.Tests/Services/EmailTemplateEngineTests.cs` |
| CREATE | `tests/FlyITA.Web.Tests/Services/FileEmailTemplateLoaderTests.cs` |
| MODIFY | `tests/FlyITA.Core.Tests/Services/EmailServiceTests.cs` |

## Dependencies

```
A1 → A2 → A3 → A4
     ↓
B1 → B2 ──────────→ C1 → C2 → C3
                     D1 → D2 → D3
                     E1 → E2 → E3 → E4 → F1 → F2
```

## Package Changes

None — all needed packages already present.
