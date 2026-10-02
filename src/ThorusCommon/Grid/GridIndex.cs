namespace ThorusCommon.Grid;

/// <summary>
/// Identifies a single cell within the planetary grid by its row and column position.
/// Row 0 is the northernmost row (+90 latitude edge); Column 0 is the westernmost
/// column (-180 longitude edge), matching the NW-origin, row-major convention used
/// by the .thd snapshot and static input file formats.
/// </summary>
/// <param name="Row">Zero-based row index, increasing southward (0 = north).</param>
/// <param name="Column">Zero-based column index, increasing eastward (0 = west).</param>
public readonly record struct GridIndex(int Row, int Column);
