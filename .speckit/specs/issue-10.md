# Spec: Phase 9 — Frontend Modernization

**Issue:** [#10 — Phase 9: Frontend Modernization](https://github.com/aravindmandadiita/FlyITA/issues/10)  
**Epic:** [#1 — EPIC: Platform Tech Debt Upgrade](https://github.com/aravindmandadiita/FlyITA/issues/1)  
**Status:** CLEAN  
**Generated:** 2026-04-07  

---

## 1. Objective

Complete the frontend modernization by adding Google Analytics 4, hardening WCAG 2.1 AA accessibility, optimizing for Lighthouse 90+ scores, and adding component tests. Most of the heavy lifting (jQuery removal, Bootstrap 5 migration, vanilla JS rewrite, CSS cleanup) was completed in Phases 5-8.

## 2. Background

### 2.1 Already Completed (Phases 5-8)

| Item | Status | Details |
|------|--------|---------|
| jQuery removal | DONE | Zero jQuery references. All JS is vanilla ES6+ |
| Bootstrap 5 migration | DONE | Bootstrap 5.3.3 via LibMan, BS5 classes throughout |
| LESS removal | DONE | Plain CSS (theme.css, main.css, print.css) |
| Modern bundling | DONE | WebOptimizer (minify in prod, passthrough in dev) |
| Itinerary modal | DONE | Bootstrap 5 modal API, vanilla JS |
| Vanilla JS rewrite | DONE | site.js (148 lines), navigation.js (86 lines), theme.js (96 lines) |
| Semantic HTML | DONE | ARIA roles, data-bs-* attributes, skip-to-content |
| Font Awesome 6 | DONE | FA 6.7.2 via LibMan |

### 2.2 Remaining Work

| Item | Status | Priority |
|------|--------|----------|
| Google Analytics 4 | NOT STARTED | P0 |
| WCAG 2.1 AA audit & fixes | PARTIAL | P0 |
| Meta tags & SEO | NOT STARTED | P1 |
| Lighthouse optimization | NOT STARTED | P1 |
| Component/rendering tests | PARTIAL (page-level tests exist) | P1 |

### 2.3 Current Frontend Stack

- **CSS:** Bootstrap 5.3.3, FontAwesome 6.7.2, theme.css (1,068 lines), main.css (120 lines), print.css (111 lines)
- **JS:** site.js, navigation.js, theme.js — all vanilla ES6+, no frameworks
- **Bundling:** WebOptimizer (LigerShark.WebOptimizer.Core v3.*)
- **Libs:** LibMan (cdnjs) — only Bootstrap + FontAwesome

## 3. Functional Requirements

### 3.1 Google Analytics 4

**File:** `src/FlyITA.Web/Pages/_Layout.cshtml` (MODIFY)

Add GA4 tracking script in `<head>`, configurable via options:

```html
<!-- GA4 — only in client-facing environments -->
@if (analyticsEnabled)
{
    <script async src="https://www.googletagmanager.com/gtag/js?id=@measurementId"></script>
    <script>
        window.dataLayer = window.dataLayer || [];
        function gtag(){dataLayer.push(arguments);}
        gtag('js', new Date());
        gtag('config', '@measurementId');
    </script>
}
```

**File:** `src/FlyITA.Core/Options/AnalyticsOptions.cs` (NEW)

```csharp
public class AnalyticsOptions
{
    public const string SectionName = "Analytics";
    public bool Enabled { get; set; } = false;
    public string MeasurementId { get; set; } = "";
}
```

- GA4 only loads when `Analytics:Enabled == true` AND environment is client-facing
- MeasurementId configurable per environment (G-XXXXXXXXXX)
- No tracking in development/staging

### 3.2 Meta Tags & SEO

**File:** `src/FlyITA.Web/Pages/_Layout.cshtml` (MODIFY)

Add standard meta tags:

```html
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<meta name="description" content="@(ViewData["Description"] ?? "International Travel Associates — Corporate and leisure travel management")">
<meta name="robots" content="@(ViewData["Robots"] ?? "index, follow")">
<link rel="canonical" href="@(ViewData["Canonical"] ?? Context.Request.Path)">
```

Check if viewport meta already exists. If so, just add description/robots/canonical.

### 3.3 WCAG 2.1 AA Accessibility Hardening

**Files:** Multiple pages and components

Audit and fix:

1. **Color contrast** — Ensure all text meets 4.5:1 ratio (normal text) or 3:1 (large text)
2. **Focus indicators** — Visible focus rings on all interactive elements
3. **Form labels** — All form inputs have associated `<label>` elements (check GuestProfile, TravelerProfile, VacationRequest, AchPayment)
4. **Image alt text** — All `<img>` tags have descriptive `alt` attributes
5. **Heading hierarchy** — Verify h1 → h2 → h3 progression on each page (no skipped levels)
6. **Link purpose** — Ensure all links have descriptive text (no bare "click here")
7. **Error identification** — Form validation errors are announced to screen readers (aria-live)

**File:** `src/FlyITA.Web/wwwroot/css/theme.css` (MODIFY)

Add focus styles:
```css
:focus-visible {
    outline: 2px solid #005d95;
    outline-offset: 2px;
}
```

### 3.4 Lighthouse Optimization

**File:** `src/FlyITA.Web/Pages/_Layout.cshtml` (MODIFY)

1. **Preconnect** to external origins (if any CDN resources)
2. **Font display** — Add `font-display: swap` for FontAwesome
3. **Defer non-critical JS** — Add `defer` attribute to JS scripts
4. **Async GA4 script** — Already async in 3.1

**File:** `src/FlyITA.Web/wwwroot/css/theme.css` (MODIFY)

1. **Remove unused CSS rules** if any (dead selectors)
2. **Ensure print.css is media="print"** — already done if using separate `<link media="print">`

### 3.5 Component Rendering Tests

**File:** `tests/FlyITA.Web.Tests/Components/` (existing tests — verify coverage)

Ensure View Component tests cover:
- MainMenu renders navigation links with ARIA attributes
- Banner renders with correct parallax markup
- ItineraryModal renders with BS5 modal structure
- CustomField renders form inputs with labels

## 4. File Structure

```
src/FlyITA.Core/
└── Options/
    └── AnalyticsOptions.cs              (NEW)

src/FlyITA.Web/
├── Pages/
│   └── _Layout.cshtml                   (MODIFY — GA4, meta tags, defer JS)
├── wwwroot/css/
│   └── theme.css                        (MODIFY — focus styles, contrast fixes)
├── Program.cs                           (MODIFY — register AnalyticsOptions)
├── appsettings.json                     (MODIFY — add Analytics section)
└── appsettings.Development.json         (MODIFY — Analytics disabled)

tests/FlyITA.Web.Tests/
├── Pages/
│   └── FrontendTests.cs                 (NEW — GA4, meta tags, accessibility)
```

## 5. Non-Functional Requirements

### 5.1 Performance
- Lighthouse Performance score target: 90+
- No render-blocking resources (JS deferred, CSS above fold)

### 5.2 Accessibility
- WCAG 2.1 AA compliance
- Lighthouse Accessibility score target: 90+

### 5.3 Privacy
- GA4 only in production/client-facing environments
- No tracking in dev/staging
- No PII in analytics events

## 6. Out of Scope

- SPA framework migration (React, Vue, Blazor) — current vanilla JS is sufficient
- Tailwind CSS — Bootstrap 5 is working well, no benefit to switching
- Vite/webpack — WebOptimizer is sufficient for current needs
- Custom analytics events (just pageview tracking for now)

## 7. Acceptance Criteria

1. GA4 loads only in client-facing environments when enabled
2. All pages have viewport, description, robots meta tags
3. Focus indicators visible on all interactive elements
4. Color contrast meets WCAG 2.1 AA (4.5:1 minimum)
5. All form inputs have associated labels
6. JS loaded with defer attribute
7. All existing tests pass (255+)
8. New tests for GA4 conditional loading and meta tags

## 8. Dependencies

- Phase 8 (System Pages) — MERGED (PR #20)

## 9. Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|-----------|
| CSS contrast changes break branding | Visual regression | Minimal changes, only adjust where below 4.5:1 |
| GA4 script slows page load | Lighthouse score drop | Script is async, only in prod |
| defer on JS breaks functionality | JS errors | Test all interactive features after deferring |
