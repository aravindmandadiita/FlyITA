# Plan: Phase 8 — System Pages Migration

**Spec:** [issue-9.md](../specs/issue-9.md)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## Implementation Order

All pages are independent — can be implemented in any order. Error page is modified (exists), all others are new.

### Step 1: Enhance Error page (MODIFY existing)
- Add `Error.cshtml.cs` PageModel
- Read `?code` query string or `Response.StatusCode`
- Differentiate 404 vs 500 messages using `ErrorLoggingOptions`
- Check `IEnvironmentService.IsClientFacing` for detail level

### Step 2: ThankYou page (NEW)
- Simple query string `?page` check, no dependencies

### Step 3: Logout page (NEW)
- Read ProgramID from `IContextManager` before clearing session
- Load program phone via `IPCentralDataAccess.GetProgramById()`
- Map `?msg` to reason-specific messages
- Clear session + cookie

### Step 4: AccessDenied page (NEW)
- Trivial — clear session, show message

### Step 5: Closed page (NEW)
- Read close reason from config/session, display message

### Step 6: ImageListing stub (NEW)
- Minimal API endpoint returning empty JSON array

### Step 7: Legacy URL redirects + UseStatusCodePagesWithReExecute
- Add 6 redirects to Program.cs
- Add `app.UseStatusCodePagesWithReExecute("/error", "?code={0}")`

### Step 8: Tests
- SystemPageTests.cs — rendering, redirects, query string handling

### Step 9: Verify all tests pass
