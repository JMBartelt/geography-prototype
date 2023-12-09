using System;
using Microsoft.Geospatial;
public class DistanceCalculator
{
    public static double CalculateDistance(LatLonAlt point1, LatLonAlt point2)
    {
        // Earth's mean radius in meters
        const double earthRadius = 6371000;

        // Convert latitude and longitude to radians
        double lat1 = point1.LatLon.LatitudeInRadians;
        double lon1 = point1.LatLon.LongitudeInRadians;
        double lat2 = point2.LatLon.LatitudeInRadians;
        double lon2 = point2.LatLon.LongitudeInRadians;

        // Calculate the central angle using haversine formula
        double centralAngle = Math.Acos(
            Math.Sin(lat1) * Math.Sin(lat2) +
            Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)
        );

        // Calculate the distance
        double distance = centralAngle * earthRadius;

        return distance;
    }

    // overload that just uses LatLon
    public static double CalculateDistance(LatLon point1, LatLon point2)
    {
        return CalculateDistance(new LatLonAlt(point1, 0), new LatLonAlt(point2, 0));
    }    

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
