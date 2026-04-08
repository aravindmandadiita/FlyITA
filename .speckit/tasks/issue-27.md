# Tasks: Email Templates — Port CustomEmails.cs Template Logic

**Issue:** #27  
**Plan:** `.speckit/plans/issue-27.md`  
**Status:** CLEAN

---

## Task 1: Create IEmailTemplateLoader interface

**File:** `src/FlyITA.Core/Interfaces/IEmailTemplateLoader.cs` (CREATE)

**Given** EmailService needs to load templates from files or sidecar  
**When** I create an abstraction for template loading  
**Then** Core can depend on it without knowing about the file system

**Changes:**
```csharp
public interface IEmailTemplateLoader
{
    Task<string?> LoadTemplateAsync(string templateName, CancellationToken ct = default);
}
```

**Verification:** `dotnet build src/FlyITA.Core/`

---

## Task 2: Add NullEmailTemplateLoader to Core DI

**File:** `src/FlyITA.Core/DependencyInjection.cs`

**Given** Core follows the TryAdd/Null pattern for pluggable services  
**When** I add NullEmailTemplateLoader  
**Then** Core tests work without Web layer

**Changes:**
- Add `services.TryAddScoped<IEmailTemplateLoader, NullEmailTemplateLoader>()`
- Add internal `NullEmailTemplateLoader` class that returns null

**Verification:** `dotnet build src/FlyITA.Core/`

---

## Task 3: Create EmailTemplateEngine utility

**File:** `src/FlyITA.Core/Services/EmailTemplateEngine.cs` (CREATE)

**Given** CustomEmails.cs has 40+ token replacements and repeating block logic  
**When** I port the token replacement as a pure utility class  
**Then** all email methods can use it for consistent replacement

**Methods:**
- `static string ReplaceParticipantTokens(string body, Dictionary<string, object?> participant)` — `<<LegalFirstName>>`, `<<LegalLastName>>`, `<<LegalMiddleName>>`, `<<DateOfBirth>>`, `<<Gender>>`, `<<EmailAddress>>`, `[PARTICIPANT_NAME]`, `[PARTICIPANT_EMAIL]`, `[PARTICIPANT_USERNAME]`, `[PARTICIPANT_PASSWORD]`, `[FIRSTNAME]`, `[LASTNAME]`
- `static string ReplaceProgramTokens(string body, Dictionary<string, object?> program)` — `[PROGRAM_NAME]`, `[PROGRAM_800NBR]`, `[PROGRAM_HQNAME]`, `[PROGRAM_URL]`, `[PROGRAM_EMAIL]`
- `static string ReplaceContactTokens(string body, List<Dictionary<string, object?>> contacts)` — `<<BusinessPhone>>`, `<<BusinessFax>>`, `<<MobilePhone>>`, `<<HomePhone>>`
- `static string ReplaceTransportationTokens(string body, Dictionary<string, object?> transportation)` — `<<TransportationType>>`, `<<TravelDates>>`, `<<PrefHomeDepartureTime>>`, `<<PrefDestDepartureTime>>`, `<<PrefAirline>>`, `<<PrefHomeDepartureCity>>`, `<<DriveTime>>`, `<<SeatPreference>>`, `<<AirRemarks>>`, `<<FrequentFlyerNumber>>`, `<<CheckInDate>>`, `<<CheckOutDate>>`, `<<Wheelchair>>`, `<<SpecialMeal>>`
- `static string ReplaceCustomFieldTokens(string body, List<Dictionary<string, object?>> customFields)` — `<<CustomField.{name}>>` pattern
- `static string ReplaceFormFieldTokens(string body, Dictionary<string, string> formValues)` — `[FieldName]` pattern for page-specific tokens
- `static string ProcessRepeatingBlock(string body, string startTag, string endTag, List<Dictionary<string, string>> items)` — generic block processor

**Verification:** `dotnet build src/FlyITA.Core/`

---

## Task 4: Unit tests for EmailTemplateEngine

**File:** `tests/FlyITA.Core.Tests/Services/EmailTemplateEngineTests.cs` (CREATE)

**Tests:**
1. `ReplaceParticipantTokens_ReplacesAllKnownTokens`
2. `ReplaceParticipantTokens_MissingKeys_ReplacesWithEmpty`
3. `ReplaceProgramTokens_ReplacesAllKnownTokens`
4. `ReplaceFormFieldTokens_ReplacesMatchingKeys`
5. `ReplaceFormFieldTokens_UnmatchedTokens_LeftAlone`
6. `ReplaceCustomFieldTokens_ReplacesMatchingFields`
7. `ReplaceCustomFieldTokens_NoCustomFields_RemovesTokens`
8. `ProcessRepeatingBlock_MultipleItems_ExpandsBlock`
9. `ProcessRepeatingBlock_ZeroItems_RemovesBlock`
10. `ProcessRepeatingBlock_NoMarkers_ReturnsSameBody`
11. `ReplaceContactTokens_MapsToCorrectPhoneTypes`
12. `ReplaceTransportationTokens_ReplacesAllTravelFields`

