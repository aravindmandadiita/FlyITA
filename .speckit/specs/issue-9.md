# Spec: Phase 8 — System Pages Migration

**Issue:** [#9 — Phase 8: Page Migration — System Pages](https://github.com/aravindmandadiita/FlyITA/issues/9)  
**Epic:** [#1 — EPIC: Platform Tech Debt Upgrade](https://github.com/aravindmandadiita/FlyITA/issues/1)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## 1. Objective

Migrate 6 system/utility pages from legacy ASP.NET Web Forms to ASP.NET Core Razor Pages: ThankYou, Closed, Error (enhance existing), Logout, AccessDenied, and ImageListing. These pages handle post-action feedback, error display, session cleanup, and a legacy image API.

## 2. Background

### 2.1 Current State

Phase 7 delivered the Legacy.Api sidecar and 4 dynamic form pages. The modern project has:
- `Error.cshtml` — exists but minimal (generic "unexpected error" message, no 404/500 differentiation)
- `ErrorLoggingMiddleware` — comprehensive error logging to DB, frequency tracking, client-facing vs internal messages
- `IContextManager` / `ContextManager` — 30+ session properties, session management
- 231 tests passing

### 2.2 Legacy Pages Summary

| # | Legacy Page | Logic | Complexity |
|---|-------------|-------|------------|
| 1 | `ThankYou.aspx` | Query string `?page=Vacation` switches header/message | Low |
| 2 | `closed.aspx` | MVP pattern, reads `ClosedMessage` from presenter, DB-driven | Medium |
| 3 | `error.aspx` | `?syserror=E404` → 404 message, else → 500 message | Low |
| 4 | `logout.aspx` | `Session.Abandon()`, `?msg=locked|unknownpax|unknownprog|timeout` → reason-specific messages, program phone from PCentralProgram | Medium |
| 5 | `Accessdenied.aspx` | `Session.Abandon()` only, no message logic | Trivial |
| 6 | `ImageListing.aspx` | All code commented out, was JSON image list from CMS folder | Trivial (skip or minimal) |

### 2.3 Key Observations

- **Error.cshtml already exists** — needs enhancement, not creation
- **Logout needs PCentralProgram** — calls `PCentralProgram.GetByID()` for program phone number. This goes through `IPCentralDataAccess.GetProgramById()` → Legacy.Api sidecar
- **Closed page uses MVP** — legacy has `IClosedView` + `closedMVP` presenter. Modern version simplifies to PageModel
- **ImageListing is dead code** — all logic commented out. Create minimal stub or skip entirely
- **Session.Abandon()** in ASP.NET Core = `HttpContext.Session.Clear()` + cookie removal

## 3. Functional Requirements

### 3.1 ThankYou Page

**File:** `src/FlyITA.Web/Pages/ThankYou.cshtml` + `.cshtml.cs` (NEW)

```razor
@page "/thank-you"
```

- Reads `?page` query string
- If `page == "Vacation"`: header = "Vacation Travel Request", message = "Your vacation travel request has been submitted successfully."
- Otherwise: header = "Thank You", message = "Your registration has been completed successfully."
- Static content, no DB access, no session

### 3.2 Closed Page

**File:** `src/FlyITA.Web/Pages/Closed.cshtml` + `.cshtml.cs` (NEW)

```razor
@page "/closed"
```

- Reads registration closed message from configuration or context
- Displays a static registration closed message (hardcoded default)
- Future enhancement: read close reason from `IContextManager.RegistrationClosedType` or config
- No MVP pattern — simplified to PageModel

### 3.3 Error Page Enhancement

**File:** `src/FlyITA.Web/Pages/Error.cshtml` + `Error.cshtml.cs` (MODIFY)

```razor
@page "/error"
```

- Reads `?code` query string (or falls back to `Response.StatusCode`)
- `code == "404"` or status 404: "The page you're looking for does not exist on our server."
- Otherwise: Uses `ErrorLoggingOptions.ClientFacingErrorMessage` for client-facing, or `InternalErrorMessage` for internal
- Checks `IEnvironmentService.IsClientFacing` to choose message
- Shows error details in non-client-facing environments

### 3.4 Logout Page

**File:** `src/FlyITA.Web/Pages/Logout.cshtml` + `.cshtml.cs` (NEW)

```razor
@page "/logout"
```

- Clears session (`HttpContext.Session.Clear()`)
- Removes session cookie
- Reads `?msg` query string for reason:
  - `"locked"`: "Your account has been locked. Please contact {program phone} for assistance."
  - `"unknownpax"`: "Your login credentials are not valid for this program."
  - `"unknownprog"`: "The requested program was not found."
  - `"timeout"`: "Your session has expired due to inactivity."
  - Default (no msg): "You have been logged out successfully."
- For `"locked"` message: loads program phone via `IPCentralDataAccess.GetProgramById(programId)` before clearing session (need ProgramID from session first)

### 3.5 AccessDenied Page

**File:** `src/FlyITA.Web/Pages/AccessDenied.cshtml` + `.cshtml.cs` (NEW)

```razor
@page "/access-denied"
```

- Clears session
- Displays "Access denied. You do not have permission to access this page."
- No query string logic, no DB access

### 3.6 ImageListing Endpoint

**File:** `src/FlyITA.Web/Pages/ImageListing.cshtml.cs` (NEW — Razor Page as API endpoint)

```razor
@page "/api/images"
```

- Legacy code was fully commented out — create minimal stub
- Returns empty JSON array `[]`
- Implemented as Razor Page with `@page "/api/images"` (stub — could migrate to minimal API later)
- Placeholder for future CMS image integration

### 3.7 Legacy URL Redirects

**File:** `src/FlyITA.Web/Program.cs` (MODIFY)

```csharp
app.MapGet("/ThankYou.aspx", () => Results.Redirect("/thank-you", permanent: true));
app.MapGet("/closed.aspx", () => Results.Redirect("/closed", permanent: true));
app.MapGet("/error.aspx", () => Results.Redirect("/error", permanent: true));
app.MapGet("/logout.aspx", () => Results.Redirect("/logout", permanent: true));
app.MapGet("/Accessdenied.aspx", () => Results.Redirect("/access-denied", permanent: true));
app.MapGet("/ImageListing.aspx", () => Results.Redirect("/api/images", permanent: true));
```

### 3.8 Wire Error Page into Middleware

**File:** `src/FlyITA.Web/Program.cs` (MODIFY)

Add `UseStatusCodePagesWithReExecute` to redirect 404s to the error page:

```csharp
app.UseStatusCodePagesWithReExecute("/error", "?code={0}");
```

## 4. File Structure

```
src/FlyITA.Web/
├── Pages/
│   ├── Error.cshtml                   (MODIFY — enhance with 404/500 differentiation)
│   ├── Error.cshtml.cs                (NEW — PageModel for error handling)
│   ├── ThankYou.cshtml                (NEW)
│   ├── ThankYou.cshtml.cs             (NEW)
│   ├── Closed.cshtml                  (NEW)
│   ├── Closed.cshtml.cs               (NEW)
│   ├── Logout.cshtml                  (NEW)
│   ├── Logout.cshtml.cs               (NEW)
│   ├── AccessDenied.cshtml            (NEW)
│   ├── AccessDenied.cshtml.cs         (NEW)
│   └── ImageListing.cshtml.cs         (NEW — API stub)
├── Program.cs                         (MODIFY — redirects + UseStatusCodePagesWithReExecute)

tests/FlyITA.Web.Tests/
├── Pages/
│   └── SystemPageTests.cs             (NEW)
```

## 5. Non-Functional Requirements

### 5.1 No PII in Error Pages
- Error details shown only in non-client-facing environments
- Stack traces never exposed to end users

### 5.2 Session Cleanup
- Logout and AccessDenied must fully clear session state
- Session cookie should be expired/removed

### 5.3 Test Coverage
- Each page returns 200 with expected content
- Error page returns correct message for 404 vs 500
- Logout page handles all `?msg` values
- ThankYou page handles `?page=Vacation` and default
- Legacy URL redirects return 301
- All existing tests pass (231+)

## 6. Out of Scope

- CMS image integration (ImageListing is a stub)
- Registration closed logic from DB (Closed page reads from session/config only)
- Login/registration flow (separate phase)
- Custom error pages per program

## 7. Acceptance Criteria

1. ThankYou page renders with correct message based on `?page` parameter
2. Closed page renders registration closed message
3. Error page shows 404 message for not-found, 500 message for server errors
4. Logout page clears session and shows reason-specific message
5. AccessDenied page clears session and shows access denied message
6. ImageListing returns empty JSON array
7. Legacy .aspx URLs redirect 301 to modern paths
8. 404s route to error page via UseStatusCodePagesWithReExecute
9. All existing tests pass (no regressions)
10. New tests for all system pages

## 8. Dependencies

- Phase 7 (Legacy Sidecar + Dynamic Pages) — MERGED (PR #19)
- `IPCentralDataAccess` for program lookup in Logout page
- `IContextManager` for session management
- `ErrorLoggingOptions` for error messages

## 9. Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Closed page MVP pattern complex | Over-engineering | Simplify to PageModel, skip MVP |
| Logout needs program data before session clear | Race condition | Read ProgramID from session before clearing |
| UseStatusCodePagesWithReExecute conflicts with ErrorLoggingMiddleware | Double handling | Test interaction, ensure middleware doesn't intercept re-executed requests |
| Legacy error.aspx used `?syserror=E404/E500`, modern uses `?code=404` | Legacy bookmarks show generic error instead of 404 | Low impact — new app never generates syserror URLs. Could add syserror→code mapping if needed |
