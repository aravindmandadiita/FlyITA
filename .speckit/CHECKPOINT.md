# FlyITA Upgrade — Checkpoint

**Date:** 2026-04-08  
**Last completed:** Epic #25 — 8 of 9 stories done  
**Remaining:** #29 (DLL Runtime Testing) — blocked on dev environment

## Epic #25 Progress

| Story | Issue | Status | PR |
|-------|-------|--------|-----|
| ACH Payment | #26 | ✅ MERGED | #35 |
| Email Templates | #27 | ✅ MERGED | #36 |
| Page Configuration | #28 | ✅ CLOSED (already works) | — |
| DLL Runtime Testing | #29 | ⬜ BLOCKED | — |
| CI Pipeline | #30 | ✅ MERGED | #37 |
| Playwright in CI | #31 | ✅ MERGED | #38 |
| axe-core a11y | #32 | ✅ MERGED | #39 |
| Color contrast | #33 | ✅ MERGED | #39 |
| CSS dead code | #34 | ✅ MERGED | #39 |

## Test Count: 285 non-E2E (72 core + 48 infra + 165 web) + 43 E2E

## What Exists

### From Epic #1 (Phases 1-10)
- Full .NET 10 Razor Pages app (20 pages migrated from Web Forms)
- Core services, Infrastructure data access, Legacy.Api sidecar
- SAML auth, error logging middleware, environment service
- Bootstrap 5.3.3, vanilla ES6+, zero jQuery

### From Epic #25
- **#26:** PaymentsController in Legacy.Api (POST /api/payments/ach → 501 placeholder), LegacyApiCardProcessService (HTTP + Polly), AchPayment page wired
- **#27:** EmailTemplateEngine (40+ tokens, repeating blocks), FileEmailTemplateLoader (file-first, sidecar-fallback), Vacation + TravelerProfile template emails
- **#28:** PageConfigurationService already calls spWRASelPageConfiguration via direct SQL
- **#30:** azure-pipelines.yml (build + test + deploy Dev/Test/Prod), GitHub Actions ci.yml
- **#31:** Playwright browser install + E2E tests in both CI pipelines
- **#32-34:** axe-core a11y tests (18 pages), color contrast tests (7 pages), CSS dead code removed, PurgeCSS config

## Infrastructure
- **CI/CD:** azure-pipelines.yml → BUILDAGENTS pool, .NET 10 SDK
- **Deploy:** WebFileSystemPublish to \\itagroup\web\{env}\publish\{env}.flyita.com
- **SMTP:** mailxfer.itagroup.com:25, no SSL
- **Connection strings:** injected on IIS per environment (not in source)
- **Legacy pipeline:** Azure DevOps release #197 (MSBuild + pubxml, still active for legacy app)

## What #29 Needs (to close Epic #25)
1. Dev server with PCentralLib.dll, ITALib.dll, ITAErrorLogging.dll
2. Database access (PerformanceCentral, ITAEnterprise, WebRegCustom, WebRegAdmin)
3. WCF endpoint access (itavault.int for CardProcessService)
4. Wire real PCentralLib method in PaymentsController (currently returns 501)
5. Verify all Legacy.Api sidecar endpoints return real data

## To Resume
1. Read this checkpoint + MEMORY.md
2. If dev env available: go 29
3. If not: Epic #25 is functionally complete, configure Azure DevOps pipeline

## Run the App
```
cd src/FlyITA.Web
dotnet run --urls "http://localhost:5050"
```

## Repo Links
- **GitHub:** https://github.com/aravindmandadiita/FlyITA
- **Epic #25:** https://github.com/aravindmandadiita/FlyITA/issues/25
- **Azure DevOps:** https://dev.azure.com/itagroup/Events/_git/FlyITA
