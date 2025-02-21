using AttendEase.Shared.Services;
using Microsoft.Extensions.Logging;
using DeviceLocation = Microsoft.Maui.Devices.Sensors.Location;
using ServiceLocation = AttendEase.Shared.Services.Location;

namespace AttendEase.Services;

internal class LocationService(ILogger<LocationService> logger) : ILocationService
{
    private readonly ILogger<LocationService> _logger = logger;

    public async Task<ServiceLocation> GetCurrentLocation()
    {
        DeviceLocation? location = null;

        try
        {
            await Geolocation.GetLastKnownLocationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get last known location.");
        }

        if (location is null)
        {
            try
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get location.");
            }
        }

        if (location is null)
        {
            return new ServiceLocation(0, 0);
        }

        return new ServiceLocation(location.Latitude, location.Longitude);
    }
}
