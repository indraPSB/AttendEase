using AttendEase.Shared.Services;
using Microsoft.JSInterop;

namespace AttendEase.Web.Services;

public class LocationService(ILogger<LocationService> logger, IJSRuntime jSRuntime) : ILocationService
{
    private readonly ILogger<LocationService> _logger = logger;
    private readonly IJSRuntime _jSRuntime = jSRuntime;

    public async Task<Location> GetCurrentLocation()
    {
        Location? location = null;

        try
        {
            location = await _jSRuntime.InvokeAsync<Location>("getCurrentLocation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get current location.");
        }

        return location ?? new Location(0, 0);
    }
}
