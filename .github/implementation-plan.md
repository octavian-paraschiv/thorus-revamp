# Thorus Revamp - Implementation Plan

This plan implements the simulation engine and supporting assemblies described in `.github/specs.md`, following the project structure and architectural guidelines defined in `.github/skills/dev-agent/project.config.md`.

The plan is organized into stages. Each stage produces a working, buildable increment. Per project configuration, code is generated one stage at a time, and the human reviews/approves a stage before the next one starts. No code outside the current stage's scope will be generated without explicit request.

`ThorusViewer` is a separate, already-existing application that will be adapted later to point at this engine; it is explicitly out of scope for this plan. Geography (elevation, land/water) and albedo are supplied as pre-made `.thd` files at simulation-grid resolution, read directly via `ThorusCommon`'s `.thd` reader.

## Architectural approach

- Domain-first: `ThorusCommon` holds the planetary/physics domain model, independent of any I/O, file format, or hosting technology (console app, importers). Nothing in the domain depends outward on GRIB/NetCDF libraries.
- Depend inward: `ThorusSimulation` (console host) depends on `ThorusCommon` and the importer assemblies; the importer assemblies depend only on `ThorusCommon` abstractions (interfaces/DTOs) they need to populate, not the other way around.
- Explicit boundaries: Each importer assembly (`GribImporter`, `NetCdfImporter`) exposes a narrow interface (e.g., `IAtmosphericDataSource`, `ISstDataSource`) returning plain domain DTOs already downscaled to the 720x360 simulation grid; internal parsing libraries/formats are never leaked past the assembly boundary.
- Separate read/write paths: Snapshot *writing* (simulation engine -> `.thd` files) and the pre-made static `.thd` input *reading* (elevation, land/water mask, albedo) share the same binary format primitives in `ThorusCommon`, but are exercised through distinct reader/writer components.
- Treat external systems as unreliable: all NOAA downloads go through a retry/timeout-aware client with typed exceptions, never crash-by-default on transient network issues (but do fail fast per spec when no cache and no download succeed).
- Observability from day one: all log/diagnostic output (startup info, warnings, errors, divergence details) is written via `Console.Write`/`Console.WriteLine` to stdout, in the same stream as the per-step progress line, so that `ThorusViewer` can intercept and display it in its UI; no separate log file or logging framework is introduced.
- Security by design: downloaded files (GRIB/NetCDF cache) and output snapshot files are written only under the `ThorusWorkFolder` environment variable's directory (downloads directly under it, snapshots under its `data` subfolder); no dynamic code execution; external URLs are allow-listed constants, not user input.
- Simplicity over purity: for a single-instance console engine, we avoid over-abstracting with full CQRS/mediator patterns; direct, well-named service classes are preferred.

## Key files / projects

- `ThorusRevamp.slnx` - solution file
- `src/ThorusCommon/` - planetary grid, atmospheric layer model, surface model, physics calculations, `.thd` binary format (read/write primitives), domain DTOs for imported data
- `src/GribImporter/` - GRIB parsing (all atmospheric levels) -> atmospheric initial condition DTOs, downscaled to the 720x360 grid
- `src/NetCdfImporter/` - NetCDF parsing (SST) -> SST initial condition DTOs, downscaled to the 720x360 grid
- `src/ThorusSimulation/` - console host: CLI parsing (`{startDate} {durationDays} [-clearInitialData]`), reading the `ThorusWorkFolder` environment variable, single-instance guard, orchestration of data loading (GRIB/NetCDF downloads + static `.thd` reads), hourly simulation loop, daily snapshot writing to `%ThorusWorkFolder%\data`, stdout progress/diagnostic reporting, fatal-error handling (all via stdout, no stderr)
- `tests/*Tests/` - one xUnit test project per corresponding src project (`ThorusCommonTests`, `GribImporterTests`, `NetCdfImporterTests`, `ThorusSimulationTests`)

`ThorusViewer` and `ThorusViewerTests` are not created by this plan.

## Risks and open questions

- Download sources (exact NOAA endpoint behavior/availability over time) need to be validated against the provided samples (`input.grib`, `SST.nc`) during the importer stages.
- Performance targets are deferred ("to be defined after initial profiling" per spec) - the performance baseline stage establishes a baseline, not a hard number to meet.
- GRIB/NetCDF parsing will likely rely on third-party NuGet packages; any open-source package licensed under Apache 2.0, LGPL, or MIT may be used freely.
- Downscaling algorithm (e.g., bilinear interpolation vs. nearest-neighbor) for higher-resolution GRIB/NetCDF sources is not specified; a reasonable default (bilinear for continuous fields) will be proposed during importer implementation and flagged for review.
- The "normal" (climatological) temperature formula is specified only as "simplified, based on latitude/season/altitude" with no exact formula; a concrete formula will be proposed during the physics-model stage and flagged for review before being finalized.

## Stages

