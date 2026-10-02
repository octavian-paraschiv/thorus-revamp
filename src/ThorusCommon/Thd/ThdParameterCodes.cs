namespace ThorusCommon.Thd;

/// <summary>
/// Maps each <see cref="ThdParameter"/> to its legacy-viewer-compatible code, used
/// both as part of the snapshot file naming convention and for diagnostics.
/// </summary>
public static class ThdParameterCodes
{
    private static readonly IReadOnlyDictionary<ThdParameter, string> CodesByParameter =
        new Dictionary<ThdParameter, string>
        {
            [ThdParameter.SurfaceTemperature] = "T_TS",
            [ThdParameter.SoilTemperature] = "T_TL",
            [ThdParameter.WaterTemperature] = "T_TW",
            [ThdParameter.ActualMinTemperature] = "T_SL",
            [ThdParameter.ActualMaxTemperature] = "T_SH",
            [ThdParameter.NormalMinTemperature] = "T_NL",
            [ThdParameter.NormalMaxTemperature] = "T_NH",
            [ThdParameter.DeltaMinTemperature] = "T_DL",
            [ThdParameter.DeltaMaxTemperature] = "T_DH",
            [ThdParameter.Humidity] = "H_DD",
            [ThdParameter.PressureSeaLevel] = "P_00",
            [ThdParameter.PressureLowLevel] = "P_01",
            [ThdParameter.PressureMidLevel] = "P_02",
            [ThdParameter.PressureTopLevel] = "P_03",
            [ThdParameter.WindSpeedLatitudinal] = "W_00",
            [ThdParameter.WindSpeedLongitudinal] = "W_10",
            [ThdParameter.Precipitation] = "C_00",
            [ThdParameter.SnowDepth] = "N_00",
            [ThdParameter.NewSnow] = "N_DD",
            [ThdParameter.FogConditions] = "F_SI",
            [ThdParameter.ThunderstormOdds] = "L_00",
            [ThdParameter.Albedo] = "A_00",
        };

    private static readonly IReadOnlyDictionary<string, ThdParameter> ParametersByCode =
        CodesByParameter.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    /// <summary>Gets the legacy-viewer-compatible code for the given parameter (e.g., "T_TS").</summary>
    public static string GetCode(ThdParameter parameter)
    {
        if (!CodesByParameter.TryGetValue(parameter, out var code))
        {
            throw new ArgumentOutOfRangeException(nameof(parameter), parameter, "Unknown .thd parameter.");
        }

        return code;
    }

    /// <summary>Resolves a <see cref="ThdParameter"/> from its legacy-viewer-compatible code (e.g., "T_TS").</summary>
    public static ThdParameter GetParameter(string code)
    {
        if (!ParametersByCode.TryGetValue(code, out var parameter))
        {
            throw new ArgumentOutOfRangeException(nameof(code), code, "Unrecognized .thd parameter code.");
        }

        return parameter;
    }
}
