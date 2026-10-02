namespace ThorusCommon.Thd;

/// <summary>
/// Thrown when a .thd file is corrupt, truncated, or otherwise does not match the
/// expected binary format (wrong size for the 720x360 grid).
/// </summary>
public sealed class ThdFileFormatException : Exception
{
    public ThdFileFormatException(string message) : base(message)
    {
    }

    public ThdFileFormatException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
