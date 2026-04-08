# Spec: Email Templates — Port CustomEmails.cs Template Logic

**Issue:** #27  
**Epic:** #25 — Post-Migration: Integration Wiring & DevOps  
**Status:** CLEAN

---

## Problem Statement

The legacy `CustomEmails.cs` (815 lines) provides template-based email sending: load HTML templates from disk, replace 40+ placeholder tokens with participant/program/form data, process repeating guest blocks, and send via SMTP. The modern app has an `EmailService.cs` that is incomplete (only 9 of 40+ tokens, no guest blocks, no custom fields, templates fetched from sidecar which returns empty). The modern Vacation page sends email with hardcoded HTML instead of templates. The modern TravelerProfile page doesn't send email at all.

## Legacy Behavior (Source of Truth)

### Email Types

| Email Type | Legacy Method | Template File | Trigger | Data Source |
|-----------|--------------|---------------|---------|-------------|
| Registration Confirmation | `CustomEmails.SendRegistrationConfirmation()` | `RegistrationConfirmationEmail.html` | Registration complete | PCentralLib participant/program |
| Logon Credentials | `CustomEmails.SendLogonCredentials()` | `LogonCredentialsEmail.html` | Login created | PCentralLib participant/program |
| Forgot Password | `CustomEmails.SendForgotPasswordCredentials()` | `SeamlessLogonCredentialsEmail.html` | Password reset | PCentralLib participant/program |
| Traveler Profile (3rd party) | `CustomEmails.formatAndSendThirdPartyEmail()` | `TravelerProfiler.html` | TravelerProfile form submit | PCentralLib + form fields |
| Vacation Request | `vacation.aspx.cs.SendTravelerProfileEmail()` | `VacationTravelRequest.html` | Vacation form submit | Form fields only |

### Template Loading

Legacy loads templates from **files on disk** in `configuration/` folder:
- Path configured in `WebRegistration.config` (e.g., `configuration/TravelerProfiler.html`)
- Loaded via `Utilities.LoadFileContent()` which calls `File.OpenText(MapPath(path))`

### Token Replacement — Standard Emails

These tokens are replaced in body AND subject for standard emails (registration, logon, forgot password):

```
[PARTICIPANT_NAME], [PARTICIPANT_EMAIL], [PARTICIPANT_USERNAME], [PARTICIPANT_PASSWORD]
[FIRSTNAME], [LASTNAME]
[PROGRAM_NAME], [PROGRAM_800NBR], [PROGRAM_HQNAME], [PROGRAM_URL], [PROGRAM_EMAIL]
```

Data from: `PCentralParticipant.GetByID()`, `PCentralProgram.GetByID()` — available via sidecar.

### Token Replacement — Third-Party Emails (TravelerProfile)

In addition to standard tokens, `formatAndSendThirdPartyEmail()` also replaces:
- **Profile tokens**: `<<LegalFirstName>>`, `<<LegalMiddleName>>`, `<<LegalLastName>>`, `<<DateOfBirth>>`, `<<Gender>>`, `<<EmailAddress>>`
- **Contact tokens**: `<<BusinessPhone>>`, `<<BusinessFax>>`, `<<MobilePhone>>`, `<<HomePhone>>`
- **Travel tokens**: `<<TransportationType>>`, `<<TravelDates>>`, `<<PrefHomeDepartureTime>>`, `<<PrefDestDepartureTime>>`, `<<PrefAirline>>`, `<<PrefHomeDepartureCity>>`, `<<DriveTime>>`, `<<SeatPreference>>`, `<<AirRemarks>>`, `<<FrequentFlyerNumber>>`
- **Hotel tokens**: `<<CheckInDate>>`, `<<CheckOutDate>>`
- **Special tokens**: `<<Wheelchair>>`, `<<SpecialMeal>>`
- **Custom field tokens**: `<<CustomField.{FieldName}>>` — dynamically replaced from DB
- **Guest tokens**: `<<GLegalFirstName>>`, `<<GLegalMiddleName>>`, `<<GLegalLastName>>`, etc.

Data from: `PCentralParticipant`, `PCentralPersonContactNumber`, `PCentralAirPreference`, `PCentralProgramCustomField` — all available via sidecar endpoints.

### Token Replacement — Vacation Page (Form Fields)

