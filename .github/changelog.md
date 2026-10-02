Chenge log
----------

## Stage 1 - Scaffold solution and project structure (2026-10-01)

- Created `ThorusCommon`, `GribImporter`, `NetCdfImporter` class library projects and `ThorusSimulation` console app project under `src/`, all targeting `net10.0`.
- Created corresponding xUnit test projects (`ThorusCommonTests`, `GribImporterTests`, `NetCdfImporterTests`, `ThorusSimulationTests`) under `tests/`, pinned to xUnit 2.5.3 and Microsoft.NET.Test.Sdk 17.8.0 per project configuration.
- Wired project references per clean-architecture dependency rules: `GribImporter` and `NetCdfImporter` depend on `ThorusCommon`; `ThorusSimulation` depends on `ThorusCommon`, `GribImporter`, and `NetCdfImporter`; each test project references its corresponding `src` project.
- Added all 8 new projects to `ThorusRevamp.slnx` under `/src/` and `/tests/` solution folders.
- Verified the full solution builds successfully.

## Stage 2 - Implement ThorusCommon planetary grid domain model (2026-10-01)

- Added `GeoCoordinate` (immutable lat/lon record struct with radian conversion helpers) in `src/ThorusCommon/Grid/GeoCoordinate.cs`.
- Added `GridIndex` (immutable row/column cell identifier matching the NW-origin, row-major `.thd` convention) in `src/ThorusCommon/Grid/GridIndex.cs`.
- Added `GridCell` (immutable cell combining grid index and geographic center coordinate) in `src/ThorusCommon/Grid/GridCell.cs`.
- Added `PlanetaryGrid` (the 720x360, 0.5-degree-resolution spherical grid) in `src/ThorusCommon/Grid/PlanetaryGrid.cs`, providing cell construction with correct NW-origin coordinates, cell lookup, haversine great-circle distance, initial bearing calculation, and row-major linear-index mapping for the `.thd` binary layout.
- Removed the default template `Class1.cs` from `ThorusCommon`.
- Verified the full solution builds successfully with no compile errors.

## Stage 3 - Implement ThorusCommon .thd binary format read/write primitives (2026-10-01)

- Added `ThdParameter` (enum for all 21 snapshot weather parameters) in `src/ThorusCommon/Thd/ThdParameter.cs`.
- Added `ThdParameterCodes` (bidirectional mapping between `ThdParameter` and legacy-viewer-compatible codes T_TS, T_TL, T_TW, T_SL, T_SH, T_NL, T_NH, T_DL, T_DH, H_DD, P_00-P_03, W_00, W_10, C_00, N_00, N_DD, F_SI, L_00, A_00) in `src/ThorusCommon/Thd/ThdParameterCodes.cs`.
- Added `ThdFileNaming` (builds/parses `{parameter}_MAP_{yyyy-MM-dd}_00.thd` file names) in `src/ThorusCommon/Thd/ThdFileNaming.cs`.
- Added `ThdFile` (shared binary reader/writer for both snapshot output and static input files: 32-bit little-endian floats via `BinaryPrimitives`, row-major NW-origin order, exactly 259,200 values per file, no version header) in `src/ThorusCommon/Thd/ThdFile.cs`.
- Added `ThdFileFormatException` (typed exception for corrupt/truncated/wrong-size `.thd` files) in `src/ThorusCommon/Thd/ThdFileFormatException.cs`.
- Verified the full solution builds successfully with no compile errors.
