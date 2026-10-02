namespace ThorusCommon.Grid;

/// <summary>
/// Represents the spherical planetary grid used by the simulation engine: a fixed
/// 720 (columns, longitude) x 360 (rows, latitude) grid at 0.5 degree resolution,
/// covering the full surface of the Earth. The grid is immutable once constructed;
/// it only describes geometry (cell positions and spherical distance helpers), not
/// any physical/weather state.
/// </summary>
public sealed class PlanetaryGrid
{
    /// <summary>Mean radius of the Earth, in kilometers.</summary>
    public const double EarthRadiusKm = 6371.0;

    /// <summary>Angular resolution of the grid, in degrees, for both latitude and longitude.</summary>
    public const double ResolutionDegrees = 0.5;

    /// <summary>Number of columns in the grid (longitude axis, -180 to +180).</summary>
    public const int Columns = 720;

    /// <summary>Number of rows in the grid (latitude axis, +90 to -90).</summary>
    public const int Rows = 360;

    /// <summary>Total number of cells in the grid.</summary>
    public const int CellCount = Columns * Rows;

    private readonly GridCell[] _cells;

    public PlanetaryGrid()
    {
        _cells = new GridCell[CellCount];

        for (var row = 0; row < Rows; row++)
        {
            // Row 0 is the northernmost row; cell center is offset half a cell south of the north edge.
            var latitude = 90.0 - (row + 0.5) * ResolutionDegrees;

            for (var column = 0; column < Columns; column++)
            {
                // Column 0 is the westernmost column; cell center is offset half a cell east of the west edge.
                var longitude = -180.0 + (column + 0.5) * ResolutionDegrees;

                var index = new GridIndex(row, column);
                var center = new GeoCoordinate(latitude, longitude);
                _cells[ToLinearIndex(index)] = new GridCell(index, center);
            }
        }
    }

    /// <summary>Gets the cell at the given row/column position.</summary>
    public GridCell GetCell(GridIndex index)
    {
        ValidateIndex(index);
        return _cells[ToLinearIndex(index)];
    }

    /// <summary>Gets the cell at the given row/column position.</summary>
    public GridCell GetCell(int row, int column) => GetCell(new GridIndex(row, column));

    /// <summary>Gets all cells in row-major order (matching the .thd binary file layout).</summary>
    public IReadOnlyList<GridCell> Cells => _cells;

    /// <summary>
    /// Computes the great-circle distance, in kilometers, between two geographic
    /// coordinates using the haversine formula.
    /// </summary>
    public static double HaversineDistanceKm(GeoCoordinate from, GeoCoordinate to)
    {
        var deltaLat = to.LatitudeRadians - from.LatitudeRadians;
        var deltaLon = to.LongitudeRadians - from.LongitudeRadians;

        var sinHalfDeltaLat = Math.Sin(deltaLat / 2.0);
        var sinHalfDeltaLon = Math.Sin(deltaLon / 2.0);

        var a = sinHalfDeltaLat * sinHalfDeltaLat +
                Math.Cos(from.LatitudeRadians) * Math.Cos(to.LatitudeRadians) *
                sinHalfDeltaLon * sinHalfDeltaLon;

        var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Computes the great-circle distance, in kilometers, between the centers of two grid cells.
    /// </summary>
    public double DistanceKm(GridIndex from, GridIndex to) =>
        HaversineDistanceKm(GetCell(from).Center, GetCell(to).Center);

    /// <summary>
    /// Computes the initial bearing, in degrees (0 = north, 90 = east, clockwise),
    /// for traveling along the great circle from one coordinate to another.
    /// </summary>
    public static double InitialBearingDegrees(GeoCoordinate from, GeoCoordinate to)
    {
        var deltaLon = to.LongitudeRadians - from.LongitudeRadians;

        var y = Math.Sin(deltaLon) * Math.Cos(to.LatitudeRadians);
        var x = Math.Cos(from.LatitudeRadians) * Math.Sin(to.LatitudeRadians) -
                Math.Sin(from.LatitudeRadians) * Math.Cos(to.LatitudeRadians) * Math.Cos(deltaLon);

        var bearingRadians = Math.Atan2(y, x);
        var bearingDegrees = bearingRadians * 180.0 / Math.PI;

        return (bearingDegrees + 360.0) % 360.0;
    }

    /// <summary>
    /// Converts a row/column grid index into the linear (row-major, NW-origin) index
    /// used by the .thd binary file layout.
    /// </summary>
    public static int ToLinearIndex(GridIndex index) => index.Row * Columns + index.Column;

    private static void ValidateIndex(GridIndex index)
    {
        if (index.Row < 0 || index.Row >= Rows)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index.Row,
                $"Row must be between 0 and {Rows - 1}.");
        }

        if (index.Column < 0 || index.Column >= Columns)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index.Column,
                $"Column must be between 0 and {Columns - 1}.");
        }
    }
}