`vacation.aspx.cs` does its OWN token replacement from form fields (NOT from PCentralLib):

```
[NameofPersonRequesting], [GeneralandPassengerEmail], [PhoneNumber]
[PassengerFirstName], [PassengerLastName], [BirthDate], [Gender]
[PassportNumber], [PassportExpirationDate], [Airline]
[DepartureCity], [PreferredAirline], [DestinationsInterestedIn]
[PreferredDatesofTravel], [DestinationsNotInterestedIn], [VacationDetails]
[ImportantAmenities], [RoomsNeeded]
```

Plus repeating blocks: `<passengerinfoi>...</passengerinfoi>`, `<frequentflyerti>...</frequentflyerti>`

### Token Replacement — TravelerProfile Page (Form Fields)

`Travelerprofileinformation.aspx.cs` does its OWN token replacement from form fields:

```
[TravelerFirstName], [TravelerMiddleName], [TravelerLastName], [CompanyName], [TravelerTitle]
[DeptCostCenter], [EmailAddress], [BusinessPhone], [BusinessFax], [MobilePhone], [HomePhone]
[BirthDate], [Gender], [PassportName], [PassportNumber], [PassportIssueDate], [PassportExpirationDate], [PlaceofIssue]
[TravelerArrangerName], [TravelerArrangerPhone], [TravelerArrangerEmail]
[EmergencyContactName], [EmergencyContactRelationship], [EmergencyContactPhone]
[PreferredDepartureAirport], [PreferredCarrier], [OtherPreferredCarrier], [SeatingPreference], [SpecialMealRequirements]
[SmokingPreference], [BedPreference], [SpecialRequirements]
[OtherHotelMembership], [OtherHotelMembershipNumber], [VehicleSize]
```

Plus repeating blocks: `<frequentflyerti>...</frequentflyerti>`, `<hotelclubmembershipsti>...</hotelclubmembershipsti>`, `<rentalcarmembershipsti>...</rentalcarmembershipsti>`

### Repeating Block Processing

Legacy has custom block processors that:
1. Find block markers in template (e.g., `<GuestBlockGI>...</GuestBlockGI>`)
2. Extract the template between markers
3. Loop through data collection (guests, passengers, frequent flyer entries, etc.)
4. Replace tokens in each iteration
5. Append all iterations, remove markers

### SMTP Sending

- Legacy: `PCentralEmailMessage` from `PCentralLib.email` — wraps SMTP
- Modern: `ISmtpClient` / `SmtpClientWrapper` using `System.Net.Mail` — functionally equivalent, no DLL needed

## Current Modern State

| Component | Status | Gap |
|-----------|--------|-----|
| `IEmailService` interface | 5 methods defined | Missing: `SendVacationRequestEmailAsync` |
| `EmailService.cs` | 221 lines, 5 methods | Only 9 tokens replaced; no guest blocks, no custom fields; templates from sidecar (returns empty) |
| `Vacation.cshtml.cs` | Sends email with hardcoded HTML | Should use template file like legacy |
| `TravelerProfile.cshtml.cs` | No email sending | Should send third-party email like legacy |
| Template files | Not in modern project | Need to copy from `src/FlyITA/configuration/` |
| `EmailController` (sidecar) | Returns empty stubs | Not needed — templates are files, not DB |
| `SmtpClientWrapper` | Working | OK |
| Email config (`appsettings.json`) | Template names empty | Need to set paths |

## What Needs to Happen

### 1. Copy template files to modern project

Copy from `src/FlyITA/configuration/` to `src/FlyITA.Web/Templates/`:
- `TravelerProfiler.html` — EXISTS in source
- `VacationTravelRequest.html` — EXISTS in source

**Not in source control** (program-specific, deployed per-environment on IIS):
- `RegistrationConfirmationEmail.html`
- `LogonCredentialsEmail.html`
- `SeamlessLogonCredentialsEmail.html`
- `ThirdPartyEmailTemplate.txt`

These 4 templates are NOT available to copy. The `EmailService` standard email methods (registration, logon, forgot password) will continue to use the sidecar fallback (`GetEmailTemplateAsync`). Wiring those templates is deferred to deployment configuration.

### 2. Add file-based template loading to EmailService

