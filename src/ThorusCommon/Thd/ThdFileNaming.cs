namespace ThorusCommon.Thd;

/// <summary>
/// Builds and parses .thd snapshot file names following the convention
/// <c>{parameter}_MAP_{yyyy-MM-dd}_00.thd</c>.
/// </summary>
public static class ThdFileNaming
{
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Builds the snapshot file name for the given parameter and simulated date (UTC).
    /// </summary>
    public static string BuildFileName(ThdParameter parameter, DateOnly simulatedDateUtc)
    {
        var code = ThdParameterCodes.GetCode(parameter);
        var datePart = simulatedDateUtc.ToString(DateFormat, System.Globalization.CultureInfo.InvariantCulture);
        return $"{code}_MAP_{datePart}_00.thd";
    }

    /// <summary>
    /// Attempts to parse a snapshot file name (without directory path) into its
    /// constituent parameter and simulated date. Returns false if the name does not
    /// match the expected convention.
    /// </summary>
    public static bool TryParseFileName(string fileName, out ThdParameter parameter, out DateOnly simulatedDateUtc)
    {
        parameter = default;
        simulatedDateUtc = default;

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var parts = nameWithoutExtension.Split('_');

        // Expected format: {codePart1}_{codePart2}_MAP_{yyyy-MM-dd}_00
        // e.g. T_TS_MAP_2026-10-01_00 -> ["T", "TS", "MAP", "2026-10-01", "00"]
        if (parts.Length != 5 || parts[2] != "MAP" || parts[4] != "00")
        {
            return false;
        }

        var code = $"{parts[0]}_{parts[1]}";

        try
        {
            parameter = ThdParameterCodes.GetParameter(code);
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }

        if (!DateOnly.TryParseExact(parts[3], DateFormat, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out simulatedDateUtc))
        {
            return false;
        }

        return true;
    }
}