**Verification:** `dotnet test tests/FlyITA.Core.Tests/`

---

## Task 5: Copy template files and configure paths

**Files:**
- COPY `src/FlyITA/configuration/TravelerProfiler.html` → `src/FlyITA.Web/Templates/TravelerProfiler.html`
- COPY `src/FlyITA/configuration/VacationTravelRequest.html` → `src/FlyITA.Web/Templates/VacationTravelRequest.html`
- MODIFY `src/FlyITA.Web/FlyITA.Web.csproj` — add `<Content>` items with `CopyToOutputDirectory`
- MODIFY `src/FlyITA.Web/appsettings.json` — set template paths:
  - `Email:TravelerProfileTemplate` → `TravelerProfiler.html`
  - `Email:VacationTravelRequestTemplate` → `VacationTravelRequest.html`

**Verification:** `dotnet build src/FlyITA.Web/` and template files appear in output

---

## Task 6: Create FileEmailTemplateLoader

**File:** `src/FlyITA.Web/Services/FileEmailTemplateLoader.cs` (CREATE)

**Given** templates are on disk first, sidecar fallback second  
**When** I implement IEmailTemplateLoader  
**Then** EmailService gets templates from the right source

**Changes:**
- Inject `IWebHostEnvironment`, `IPCentralDataAccess`, `IContextManager`, `ILogger<FileEmailTemplateLoader>`
- `LoadTemplateAsync(templateName)`:
  - Build path: `Path.Combine(env.ContentRootPath, "Templates", templateName)`
  - If file exists: `await File.ReadAllTextAsync(path, ct)`
  - Else: `await _dataAccess.GetEmailTemplateAsync(templateName, _context.ProgramID)`
  - Log which source was used

**Register in `Program.cs`:**
```csharp
builder.Services.AddScoped<IEmailTemplateLoader, FileEmailTemplateLoader>();
```

**Verification:** `dotnet build src/FlyITA.Web/`

---

## Task 7: Unit tests for FileEmailTemplateLoader

**File:** `tests/FlyITA.Web.Tests/Services/FileEmailTemplateLoaderTests.cs` (CREATE)

**Tests:**
1. `LoadTemplate_FileExists_ReturnsFileContent` — mock env with temp dir, write file, verify content returned
2. `LoadTemplate_FileMissing_FallsBackToSidecar` — no file, mock sidecar returns content
3. `LoadTemplate_BothMissing_ReturnsNull` — no file, sidecar returns null

**Verification:** `dotnet test tests/FlyITA.Web.Tests/`

---

## Task 8: Create VacationEmailData model

**File:** `src/FlyITA.Core/Models/VacationEmailData.cs` (CREATE)

**Properties:**
- `ToEmail`, `FromEmail`, `Subject` (routing — page fills from EmailOptions)
- `NameOfPersonRequesting`, `GeneralAndPassengerEmail`, `PhoneNumber`
- `DepartureCity`, `PreferredAirline`, `DestinationsInterestedIn`, `PreferredDatesOfTravel`
- `DestinationsNotInterestedIn`, `VacationDetails`, `ImportantAmenities`, `RoomsNeeded`
- `List<PassengerData> Passengers` (FirstName, LastName, BirthDate, Gender, PassportNumber, PassportExpirationDate)
- `List<FrequentFlyerData> FrequentFlyers` (Airline, Number)

Inner classes `PassengerData` and `FrequentFlyerData`.

**Verification:** `dotnet build src/FlyITA.Core/`

---

## Task 9: Create TravelerProfileEmailData model

**File:** `src/FlyITA.Core/Models/TravelerProfileEmailData.cs` (CREATE)

**Properties:**
- `ToEmail`, `FromEmail`, `Subject` (routing)
- All 30+ fields from legacy template tokens (most will be empty string from current form — populated as form is expanded)
- `List<FrequentFlyerData> FrequentFlyers`
- `List<HotelMembershipData> HotelMemberships` (HotelChain, MemberNumber)
- `List<RentalCarMembershipData> RentalCarMemberships` (Company, MemberNumber)

**Verification:** `dotnet build src/FlyITA.Core/`

---

## Task 10: Update IEmailService and EmailService

**Files:**
- `src/FlyITA.Core/Interfaces/IEmailService.cs` — add 2 methods
- `src/FlyITA.Core/Services/EmailService.cs` — rewrite to use IEmailTemplateLoader + EmailTemplateEngine

