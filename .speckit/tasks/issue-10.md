# Tasks: Phase 9 — Frontend Modernization

**Plan:** [issue-10.md](../plans/issue-10.md)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## Task 1: AnalyticsOptions + config
Add AnalyticsOptions class, register in Program.cs, add to appsettings.

## Task 2: GA4 in Layout
Add conditional GA4 script to _Layout.cshtml head section.

## Task 3: Meta tags in Layout
Add viewport (if missing), description, and robots meta tags.

## Task 4: Defer JS scripts
Add defer attribute to JS script tags in Layout.

## Task 5: CSS accessibility
Add :focus-visible styles for all interactive elements. Color contrast deferred to manual Lighthouse audit.

## Task 6: Form page accessibility audit
Verify all form inputs have labels and validation is accessible. Result: already correct from prior phases, no changes needed.

## Task 7: Frontend tests
Test GA4 conditional loading, meta tag presence, defer attributes, accessibility elements.

## Task 8: Verify all tests pass