- Load templates from `Templates/` folder first (file I/O)
- If not found, fall back to sidecar (`GetEmailTemplateAsync`)
- Use `IWebHostEnvironment.ContentRootPath` to resolve file paths

### 3. Complete token replacement in EmailService

Port all 40+ tokens from `CustomEmails.cs`. Group by data source:
- **Participant data** — from sidecar `GetParticipantByIdAsync()`
- **Program data** — from sidecar `GetProgramByIdAsync()`
- **Contact numbers** — from sidecar `GetContactNumbersAsync()`
- **Transportation data** — from sidecar `GetTransportationDetailsAsync()`
- **Custom fields** — from sidecar `GetCustomFieldValuesAsync()`
- **Guest data** — from sidecar `GetParticipantByIdAsync()` (includes party data if available) or new endpoint

### 4. Add repeating block processor

Port the `replaceGuestBlockGI`, `replaceGuestBlockTI` logic as a generic block processor:
- Find block markers, extract template, loop through data, replace tokens per iteration

### 5. Add vacation email method using templates

- Add `SendVacationRequestEmailAsync()` to `IEmailService` / `EmailService`
- Load `VacationTravelRequest.html` template
- Accept form field values as parameters (not from PCentralLib — legacy does the same)
- Process `<passengerinfoi>` and `<frequentflyerti>` repeating blocks
- Replace all vacation-specific tokens from form data

### 6. Wire Vacation page to use template-based email

- Replace `BuildEmailBody()` hardcoded HTML with template-based approach
- Call `IEmailService.SendVacationRequestEmailAsync()` passing form field values

### 7. Add traveler profile email to TravelerProfile page

- Add `SendTravelerProfileFormEmailAsync()` to `IEmailService` / `EmailService`
- Load `TravelerProfiler.html` template
- Accept form field values as parameters
- Process repeating blocks for frequent flyer, hotel memberships, rental car
- Replace all traveler-profile-specific tokens from form data

### 8. Update EmailController stub (sidecar)

The `EmailController` returns empty — it was designed to serve templates from DB. Since the 2 available templates are file-based, this controller is **not needed** for them. Keep the stub for program-specific templates (registration, logon, seamless logon) — those will be wired per-deployment. No changes needed now.

### 9. Update config

`EmailOptions` already has template path properties (`TravelerProfileTemplate`, `VacationTravelRequestTemplate`, etc.) — currently empty strings in `appsettings.json`. Set the file paths for the 2 templates that exist:
- `TravelerProfileTemplate` → `Templates/TravelerProfiler.html`
- `VacationTravelRequestTemplate` → `Templates/VacationTravelRequest.html`

`VacationToEmail` and `VacationSubject` already exist in `EmailOptions`. Just need non-empty default values in config.

## Acceptance Criteria

1. Template files copied to `src/FlyITA.Web/Templates/`
2. `EmailService` loads templates from files (with sidecar fallback)
3. All 40+ token replacements ported from `CustomEmails.cs`
4. Repeating block processor works for guest/passenger/frequent flyer blocks
5. Vacation page sends template-based emails (not hardcoded HTML)
6. TravelerProfile page sends third-party email on submit
7. Existing `EmailService` tests updated; new tests for template loading and token replacement
8. All existing tests pass; build succeeds with zero warnings

## Out of Scope

- Runtime testing of SMTP delivery (blocked on SMTP server access)
- Program-specific templates (RegistrationConfirmation, LogonCredentials, SeamlessLogon, ThirdParty) — not in source, deployed per-environment
- Standard email methods (SendRegistrationConfirmation, SendLogonCredentials, SendForgotPassword) — they exist in EmailService with basic token replacement. Improving their token replacement is in scope, but end-to-end testing requires program-specific templates (out of scope).
- Guest block processing via sidecar — `GetParticipantByIdAsync()` returns flat dictionary, party guest data may not be included. Implement the block processor but defer runtime verification to #29.

## Risks

- **Guest/party data from sidecar** — `GetParticipantByIdAsync()` returns a flat dictionary; guest party members may not be included. Mitigation: implement the generic block processor, but guest blocks specifically may return empty until #29 verifies the sidecar data shape.
- **Template file encoding** — Legacy uses `File.OpenText()` which defaults to UTF-8. Modern should match.