1. [COMPLETED] Scaffold solution and project structure
   - Create `ThorusRevamp.slnx` and the `src/ThorusCommon`, `src/GribImporter`, `src/NetCdfImporter`, `src/ThorusSimulation` projects plus their corresponding `tests/*Tests` xUnit projects, targeting .NET 10, with xUnit 2.5.3 wired to Microsoft.NET.Test.Sdk 17.8.0.
2. [COMPLETED] Implement ThorusCommon planetary grid domain model
   - Spherical grid (720x360 cells), coordinate system, haversine distance helpers, immutable cell/grid types - no physics yet.
3. [COMPLETED] Implement ThorusCommon .thd binary format read/write primitives
   - Little-endian 32-bit float row-major reader and writer, shared by both snapshot output and static input files (elevation, land/water mask, albedo); parameter-to-filename mapping per the `{parameter}_MAP_{yyyy-MM-dd}_00.thd` convention and the old-viewer-compatible parameter codes (T_TS, T_TL, T_TW, T_SL, T_SH, T_NL, T_NH, T_DL, T_DH, H_DD, P_00-P_03, W_00, W_10, C_00, N_00, N_DD, F_SI, L_00, A_00).
4. Implement ThorusCommon static input readers
   - Load `elevationMap.thd`, `LandWaterMask.thd`, and `albedo.thd` from the `data` folder located next to the `ThorusSimulation.exe` binary (i.e., the executable's own directory, not the current working directory) at startup, using the Stage 3 reader.
5. Implement NetCdfImporter for sea surface temperature data
   - Download/cache logic (URL templated by simulation start date), NetCDF parsing, downscaling to the 720x360 grid, SST DTO extraction. The SST file is the leading/authoritative source for the simulation's actual start date: when freshly downloaded, its embedded date becomes the simulation's actual start date (per the nearest-available-dataset rule); when reused from cache (no `-clearInitialData`), its embedded date overrides the requested `{startDate}` CLI argument.
6. Implement GribImporter for atmospheric initial conditions
   - Download/cache logic (URL templated by the SST-resolved date/time, not independently by the requested `{startDate}`, to keep both sources in sync), GRIB parsing for all 4 atmospheric levels, downscaling to the 720x360 grid, per-layer temperature/humidity/pressure/wind DTO extraction.
7. Implement ThorusCommon atmospheric layer and surface physics models
   - Layer steering, vertical/horizontal motion, Coriolis effect, precipitation type determination (rain/snow/hail/sleet/freezing rain), precipitation amount encoding (type-offset scheme), snow accumulation, day/night length, min/max actual surface temperature, simplified normal (climatological) min/max temperature formula, and actual-to-normal deltas.
8. Implement ThorusSimulation console host: CLI, single-instance guard, orchestration
   - Parse `{startDate} {durationDays} [-clearInitialData]`, read the `ThorusWorkFolder` environment variable to locate the work folder (fatal stdout error + non-zero exit at startup if unset/empty/inaccessible), enforce named-mutex single instance, wire static `.thd` reads (from the exe-adjacent `data` folder) and GRIB/NetCDF data loading: download/reuse SST first to resolve the actual simulation start date (ignoring the `{startDate}` argument when cached files exist and `-clearInitialData` is absent), then download/reuse GRIB for that same resolved date; re-download both when `-clearInitialData` is passed. Always clear the work folder's `data` subfolder (snapshots) on start, independent of the `-clearInitialData` flag.
9. Implement ThorusSimulation hourly simulation loop and daily snapshot writing
   - Advance grid state hour by hour, write all 19 snapshot parameter files to `%ThorusWorkFolder%\data` at 00:00 UTC boundaries, emit the stdout progress line per step, handle numerical divergence detection and typed-exception error paths by emitting a clearly flagged fatal message to stdout and exiting with a non-zero code.
10. Write unit tests for domain model and .thd format (stages 2-4)
    - Grid construction, distance calculations, binary round-trip read/write correctness for both snapshot and static input files.
11. Write unit tests for importers (stages 5-6)
    - Parsing correctness against the provided sample files (`SST.nc`, `input.grib`), downscaling correctness, caching behavior, error handling for corrupt/missing files, SST-leading-date resolution (cached vs. fresh download), and GRIB date synchronization with the resolved SST date.
12. Write unit tests for physics models (stage 7)
    - Precipitation type determination boundary conditions, precipitation amount/type encoding round-trip, snow accumulation logic, day/night length at various latitudes/seasons, normal-temperature formula sanity checks.
13. Write integration tests for end-to-end simulation run (stages 8-9)
    - Short-duration run producing expected snapshot files/naming, determinism (repeat run byte-identical output), single-instance enforcement, divergence/error exit codes.
14. Establish performance baseline and update performance requirement
    - Profile a full simulated day run, record wall-clock baseline, update `.github/specs.md` performance section with a concrete target if warranted.
15. Update changelog.md for all completed stages
    - One entry per approved stage, per the process defined in project.config.md.
