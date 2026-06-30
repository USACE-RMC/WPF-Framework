# Critical Review And Implementation Plan

Review date: 2026-05-01  
Scope: all tracked C# libraries under `src/`, excluding generated build output and `.claude/worktrees`.

## Context Notes

- No tracked or workspace `Claude.md`, `CLAUDE.md`, or `claude.md` files were found. I used `README.md`, `docs/architecture.md`, the solution file, project references, tests, and source review to orient.
- Baseline verification before changes:
  - `dotnet build WPF-Framework.sln --no-restore --configuration Debug --verbosity minimal` passed with 0 warnings and 0 errors.
  - `dotnet test WPF-Framework.sln --no-restore --configuration Debug --verbosity minimal` passed, with about 6815 tests passing and 6 skipped.
- Review focus: critical bugs only: data loss, security/destructive injection, corrupted installs, or silent data corruption.
- No implementation changes are approved yet.

## Approval Checklist

Use this section for review feedback.

- [ ] Approve all listed fixes.
- [ ] Approve only issue DB-001.
- [ ] Approve only issue DB-002.
- [ ] Approve only issue UPD-001.
- [ ] Approve only issue DB-003.
- [ ] Hold issue DB-001.
- [ ] Hold issue DB-002.
- [ ] Hold issue UPD-001.
- [ ] Hold issue DB-003.
- [ ] Revise scope or approach before implementation.

Reviewer notes:

```text

```

## Critical Findings

### DB-001: DBF table edits can permanently delete the database if replacement fails

Status: proposed  
Severity: P0 data loss  
Primary files:

- `src/DatabaseManager/DBFReader.cs:625`
- `src/DatabaseManager/DBFReader.cs:705`
- `src/DatabaseManager/DBFReader.cs:1573`
- `src/DatabaseManager/DBFReader.cs:1679`
- `src/DatabaseManager/DBFReader.cs:1794`
- `src/DatabaseManager/DBFReader.cs:1904`
- `src/DatabaseManager/DBFReader.cs:2014`
- `src/DatabaseManager/DBFReader.cs:2125`
- `src/DatabaseManager/DBFReader.cs:2233`
- `src/DatabaseManager/DBFReader.cs:2341`
- `src/DatabaseManager/DBFReader.cs:2458`
- `src/DatabaseManager/DBFReader.cs:2581`

Evidence:

Multiple DBF mutation paths create a temp DBF, then call `File.Delete(_parentDatabase.DataBasePath)` before `File.Copy(tmpdbf, _parentDatabase.DataBasePath)`. If the copy fails after deletion due to disk space, permissions, antivirus locking, process interruption, or file-system failure, the original database is gone.

Implementation plan:

1. Add a private safe-replacement helper in `DBFReader`.
2. Keep the original DBF in place until the replacement file is fully written and ready.
3. Use `File.Replace(tempPath, targetPath, backupPath, ignoreMetadataErrors: true)` when the target exists.
4. Use a same-directory move only for new targets.
5. Clean up temp and backup files in `finally`, preserving the backup if replacement fails.
6. Replace all DBF delete/copy mutation call sites with the helper.
7. Add tests around row deletion, column deletion, and column addition to confirm outputs remain valid and that failure leaves the original intact where feasible.

Acceptance criteria:

- No DBF mutation path deletes the target before a successful replacement.
- Existing DBF edit tests still pass.
- New tests cover at least one failure path and representative edit paths.

### DB-002: SQLite table and column identifiers are interpolated without safe quoting

Status: proposed  
Severity: P0 data integrity/security  
Primary files:

- `src/DatabaseManager/SQLiteManager.cs:415`
- `src/DatabaseManager/SQLiteManager.cs:576`
- `src/DatabaseManager/SQLiteManager.cs:698`
- `src/DatabaseManager/SQLiteManager.cs:846`
- `src/DatabaseManager/SQLiteManager.cs:1402`
- `src/DatabaseManager/SQLiteManager.cs:1759`
- `src/DatabaseManager/SQLiteManager.cs:1868`
- `src/DatabaseManager/SQLiteManager.cs:1902`
- `src/DatabaseManager/SQLiteManager.cs:2003`
- `src/DatabaseManager/SQLiteManager.cs:2531`
- `src/DatabaseManager/DatabaseManager.cs:237`

Evidence:

SQLite identifiers are assembled as `[` + name + `]` in many schema and update statements. Names are accepted from external sources such as CSV headers and table names. A name containing `]`, quotes, reserved words, or SQL punctuation can break SQL, target the wrong object, or potentially execute destructive extra statements through `ExecuteNonQuery`. Some write paths also derive parameter names directly from user column names, which fails for spaces and special characters.

Implementation plan:

