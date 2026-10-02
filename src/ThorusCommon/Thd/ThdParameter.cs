namespace ThorusCommon.Thd;

/// <summary>
/// Identifies each weather parameter that can be stored in a .thd file, using
/// legacy-viewer-compatible codes as the canonical names.
/// </summary>
public enum ThdParameter
{
    /// <summary>Surface temperature (redundant combination of soil/water temperature): T_TS.</summary>
    SurfaceTemperature,

    /// <summary>Soil temperature (zero where the surface is water): T_TL.</summary>
    SoilTemperature,

    /// <summary>Water temperature (zero where the surface is land): T_TW.</summary>
    WaterTemperature,

    /// <summary>Actual air min (nocturnal) temperature: T_SL.</summary>
    ActualMinTemperature,

    /// <summary>Actual air max (diurnal) temperature: T_SH.</summary>
    ActualMaxTemperature,

    /// <summary>Normal (climatological) air min (nocturnal) temperature: T_NL.</summary>
    NormalMinTemperature,

    /// <summary>Normal (climatological) air max (diurnal) temperature: T_NH.</summary>
    NormalMaxTemperature,

    /// <summary>Delta actual-to-normal air min (nocturnal) temperature: T_DL.</summary>
    DeltaMinTemperature,

    /// <summary>Delta actual-to-normal air max (diurnal) temperature: T_DH.</summary>
    DeltaMaxTemperature,

    /// <summary>Humidity: H_DD.</summary>
    Humidity,

    /// <summary>Air pressure at the 1000 hPa (sea level) layer: P_00.</summary>
    PressureSeaLevel,

    /// <summary>Air pressure at the 850 hPa (low level) layer: P_01.</summary>
    PressureLowLevel,

    /// <summary>Air pressure at the 500 hPa (mid level) layer: P_02.</summary>
    PressureMidLevel,

    /// <summary>Air pressure at the 300 hPa (top/jet stream level) layer: P_03.</summary>
    PressureTopLevel,

    /// <summary>Speed of the wind component in the "latitudinal" direction: W_00.</summary>
    WindSpeedLatitudinal,

    /// <summary>Speed of the wind component in the "longitudinal" direction: W_10.</summary>
    WindSpeedLongitudinal,

    /// <summary>Precipitation amount and type, combined into a single encoded value: C_00.</summary>
    Precipitation,

    /// <summary>Total snow depth: N_00.</summary>
    SnowDepth,

    /// <summary>New deposited snow: N_DD.</summary>
    NewSnow,

    /// <summary>Fog conditions: F_SI.</summary>
    FogConditions,

    /// <summary>Thunderstorm odds: L_00.</summary>
    ThunderstormOdds,

    /// <summary>Albedo, as a percentage: A_00.</summary>
    Albedo,
}
