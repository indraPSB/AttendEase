namespace AttendEase.Shared.Services;

public interface ILocationService
{
    Task<Location> GetCurrentLocation();
}

public record Location(double Latitude, double Longitude);