1. Add a central `QuoteIdentifier(string identifier)` helper using SQLite double-quoted identifiers and escaped embedded quotes.
2. Validate null or empty identifiers explicitly.
3. Replace direct bracket concatenation for table names, temp table names, and column names.
4. Stop generating parameter names from column names; use stable generated names such as `@p0`, `@p1`, `@cellValue`, and `@rowId`.
5. Prefer parameterized `rowid` predicates where practical, even though current `rowid` sources are internal.
6. Add tests for names with spaces, dashes, reserved words, `]`, `"`, and injection-like text across create, save, append, edit, read, add-column, and delete-column paths.

Acceptance criteria:

- SQLite schema and data operations work with quoted/special table and column names.
- Injection-like names are stored as identifiers, not interpreted as SQL.
- Existing SQLite tests still pass.

### UPD-001: Updater can leave a mixed-version installation after partial extraction

Status: proposed  
Severity: P1 install integrity/data loss  
Primary files:

- `src/SoftwareUpdate.Updater/InstallationManager.cs:131`
- `src/SoftwareUpdate.Updater/InstallationManager.cs:181`
- `src/SoftwareUpdate.Updater/InstallationManager.cs:199`
- `src/SoftwareUpdate.Updater/InstallationManager.cs:246`
- `src/SoftwareUpdate.Updater/InstallationManager.cs:285`

Evidence:

The updater backs up the target directory, then extracts files directly into the live target. If extraction fails after writing new files, rollback copies backup files over the target but does not remove newly-created files from the failed package. A successful update also does not remove stale files that disappeared from the new package. This can leave assemblies/resources from different versions side by side. There is also a related root-folder stripping edge case: root detection accepts `/` and `\`, but stripping uses only `/`.

Implementation plan:

1. Extract and validate the update archive into a staging directory, not the live target.
2. Normalize archive separators before root-folder stripping and path traversal checks.
3. Validate every entry path before copying anything into the target.
4. Apply the staged update in a controlled replace step: backup target, clear intended target contents, copy staging contents in.
5. On failure, restore from backup and remove files that were not present in the backup.
6. Add tests for partial extraction failure, new-file rollback, stale-file cleanup, traversal protection, backup-directory skipping, and backslash root-folder packages.

Acceptance criteria:

- A failed update restores the exact previous target contents, except intentional updater/log exclusions if documented.
- A successful update does not leave stale files from the previous version.
- Existing updater tests still pass.

### DB-003: DBF export can filter the wrong columns and delete an existing export before a successful write

Status: proposed  
Severity: P1 data export integrity  
Primary file:

- `src/DatabaseManager/DataTableView.cs:2688`
- `src/DatabaseManager/DataTableView.cs:2725`
- `src/DatabaseManager/DataTableView.cs:2728`

Evidence:

`ExportToDBF` filters selected columns by checking `_columnTypes[i]` instead of `_columnTypes[columnIndicesToExport[i]]`. When exporting a subset or reordered list of columns, the method can include unsupported fields or omit valid fields. The method also reads `rowIndicesToExport[0]`, which crashes for an empty selection, and deletes an existing output file before the new DBF is successfully created.

Implementation plan:

1. Change the filter to inspect `_columnTypes[columnIndicesToExport[i]]`.
2. Define empty-row export behavior and test it. Preferred behavior: create a valid DBF schema with zero rows.
3. Write export output to a temp file in the destination directory.
4. Replace the requested output only after the temp DBF is successfully created and validated.
5. Add tests for subset export, reordered export, unsupported skipped types, empty row export, and failed export preserving an existing target where feasible.

Acceptance criteria:

- Exported DBF columns match the requested supported columns.
- Empty row selections do not crash.
- Existing export targets are preserved if export creation fails.

## Reviewed With No Critical Findings

No critical issues crossed the review threshold in these areas during this pass:

- `FrameworkInterfaces`
- `FrameworkUI`
- `Themes`
- `DAG` and `DAGControls`
- `ExpressionParser` and `ExpressionParserControls`
- `DatabaseControls`
- `GenericControls`
- `NumericControls`
- `OxyPlotControls`
- Vendored `AvalonDock`
- Vendored `OxyPlot`

Non-critical issues were intentionally not added to this plan.

## Proposed Execution Order

1. Implement DB-001 first because it is the highest direct data-loss risk.
2. Implement DB-002 next because it touches many SQLite paths and should be tested broadly.
3. Implement UPD-001 after database fixes because it changes update transaction semantics.
4. Implement DB-003 last because it is narrower and mostly isolated to export behavior.
5. Run targeted tests after each issue, then full `dotnet build` and `dotnet test` at the end.

## Final Verification Plan

After approved implementation:

1. Run focused database tests.
2. Run focused updater tests.
3. Run `dotnet build WPF-Framework.sln --no-restore --configuration Debug --verbosity minimal`.
4. Run `dotnet test WPF-Framework.sln --no-restore --configuration Debug --verbosity minimal`.
5. Summarize changed files, risk reduction, and any remaining test gaps.
