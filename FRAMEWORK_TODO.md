# WPF Framework — Pre-Release Checklist

Items to address before making this repository public.

## Must-Do Before Public Release

### Repository Configuration

- [ ] Confirm GitHub repository name: `USACE-RMC/WPF-Framework`
- [ ] Set repository visibility to public
- [ ] Configure branch protection rules for `main`
- [ ] Add repository description and topics on GitHub

### Version and Dates

- [ ] Set version number in `CITATION.cff` (currently `1.0.0` placeholder)
- [ ] Set release date in `CITATION.cff` (currently `2026-01-01` placeholder)
- [ ] Create initial git tag matching the version (e.g., `v1.0.0`)

### External Dependencies

- [ ] Confirm [Numerics](https://github.com/USACE-RMC/Numerics) NuGet package is publicly available on nuget.org
- [ ] Update Numerics HintPath references (currently point to local `C:\GIT\numerics\` paths)
- [ ] Decide distribution strategy: NuGet packages or source-only?

### Code Cleanup

- [ ] Remove legacy VB.NET project directories from `src/` (DataBase_Reader_VB, DataTable_Viewer_VB, DataViewer_Testing_VB)
- [ ] Audit for hardcoded local file paths in source code
- [ ] Review items from former `docs/REMAINING_IMPROVEMENTS.md`:
  - Complex method refactoring (PasteClipboard, ExportDataButton_Click, etc.)
  - Null-forgiving operators and event handler cleanup
- [ ] Standardize null checking patterns across codebase

### Application Links

- [ ] Verify all RMC suite GitHub URLs are correct and the repos are public:
  - https://github.com/USACE-RMC/RMC-BestFit
  - https://github.com/USACE-RMC/RMC-RFA
  - https://github.com/USACE-RMC/RMC-TotalRisk
  - https://github.com/USACE-RMC/LifeSim
- [ ] Verify Numerics URL: https://github.com/USACE-RMC/Numerics

### Documentation

- [ ] Final proof-read of README.md, CONTRIBUTING.md, and all `docs/` pages
- [ ] Confirm author list and affiliations in `CITATION.cff`
- [ ] Confirm maintainer email in `CODE_OF_CONDUCT.md`
- [ ] Build `Documentation.Tests` project to verify code snippets compile

### Test Suite

- [ ] Run full test suite: `dotnet test WPF-Framework.sln`
- [ ] Confirm known OxyPlot.Tests floating-point failures are pre-existing (not regressions)
- [ ] Address any new test failures

## Nice-to-Have Before Release

- [ ] Add screenshots to README.md (Light, Dark, Blue theme examples)
- [ ] Add GitHub Actions CI workflows (`Integration.yml`, `Snapshot.yml`, `Release.yml`)
- [ ] Create `codemeta.json` for software discovery metadata
- [ ] Add `.github/ISSUE_TEMPLATE/` with bug report and feature request forms
- [ ] Add `.github/PULL_REQUEST_TEMPLATE.md`
- [ ] Add `SECURITY.md` for GitHub security advisories
- [ ] Consider adding a `CHANGELOG.md`
- [ ] Consider publishing framework as NuGet packages
