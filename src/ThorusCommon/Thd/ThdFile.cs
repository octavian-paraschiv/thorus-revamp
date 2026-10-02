using System.Buffers.Binary;
using ThorusCommon.Grid;

namespace ThorusCommon.Thd;

/// <summary>
/// Reads and writes .thd binary files: a flat sequence of 32-bit little-endian
/// floating point values, one per grid cell, in row-major order starting from the
/// north-west corner (matching <see cref="PlanetaryGrid"/>'s cell layout). No
/// format version header is included, per specification.
/// </summary>
public static class ThdFile
{
    /// <summary>
    /// Reads a .thd file into a flat array of values, one per grid cell, in
    /// row-major NW-origin order. The array length always equals
    /// <see cref="PlanetaryGrid.CellCount"/>.
    /// </summary>
    /// <exception cref="ThdFileFormatException">
    /// Thrown if the file size does not match the expected number of cells.
    /// </exception>
    public static float[] Read(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Read(stream, filePath);
    }

    /// <summary>
    /// Reads .thd-formatted data from a stream into a flat array of values, one per
    /// grid cell, in row-major NW-origin order.
    /// </summary>
    /// <param name="stream">The stream to read from. Not closed by this method.</param>
    /// <param name="sourceDescription">A human-readable description of the source, used in error messages.</param>
    public static float[] Read(Stream stream, string sourceDescription)
    {
        const int cellCount = PlanetaryGrid.CellCount;
        const int expectedByteLength = cellCount * sizeof(float);

        if (stream.CanSeek && stream.Length != expectedByteLength)
        {
            throw new ThdFileFormatException(
                $"'{sourceDescription}' has an unexpected size of {stream.Length} bytes; " +
                $"expected {expectedByteLength} bytes for a {PlanetaryGrid.Columns}x{PlanetaryGrid.Rows} grid.");
        }

        var buffer = new byte[expectedByteLength];
        var totalRead = ReadFully(stream, buffer);

        if (totalRead != expectedByteLength)
        {
            throw new ThdFileFormatException(
                $"'{sourceDescription}' is truncated: read {totalRead} bytes, expected {expectedByteLength} bytes.");
        }

        var values = new float[cellCount];
        for (var i = 0; i < cellCount; i++)
        {
            var span = buffer.AsSpan(i * sizeof(float), sizeof(float));
            values[i] = BinaryPrimitives.ReadSingleLittleEndian(span);
        }

        return values;
    }

    /// <summary>
    /// Writes a flat array of values, one per grid cell in row-major NW-origin
    /// order, to a .thd file. The array must have exactly
    /// <see cref="PlanetaryGrid.CellCount"/> elements.
    /// </summary>
    public static void Write(string filePath, IReadOnlyList<float> values)
    {
        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        Write(stream, values);
    }

    /// <summary>
    /// Writes .thd-formatted data to a stream from a flat array of values, one per
    /// grid cell, in row-major NW-origin order.
    /// </summary>
    /// <param name="stream">The stream to write to. Not closed by this method.</param>
    public static void Write(Stream stream, IReadOnlyList<float> values)
    {
        if (values.Count != PlanetaryGrid.CellCount)
        {
            throw new ArgumentException(
                $"Expected exactly {PlanetaryGrid.CellCount} values ({PlanetaryGrid.Columns}x{PlanetaryGrid.Rows} grid), " +
                $"but received {values.Count}.",
                nameof(values));
        }

        var buffer = new byte[values.Count * sizeof(float)];
        for (var i = 0; i < values.Count; i++)
        {
            var span = buffer.AsSpan(i * sizeof(float), sizeof(float));
            BinaryPrimitives.WriteSingleLittleEndian(span, values[i]);
        }

        stream.Write(buffer, 0, buffer.Length);
    }

    private static int ReadFully(Stream stream, byte[] buffer)
    {
        var totalRead = 0;
        int bytesRead;

        while (totalRead < buffer.Length &&
               (bytesRead = stream.Read(buffer, totalRead, buffer.Length - totalRead)) > 0)
        {
            totalRead += bytesRead;
        }

        return totalRead;
    }
}
