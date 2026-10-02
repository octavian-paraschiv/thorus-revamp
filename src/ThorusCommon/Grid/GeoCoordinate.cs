namespace ThorusCommon.Grid;

/// <summary>
/// Represents a point on the Earth's surface using geographic latitude and longitude,
/// expressed in degrees. Latitude ranges from -90 (south pole) to +90 (north pole).
/// Longitude ranges from -180 (west) to +180 (east).
/// </summary>
public readonly record struct GeoCoordinate(double Latitude, double Longitude)
{
    public double LatitudeRadians => Latitude * Math.PI / 180.0;

    public double LongitudeRadians => Longitude * Math.PI / 180.0;
}