**Interface additions:**
```csharp
Task<ValidationResult> SendVacationRequestEmailAsync(VacationEmailData data);
Task<ValidationResult> SendTravelerProfileFormEmailAsync(TravelerProfileEmailData data);
```

**EmailService changes:**
- Add `IEmailTemplateLoader` to constructor
- Replace all `_dataAccess.GetEmailTemplateAsync()` with `_templateLoader.LoadTemplateAsync()`
- Replace `ReplacePlaceholdersAsync()` with `EmailTemplateEngine` methods for standard emails
- Implement `SendVacationRequestEmailAsync`:
  - Load template via `_templateLoader.LoadTemplateAsync(config["Email:VacationTravelRequestTemplate"])`
  - Convert `VacationEmailData` form fields to `Dictionary<string, string>` for `ReplaceFormFieldTokens`
  - Process `<passengerinfoi>` and `<frequentflyerti>` repeating blocks via `ProcessRepeatingBlock`
  - Send via `_smtpClient`
- Implement `SendTravelerProfileFormEmailAsync`:
  - Same pattern with `TravelerProfiler.html` template
  - Process `<frequentflyerti>`, `<hotelclubmembershipsti>`, `<rentalcarmembershipsti>` blocks

**Verification:** `dotnet build src/FlyITA.Core/`

---

## Task 11: Update EmailService tests

**File:** `tests/FlyITA.Core.Tests/Services/EmailServiceTests.cs`

**Changes:**
- Mock `IEmailTemplateLoader` instead of `IPCentralDataAccess.GetEmailTemplateAsync`
- Add tests for `SendVacationRequestEmailAsync`:
  - Template loaded → tokens replaced → email sent with correct body
  - Template missing → returns error
  - Repeating blocks populated correctly
- Add tests for `SendTravelerProfileFormEmailAsync`:
  - Same pattern
- Verify existing standard email tests still pass

**Verification:** `dotnet test tests/FlyITA.Core.Tests/`

---

## Task 12: Wire Vacation page to IEmailService

**File:** `src/FlyITA.Web/Pages/Vacation.cshtml.cs`

**Changes:**
- Replace `ISmtpClient` constructor parameter with `IEmailService`
- Remove private `SendVacationRequestEmailAsync` and `BuildEmailBody` methods
- In `OnPostAsync`, after validation:
  - Build `VacationEmailData` from form properties + `_emailOptions` (ToEmail, FromEmail, Subject)
  - Call `await _emailService.SendVacationRequestEmailAsync(data)`
  - Handle result (success/error)

**Verification:** `dotnet build src/FlyITA.Web/`

---

## Task 13: Wire TravelerProfile page to send email

**File:** `src/FlyITA.Web/Pages/TravelerProfile.cshtml.cs`

**Changes:**
- Add `IEmailService` and `IOptions<EmailOptions>` to constructor
- In `OnPostAsync`, after successful save:
  - Build `TravelerProfileEmailData` from form properties + email options
  - Call `await _emailService.SendTravelerProfileFormEmailAsync(data)`
  - Handle result (log error but don't fail the page — email is best-effort, matching legacy behavior)

**Verification:** `dotnet build src/FlyITA.Web/`

---

## Task 14: Update page tests

**Changes:**
- Update Vacation page tests: verify `IEmailService.SendVacationRequestEmailAsync` is called (mock), not `ISmtpClient.SendAsync`
- Add TravelerProfile test: verify `IEmailService.SendTravelerProfileFormEmailAsync` is called on POST
- Verify `FormPageTests` still passes (GET requests unaffected by new constructor params)

**Verification:** `dotnet test tests/FlyITA.Web.Tests/`

---

## Task 15: Full build + test verification

**Steps:**
1. `dotnet build FlyITA.Modern.sln` — zero warnings
2. `dotnet test FlyITA.Modern.sln --filter "FullyQualifiedName!~E2E"` — all pass
3. Grep for `BuildEmailBody` — should not exist in source (removed from Vacation page)
4. Verify template files exist in build output

---

## Task Order & Dependencies

```
Task 1 (interface) → Task 2 (null impl) → Task 3 (engine) → Task 4 (engine tests)
                                                              ↓
Task 5 (templates) → Task 6 (loader) → Task 7 (loader tests)
                                                              ↓
Task 8 (vacation model) ──┐                                   
Task 9 (profile model) ──┤→ Task 10 (EmailService) → Task 11 (service tests)
                          │                                    ↓
                          └── Task 12 (vacation page) → Task 13 (profile page)
                                                              ↓
                                                   Task 14 (page tests) → Task 15 (verify)
```
