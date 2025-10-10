# KaizenLang — Improvements roadmap

This document lists prioritized, actionable improvements for the KaizenLang project with concrete file targets and estimated effort. Use this as the single-source-of-truth when preparing the final deliverable.

## Priority: P0 — Must do before submission

1. Strengthen semantic analysis (type inference + validations)
   - Files to edit: `src/KaizenLang.Core/Semantic/SemanticAnalyzer.cs`
   - Optional helper: `src/KaizenLang.Core/Semantic/TypeResolver.cs`
   - What: Infer types for standard expressions (arithmetic, boolean, function calls), validate array<T>/matrix<T> initializers when elements are expressions, and decide numeric promotion policy.
   - Estimated effort: 6–10 hours.

2. Finalize packaging and release script
   - Files: `tools/prepare_release.ps1` (new), update `README.md` and `docs/README_RELEASE.md`.
   - What: Build Release, collect exe/dlls + docs into `release/` and zip.
   - Effort: 1–2 hours.

3. Add CI for build and tests
   - Files: `.github/workflows/dotnet.yml`
   - What: Run `dotnet build` and `dotnet test` on push/PR and run unit tests under Windows runner.
   - Effort: 30–90 minutes.

## Priority: P1 — Important improvements

4. Expand unit tests (arrays, matrices, functions, input)
   - Files: add/modify under `tools/Tests/` (e.g., `RuntimeTests.cs`, extend `CollectionSemanticTests.cs`)
   - What: Add tests for ragged matrices, empty matrices, index OOB, function arity and return, input conversion, and assignment type mismatch with expressions.
   - Effort: 2–4 hours.

5. Logging & CLI `--verbose`
   - Files: `src/KaizenLang.Core/Logging/Logger.cs`, modify `tools/IDERunner/Program.cs` and `tools/CompilationTester/Program.cs` to accept `--verbose`/env vars.
   - What: Expose debug logging via CLI flag rather than printing ad-hoc Console lines.
   - Effort: 1–2 hours.

6. Docs: produce the final PDF and mapping to the grading rubric
   - Files: `docs/descripción-proyecto.md`, `docs/README_RELEASE.md`, `CHANGELOG.md`
   - What: Consolidate docs and generate `entrega-proyecto1.pdf`.
   - Effort: 1–2 hours.

## Priority: P2 — Nice to have

7. UI polish: error highlighting, theme tweaks
   - Files: `src/KaizenLang.UI/*` and `docs/theme.css`
   - Effort: 2–6 hours.

8. Create Developer Handoff notes
   - Files: `docs/DEVELOPER.md`
   - Effort: 30–60 minutes.

## Acceptance criteria (for submission)
- `dotnet build KaizenLang.sln` completes without errors.
- `dotnet test` passes all tests added in `tools/Tests`.
- `tools/prepare_release.ps1` builds a release ZIP with exe/dlls and `entrega-proyecto1.pdf`.
- `docs/` contains a short README with build/run/test instructions and rubric mapping.

---

If you want, I can start implementing items in the P0 list now. Which one prefieres que comience primero?
