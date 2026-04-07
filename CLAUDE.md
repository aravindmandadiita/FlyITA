# FlyITA Project Instructions

## Project Overview
FlyITA is a travel/event registration platform currently built on legacy ASP.NET Web Forms (.NET Framework 4.8). The project is undergoing a full tech debt upgrade to modern .NET and modern UI.

## SpecKit Workflow

This project uses the SpecKit pipeline for all specification-to-implementation work. The pipeline is strictly sequential:

```
 1. speckit.specify   → generates the spec from a GitHub issue
 2. speckit.clarify   → scans spec for issues (loops until CLEAN)
 3. speckit.review    → manual review
 4. speckit.plan      → generates implementation plan from clean spec
 5. speckit.analyze   → scans plan for issues (loops until CLEAN)
 6. speckit.review    → manual review
 7. speckit.tasks     → generates detailed task breakdown
 8. speckit.checklist → scans tasks for issues (loops until CLEAN)
 9. speckit.review    → manual review
10. speckit.implement → executes tasks and writes code
```

### Rules
- Never skip steps
- Scan steps loop until CLEAN (zero issues)
- Manual review is mandatory between automated steps
- Changes during review require re-running the previous scan
- Check both global (`~/.speckit/constitution.md`) and project (`.speckit/constitution.md`) constitutions at every stage

## Tech Stack (Current → Target)
- **Backend**: ASP.NET Web Forms (.NET 4.8) → ASP.NET Core (.NET 10)
- **Frontend**: jQuery 1.10 + Bootstrap 3 → Modern UI framework (TBD)
- **CSS**: LESS → TBD (Tailwind, CSS Modules, etc.)
- **Hosting**: IIS → IIS on Azure (Kestrel behind IIS reverse proxy)
- **Packages**: packages.config → SDK-style NuGet
- **Testing**: Zero tests → xUnit + Moq + Playwright (mandatory gate, no merge without tests)
