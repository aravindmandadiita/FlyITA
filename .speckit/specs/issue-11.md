# Spec: Phase 10 — E2E Tests & Polish

**Issue:** [#11 — Phase 10: E2E Tests & Polish](https://github.com/aravindmandadiita/FlyITA/issues/11)  
**Epic:** [#1 — EPIC: Platform Tech Debt Upgrade](https://github.com/aravindmandadiita/FlyITA/issues/1)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## 1. Objective

Add comprehensive Playwright E2E tests covering all pages and critical user journeys. Clean up the duplicate test projects. This is the final phase of the platform upgrade.

## 2. Background

### 2.1 Current Test State
- 259 unit/integration tests passing (Core: 57, Infrastructure: 46, Web: 156)
- 1 E2E smoke test (HomePage_Loads_Successfully) — fails because Playwright browsers aren't installed
- Duplicate E2E projects: FlyITA.E2E.Tests (primary) and FlyITA.Tests.E2E (secondary, skipped)
- Empty test projects: FlyITA.Tests.Integration, FlyITA.Tests.Unit (1 skipped test each)

### 2.2 PlaywrightFixture
Already set up with WebApplicationFactory<Program>, headless Chromium, IAsyncLifetime pattern. Provides HttpClient, IBrowser, BaseUrl.

### 2.3 Pages to Cover (20 pages)
Static: Index, About, Contact, Privacy, International, Programs, Reservations, Resources, TravelInfo (9)
Forms: GuestProfile, TravelerProfile, VacationRequest, AchPayment (4)
System: Error, ThankYou, Logout, AccessDenied, Closed, ImageListing (6)
API: /health (1)

## 3. Functional Requirements

### 3.1 E2E Test Coverage

#### Navigation & Static Pages
- Home page loads with banner, navigation, footer
- Navigate from home to each static page via menu/footer links
- Each static page renders correct title and key content
- Legacy .aspx URLs redirect to modern paths

#### Form Pages
- Guest Profile: Load form, fill required fields, submit
- Traveler Profile: Load form, add frequent flyer entry, submit
- Vacation Request: Fill form, submit, see success message
- ACH Payment: Fill form with valid routing number, submit

#### System Pages
- Error page: /error shows generic, /error?code=404 shows not found
- ThankYou: Default and ?page=Vacation variants
- Logout: All ?msg variants show correct messages
- AccessDenied: Shows denied message
- Closed: Shows registration closed message

#### Accessibility (manual checks — axe-core integration deferred)
- ARIA landmark verification (banner, main, contentinfo)
- Single H1 per page
- Image alt text presence
- No jQuery in runtime
- Modal accessibility attributes

### 3.2 Cleanup Duplicate Test Projects

Remove the duplicate/empty test projects from disk (they were never in the .sln):
- `tests/FlyITA.Tests.E2E/` — duplicate of FlyITA.E2E.Tests
- `tests/FlyITA.Tests.Integration/` — empty
- `tests/FlyITA.Tests.Unit/` — empty

### 3.3 Consolidate E2E Tests

All E2E tests go in `tests/FlyITA.E2E.Tests/` using the existing PlaywrightFixture:
- `SmokeTests.cs` — existing (keep)
- `NavigationTests.cs` — static page navigation (NEW)
- `FormTests.cs` — form page interactions (NEW)
- `SystemPageTests.cs` — system page scenarios (NEW)
- `AccessibilityTests.cs` — axe-core scans (NEW)

## 4. File Structure

```
tests/FlyITA.E2E.Tests/
├── PlaywrightFixture.cs           (EXISTS — keep)
├── SmokeTests.cs                  (EXISTS — expand)
├── NavigationTests.cs             (NEW)
├── FormTests.cs                   (NEW)
├── SystemPageTests.cs             (NEW)
└── AccessibilityTests.cs          (NEW)
```

## 5. Non-Functional Requirements

- E2E tests require Playwright browsers installed (`pwsh bin/Debug/netX/playwright.ps1 install`)
- Tests use WebApplicationFactory — no external server needed
- Tests run headless by default
- E2E tests are separate from unit/integration tests (can be excluded via `--filter`)

## 6. Out of Scope

- Performance/load testing (no infrastructure for it)
- OWASP security scan tooling (manual review, not automated in tests)
- CI pipeline setup (separate task)
- Lighthouse score automation (manual audit)

## 7. Acceptance Criteria

1. E2E tests cover all 20 pages
2. Form submission tests verify success messages
3. System page tests verify all query string variants
4. Duplicate test projects removed from solution
5. All existing 259 unit/integration tests still pass
6. E2E tests pass when Playwright browsers are installed
