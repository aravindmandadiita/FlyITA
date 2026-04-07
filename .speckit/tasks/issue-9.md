# Tasks: Phase 8 — System Pages Migration

**Plan:** [issue-9.md](../plans/issue-9.md)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## Task 1: Enhance Error page

**Given** Error.cshtml exists with a generic message  
**When** I add a PageModel and differentiate 404/500  
**Then** 404 shows "page not found" and 500 shows configurable error message  
**And** non-client-facing environments show error details  

**Files:** Error.cshtml (MODIFY), Error.cshtml.cs (NEW)

---

## Task 2: ThankYou page

**Given** no ThankYou page exists  
**When** I create it with `?page` query string handling  
**Then** `?page=Vacation` shows vacation success message  
**And** default shows registration success message  

**Files:** ThankYou.cshtml (NEW), ThankYou.cshtml.cs (NEW)

---

## Task 3: Logout page

**Given** no Logout page exists  
**When** I create it with session cleanup and `?msg` handling  
**Then** session is cleared, correct message shows for each reason  
**And** `?msg=locked` includes program phone number from IPCentralDataAccess  

**Files:** Logout.cshtml (NEW), Logout.cshtml.cs (NEW)

---

## Task 4: AccessDenied page

**Given** no AccessDenied page exists  
**When** I create it  
**Then** session is cleared and access denied message displays  

**Files:** AccessDenied.cshtml (NEW), AccessDenied.cshtml.cs (NEW)

---

## Task 5: Closed page

**Given** no Closed page exists  
**When** I create it  
**Then** registration closed message displays  

**Files:** Closed.cshtml (NEW), Closed.cshtml.cs (NEW)

---

## Task 6: ImageListing stub

**Given** legacy ImageListing was fully commented out  
**When** I create a minimal API stub  
**Then** GET /api/images returns empty JSON array  

**Files:** ImageListing.cshtml.cs (NEW)

---

## Task 7: Legacy URL redirects + status code pages

**Given** no redirects exist for system pages  
**When** I add redirects and UseStatusCodePagesWithReExecute  
**Then** legacy .aspx URLs return 301 to modern paths  
**And** 404s route to /error?code=404  

**Files:** Program.cs (MODIFY)

---

## Task 8: System page tests

**Given** all system pages are implemented  
**When** I create SystemPageTests  
**Then** rendering, redirects, and query string handling are verified  

**Files:** SystemPageTests.cs (NEW)

---

## Task 9: Verify all tests pass

**Given** all changes are complete  
**When** I run dotnet test  
**Then** all existing + new tests pass, zero warnings  
