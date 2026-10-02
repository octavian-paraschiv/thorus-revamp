namespace ThorusCommon.Grid;

/// <summary>
/// Represents a single immutable cell of the planetary grid, identified by its
/// grid index and the geographic coordinate of its center point.
/// </summary>
/// <param name="Index">The cell's row/column position within the grid.</param>
/// <param name="Center">The geographic coordinate of the cell's center point.</param>
public readonly record struct GridCell(GridIndex Index, GeoCoordinate Center);
