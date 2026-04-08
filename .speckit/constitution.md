# FlyITA SpecKit Constitution

> Project-level overrides for the FlyITA platform upgrade.

## Project Context
- **Current Stack**: ASP.NET Web Forms, .NET Framework 4.8, jQuery 1.10, Bootstrap 3, IIS
- **Target Stack**: ASP.NET Core (.NET 10), modern UI framework, IIS on Azure, modern tooling
- **Goal**: Full platform tech debt upgrade — backend, frontend, and infrastructure

## Architecture Principles
- Prefer composition over inheritance
- Minimize new dependencies — justify any new package
- Data model changes require migration plans
- UI changes require wireframe/mockup approval
- Preserve all existing business logic during migration
- Maintain backward compatibility with existing ITA libraries where possible
- Each migration phase must result in a deployable state (no big-bang rewrites)

## SpecKit Pipeline (Standard)

The pipeline is strictly sequential. Every step must complete before the next begins.

```
 1. speckit.specify   → generates the spec from a GitHub issue
 2. speckit.clarify   → scans spec for issues (loops until CLEAN)
 3. speckit.review    → manual review OR auto-approve (see below)
 4. speckit.plan      → generates implementation plan from clean spec
 5. speckit.analyze   → scans plan for issues (loops until CLEAN)
 6. speckit.review    → manual review OR auto-approve (see below)
 7. speckit.tasks     → generates detailed task breakdown
 8. speckit.checklist → scans tasks for issues (loops until CLEAN)
 9. speckit.review    → manual review OR auto-approve (see below)
10. speckit.implement → executes tasks and writes code
```

### Rules
- Scan steps (clarify, analyze, checklist) loop until they return CLEAN (zero issues, including minor)
- Manual review is mandatory by default between automated steps
- When user says "go" or "auto-approve", skip all three review gates and run 1→10 straight through
- Changes during review require re-running the previous scan step
- Skipping pipeline steps (specify, clarify, plan, etc.) is never allowed
- The constitution is checked at every stage for alignment

## Quality Standards (apply to all issues)

### Performance
- HttpClientFactory for all HTTP calls (connection pooling, DNS rotation)
- Async/await throughout — CancellationToken on all async methods, no sync-over-async
- System.Text.Json for serialization (not Newtonsoft in modern app)
- No unnecessary allocations — prefer spans, string interpolation handlers where appropriate

### Resilience
- Polly retry + circuit breaker for all HTTP and external service calls
- Graceful degradation — return meaningful errors, never swallow exceptions silently
- Timeouts on all external calls (configurable via options)

### Code Quality
- Structured logging via ILogger — no string concatenation in log messages
- .NET 10 idioms — use latest language features, match existing codebase style
- Validation at system boundaries only — trust internal code and framework guarantees
- No bloat — only what the issue needs, no speculative abstractions, no gold-plating
- No placeholder or stub implementations — real code or skip entirely

### Architecture
- **Sidecar pattern** — All DLL/WCF calls stay in Legacy.Api (net48), modern app calls sidecar over HTTP
- PCentralLib.dll already contains WCF proxies — no WSDL generation or new proxy creation
- Follow existing controller patterns in Legacy.Api for new endpoints
- One PR per issue, Copilot added as reviewer

## Testing (Mandatory Guard)
- No code merges without passing tests — this is a hard gate
- Tests written alongside code, not after
- Unit tests (xUnit + Moq) for all business logic
- Integration tests (WebApplicationFactory) for APIs, middleware, auth
- UI tests (bUnit / Playwright) for component rendering and forms
- E2E tests (Playwright) for full user journeys
- Minimum 80% code coverage on business logic
- Tests must run in CI on every PR
- No skipping or ignoring tests without documented justification
- Build + all tests must pass after each task before moving to the next

## Workflow Enforcement
- One logical change per commit
- Tests accompany implementation (not after)
- No secrets, credentials, or environment-specific values in code
- Use reflection/self-review before marking work complete
