# Thorus Revamp - Implementation Plan

This plan implements the simulation engine and supporting assemblies described in `.github/specs.md`, following the project structure and architectural guidelines defined in `.github/skills/dev-agent/project.config.md`.

The plan is organized into stages. Each stage produces a working, buildable increment. Per project configuration, code is generated one stage at a time, and the human reviews/approves a stage before the next one starts. No code outside the current stage's scope will be generated without explicit request.

## Architectural approach

- Domain-first: `ThorusCommon` holds the planetary/physics domain model, independent of any I/O, file format, or hosting technology (console app, importers). Nothing in the domain depends outward on GRIB/NetCDF/GeoTIFF libraries.
- Depend inward: `ThorusSimulation` (console host) depends on `ThorusCommon` and the importer assemblies; the importer assemblies depend only on `ThorusCommon` abstractions (interfaces/DTOs) they need to populate, not the other way around.
- Explicit boundaries: Each importer assembly (`GribImporter`, `NetCdfImporter`, `GeoTiffImporter`) exposes a narrow interface (e.g., `IAtmosphericDataSource`, `ISstDataSource`, `IGeographyDataSource`, `IAlbedoDataSource`) returning plain domain DTOs; internal parsing libraries/formats are never leaked past the assembly boundary.
- Separate read/write paths: Snapshot writing (simulation engine -> `.thd` files) and snapshot reading (future viewer) are modeled as distinct, independently testable components (`SnapshotWriter` vs. a future `SnapshotReader`), sharing only the binary format definition.
- Treat external systems as unreliable: all NOAA/USGS downloads go through a retry/timeout-aware client with typed exceptions, never crash-by-default on transient network issues (but do fail fast per spec when no cache and no download succeed).
- Observability from day one: structured logging (via `Microsoft.Extensions.Logging`) is wired in from the first stage onward, not bolted on later.
- Security by design: downloaded files are written only to a well-defined cache directory; no dynamic code execution; external URLs are allow-listed constants, not user input.
- Simplicity over purity: for a single-instance console engine, we avoid over-abstracting with full CQRS/mediator patterns; direct, well-named service classes are preferred.

## Key files / projects (from project.config.md structure)

- `ThorusRevamp.slnx` - solution file
- `src/ThorusCommon/` - planetary grid, atmospheric layer model, surface model, physics calculations, `.thd` binary format (read/write primitives), domain DTOs for imported data
- `src/GribImporter/` - GRIB parsing -> atmospheric initial conditions DTOs
- `src/NetCdfImporter/` - NetCDF parsing -> SST initial conditions DTOs
- `src/GeoTiffImporter/` - GeoTIFF parsing -> geography (soil height, land/water) and albedo DTOs
- `src/ThorusSimulation/` - console host: CLI parsing, single-instance guard, orchestration of data loading, hourly simulation loop, daily snapshot writing, stdout progress reporting, error handling
- `src/ThorusViewer/` - minimal process-control scaffold only in this plan (full viewer UI is out of scope until its own spec exists)
- `tests/*Tests/` - one xUnit test project per corresponding src project

## Risks and open questions

- Viewer UI has no spec yet; the viewer stage only scaffolds process launch/stop (PID tracking) and stdout parsing, not the actual visualization UI.
- Download sources (specific NOAA/USGS endpoint URLs and exact file formats/resolutions) need to be pinned down during the importer stages - may require sample files or API docs lookup.
- Performance targets are deferred ("to be defined after initial profiling" per spec) - the performance baseline stage establishes a baseline, not a hard number to meet.
- GRIB/NetCDF parsing will likely rely on third-party NuGet packages - license/availability needs confirmation before importer coding begins.

## Stages

1. Scaffold solution and project structure
   - Create `ThorusRevamp.slnx`, all src/* and tests/* projects per the structure in project.config.md, targeting .NET 10, with xUnit 2.5.3 test projects wired to Microsoft.NET.Test.Sdk 17.8.0.
2. Implement ThorusCommon planetary grid domain model
   - Spherical grid (720x360 cells), coordinate system, haversine distance helpers, immutable cell/grid types - no physics yet.
3. Implement ThorusCommon .thd binary snapshot format read/write primitives
   - Little-endian 32-bit float row-major writer/reader, parameter-to-filename mapping, enum ordinal encoding helpers (wind direction, precipitation type, fog level).
4. Implement GeoTiffImporter for geography and albedo data
   - Download/cache logic, GeoTIFF parsing, land/water 50% classification, max-soil-height aggregation, albedo extraction.
5. Implement NetCdfImporter for sea surface temperature data
   - Download/cache logic, NetCDF parsing, SST DTO extraction.
6. Implement GribImporter for atmospheric initial conditions
   - Download/cache logic, GRIB parsing, per-layer temperature/humidity/pressure/wind/soil-temperature DTO extraction.
7. Implement ThorusCommon atmospheric layer and surface physics models
   - Layer steering, vertical/horizontal motion, Coriolis effect, precipitation type determination (rain/snow/hail/sleet/freezing rain), snow accumulation, day/night length, min/max surface temperature calculations.
8. Implement ThorusSimulation console host: CLI, single-instance guard, orchestration
   - Parse {startDate} {durationDays} {outputFolder}, enforce named-mutex single instance, wire data loading via importer interfaces, clear output folder on start.
9. Implement ThorusSimulation hourly simulation loop and daily snapshot writing
   - Advance grid state hour by hour, write snapshots at 00:00 UTC boundaries, emit stdout progress line per step, handle numerical divergence detection and typed-exception error paths.
10. Scaffold ThorusViewer process control
    - Launch ThorusSimulation with proper CLI args, retain PID, terminate on stop request, capture/parse stdout progress lines (no visualization UI yet).
11. Write unit tests for domain model and .thd format (stages 2-3)
    - Grid construction, distance calculations, binary round-trip read/write correctness.
12. Write unit tests for importers (stages 4-6)
    - Parsing correctness against sample files, caching behavior, error handling for corrupt/missing files.
13. Write unit tests for physics models (stage 7)
    - Precipitation type determination boundary conditions, snow accumulation logic, day/night length at various latitudes/seasons.
14. Write integration tests for end-to-end simulation run (stages 8-9)
    - Short-duration run producing expected snapshot files/naming, determinism (repeat run byte-identical output), single-instance enforcement, divergence/error exit codes.
15. Establish performance baseline and update performance requirement
    - Profile a full simulated day run, record wall-clock baseline, update .github/specs.md performance section with a concrete target if warranted.
16. Update changelog.md for all completed stages
    - One entry per approved stage, per the process defined in project.config.md.
