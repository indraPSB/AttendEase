namespace AttendEase.Shared.Services;

public interface ILocationService
{
    Task<Location> GetCurrentLocation();

    async Task<bool> IsWithinPremises(Location premisesLocation, double toleranceInMeters)
    {
        (double currentLatitude, double currentLongitude) = await GetCurrentLocation();
        (double premisesLatitude, double premisesLongitude) = premisesLocation;

        double distance = HaversineDistance(currentLatitude, currentLongitude, premisesLatitude, premisesLongitude);

        // Distance is within the tolerance
        return distance <= toleranceInMeters;
    }

    /// <summary>
    /// Calculate the distance between two points on the Earth's surface using the Haversine formula.
    /// </summary>
    /// <param name="latitude1">Latitude of the first point in degrees.</param>
    /// <param name="longitude1">Longitude of the first point in degrees.</param>
    /// <param name="latitude2">Latitude of the second point in degrees.</param>
    /// <param name="longitude2">Longitude of the second point in degrees.</param>
    /// <returns></returns>
    static double HaversineDistance(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        // Static method to convert degrees to radians
        static double ToRadians(double degrees) => degrees * (Math.PI / 180);

        // https://en.wikipedia.org/wiki/Earth_radius
        // Earth's radius in meters
        const double EarthRadius = 6_371_000;

        // https://en.wikipedia.org/wiki/Haversine_formula
        // The haversine formula determines the great-circle distance between two points on a sphere given their longitudes and latitudes.

        // φ1, φ2
        // λ1, λ2
        (double phi1, double phi2) = (latitude1, latitude2);
        (double lambda1, double lambda2) = (longitude1, longitude2);

        // Δφ = φ2 - φ1
        // Δλ = λ2 - λ1
        double deltaPhi = ToRadians(phi2 - phi1);
        double deltaLambda = ToRadians(lambda2 - lambda1);

        // havθ = hav(Δφ) + cos(φ1).cos(φ2).hav(Δλ)
        //   where hav(θ) = sin²(θ/2)
        // havθ = sin²(Δφ/2) + cos(φ1).cos(φ2).sin²(Δλ/2)
        double havDeltaPhi = Math.Sin(deltaPhi / 2);
        havDeltaPhi *= havDeltaPhi;
        double havDeltaLambda = Math.Sin(deltaLambda / 2);
        havDeltaLambda *= havDeltaLambda;
        double cosPhi1 = Math.Cos(ToRadians(phi1));
        double cosPhi2 = Math.Cos(ToRadians(phi2));
        double havTheta = havDeltaPhi + cosPhi1 * cosPhi2 * havDeltaLambda;

        // d = 2r.arcsin(√havθ)
        double distance = 2 * EarthRadius * Math.Asin(Math.Sqrt(havTheta));

        // Distance in meters
        return distance;
    }
}

public record Location(double Latitude, double Longitude);
